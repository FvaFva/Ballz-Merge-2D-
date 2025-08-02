using BallzMerge.Data;
using BallzMerge.Gameplay;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using BallzMerge.Root;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GameCycler : MonoBehaviour, ISceneEnterPoint
{
    private const string QuitQuestName = "Quit";
    private const string RestartQuestName = "Restart";
    private const string SaveQuestName = "Save";

    [SerializeField] private UIView _mainUI;
    [SerializeField] private CamerasOperator _operator;
    [SerializeField] private Dropper _dropper;
    [SerializeField] private LevelInGame _level;
    [SerializeField] private List<CyclicBehavior> _components;

    private Dictionary<Type, object> _behaviourMap;
    private Action<SceneExitData> _sceneCallBack;
    private SaveDataContainer _save;
    private SceneExitData _exitData;
    private ConductorBetweenWaves _conductor;

    [Inject] private BlocksBinder _blocksBus;
    [Inject] private UserQuestioner _userQuestioner;
    [Inject] private Ball _ball;
    [Inject] private UIRootView _rootUI;
    [Inject] private DataBaseSource _data;

    public IEnumerable<IInitializable> InitializedComponents => GetFromMap<IInitializable>();
    public IEnumerable<IDependentScreenOrientation> OrientationDepends => GetFromMap<IDependentScreenOrientation>();
    public bool IsAvailable { get; private set; }

    private void Awake()
    {
        BuildBehaviourMap();
        _conductor = new ConductorBetweenWaves(_ball.GetBallComponent<BallAwaitBreaker>(), _dropper, _blocksBus);
        _save = _data.Saves.Get();

        if (_save.IsLoaded)
            _level.Load(_save);
        else
            _level.Init();

        LoadSettings();
        IsAvailable = true;
    }

    private void OnEnable()
    {
        _ball.LeftGame += OnBallLeftGame;
        _conductor.GameFinished += OnGameFinished;
        _conductor.WaveLoaded += OnWaveLoaded;
        _rootUI.EscapeMenu.QuitRequired += OnQuitRequired;
    }

    private void OnDisable()
    {
        _ball.LeftGame -= OnBallLeftGame;
        _conductor.GameFinished -= OnGameFinished;
        _conductor.WaveLoaded -= OnWaveLoaded;
        _rootUI.EscapeMenu.QuitRequired -= OnQuitRequired;
    }

    private void OnDestroy()
    {
        IsAvailable = false;
    }

    public void Init(Action<SceneExitData> callback, bool isLoad)
    {
        if (_conductor == null)
        {
            Debug.LogError("WARNING!! CONDUCTOR WAS FIRED!");
            callback.Invoke(new SceneExitData(ScenesNames.MAIN_MENU));
            return;
        }

        _sceneCallBack = callback;
        _mainUI.Init();
        _rootUI.AttachSceneUI(_mainUI, _operator.UI);
        RestartLevel(isLoad && _save.IsLoaded);
        GetFromMap<IDependentScreenOrientation>().Clear();
    }

    private void OnBallLeftGame()
    {
        _conductor.Continue();
    }

    private void RestartLevel(bool isLoad = false)
    {
        if (isLoad)
            LoadSave();
        else
            _conductor.Start();

        StartLevel(isLoad);
        _data.Saves.EraseAllData();
    }

    private void LoadSettings()
    {
        foreach (var settingsDepend in GetFromMap<IDependentSettings>())
            settingsDepend.ApplySettings(_level.Current);
    }

    private void StartLevel(bool isAfterLoad = false)
    {
        foreach (var starter in GetFromMap<ILevelStarter>())
            starter.StartLevel(isAfterLoad);
    }

    private void LoadSave()
    {
        foreach (var loader in GetFromMap<ISaveDependedObject>())
            loader.Load(_save);
    }

    private SaveDataContainer CreateSave()
    {
        SaveDataContainer save = new SaveDataContainer();

        foreach (var loader in GetFromMap<ISaveDependedObject>())
            loader.Save(save);

        return save;
    }

    private void OnWaveLoaded()
    {
        foreach (var waver in GetFromMap<IWaveUpdater>())
            waver.UpdateWave();
    }

    private void OnGameFinished()
    {
        foreach (var finisher in GetFromMap<ILevelFinisher>())
            finisher.FinishLevel();

        StartQuest(RestartQuestName, "Want one more game?");
    }

    private void OnQuitRequired(SceneExitData exitData)
    {
        _exitData = exitData;
        StartQuest(QuitQuestName, "Really left dat the best run?");
    }

    private void StartQuest(string id, string header)
    {
        _userQuestioner.Show(new UserQuestion(id, header));
        _userQuestioner.Answer += OnUserAnswer;
    }

    private void OnUserAnswer(UserQuestion answer)
    {
        if (answer.Name == RestartQuestName)
        {
            _userQuestioner.Answer -= OnUserAnswer;

            if (answer.IsPositiveAnswer)
                RestartLevel();
            else
                _sceneCallBack.Invoke(new SceneExitData(ScenesNames.MAIN_MENU));
        }
        else if (answer is { Name: QuitQuestName, IsPositiveAnswer: true })
        {
            _userQuestioner.Answer -= OnUserAnswer;

            if (_exitData.TargetScene == ScenesNames.GAMEPLAY)
            {
                _sceneCallBack.Invoke(_exitData);
            }
            else
            {
                StartQuest(SaveQuestName, "Do you want to save your progress?");
            }
        }
        else if (answer.Name == SaveQuestName)
        {
            _userQuestioner.Answer -= OnUserAnswer;

            if (answer.IsPositiveAnswer)
                _exitData.Put(CreateSave());
            else
                _exitData.Put(CreateHistory());

            _sceneCallBack.Invoke(_exitData);
        }
    }

    private GameHistoryData CreateHistory()
    {
        GameHistoryData historyData = new GameHistoryData();

        foreach (var historical in GetFromMap<IHistorical>())
            historyData = historical.Write(historyData);
    
        return historyData;
    }
    
    private void BuildBehaviourMap()
    {
        _behaviourMap = new Dictionary<Type, object>();
        AddToBehaviourMap<ILevelFinisher>();
        AddToBehaviourMap<IInitializable>();
        AddToBehaviourMap<IWaveUpdater>();
        AddToBehaviourMap<IDependentScreenOrientation>();
        AddToBehaviourMap<ISaveDependedObject>();
        AddToBehaviourMap<ILevelStarter>();
        AddToBehaviourMap<IDependentSettings>();
        AddToBehaviourMap<IHistorical>();
    }

    private void AddToBehaviourMap<T>()
    {
        List<T> values = new List<T>();

        foreach (var cyclical in _components.OrderBy(item => item.Order))
        {
            if (cyclical is T value)
                values.Add(value);
        }

        _behaviourMap.Add(typeof(T), values);
    }

    private List<T> GetFromMap<T>() => _behaviourMap[typeof(T)] as List<T>;
}