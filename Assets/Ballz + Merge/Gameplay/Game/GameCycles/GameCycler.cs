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
        _conductor = new ConductorBetweenWaves(_ball.GetBallComponent<BallAwaitBreaker>(), _dropper, _blocksBus, () => GetFromMap<ICompleteLevelTrigger>().All(f => f.IsReadyToComplete));
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
        _conductor.GameIsLost += OnGameIsLost;
        _conductor.WaveLoaded += OnWaveLoaded;
        _conductor.GameIsComplete += OnCompleteLevel;
        _rootUI.EscapeMenu.QuitRequired += OnQuitRequired;
    }

    private void OnDisable()
    {
        _ball.LeftGame -= OnBallLeftGame;
        _conductor.GameIsLost -= OnGameIsLost;
        _conductor.WaveLoaded -= OnWaveLoaded;
        _conductor.GameIsComplete -= OnCompleteLevel;
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

    private void OnCompleteLevel()
    {
        foreach (var completer in GetFromMap<ILevelCompleter>())
            completer.Complete();

        _exitData.Put(CreateHistory(true));
        _exitData.TargetScene = ScenesNames.MAIN_MENU;
        _mainUI.FinishView.Show(() => _sceneCallBack.Invoke(_exitData));
    }

    private void FinishLevel()
    {
        foreach (var finisher in GetFromMap<ILevelFinisher>())
            finisher.FinishLevel();
    }

    private void OnGameIsLost()
    {
        FinishLevel();
        _userQuestioner.Show(new UserQuestion(HandlerRestartQuestion, "Want one more game?"));
    }

    private void RestartLevel(bool isLoad = false)
    {
        if (isLoad)
            LoadSave();
        else
            FinishLevel();

        StartLevel(isLoad);
        _data.Saves.EraseAllData();
    }

    private void LoadSettings()
    {
        foreach (var settingsDepend in GetFromMap<IDependentSettings>())
            settingsDepend.ApplySettings(_level.Current);
    }

    private void StartLevel(bool isAfterLoad)
    {
        foreach (var starter in GetFromMap<ILevelStarter>())
            starter.StartLevel(isAfterLoad);

        if (isAfterLoad == false)
            _conductor.Start();
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

    private void OnQuitRequired(SceneExitData exitData)
    {
        _exitData = exitData;
        _userQuestioner.Show(new UserQuestion(HandlerQuitQuestion, "Really left dat the best run?"));
    }

    private void HandlerRestartQuestion(bool answer)
    {
        if (answer)
            RestartLevel();
        else
            _sceneCallBack.Invoke(new SceneExitData(ScenesNames.MAIN_MENU));
    }

    private void HandlerQuitQuestion(bool answer)
    {
        if (answer)
        {
            if (_exitData.TargetScene == ScenesNames.GAMEPLAY)
                RestartLevel();
            else
                _userQuestioner.Show(new UserQuestion(HandlerSaveQuestion, "Do you want to save your progress?"));
        }
    }

    private void HandlerSaveQuestion(bool answer)
    {
        if (answer)
            _exitData.Put(CreateSave());
        else
            _exitData.Put(CreateHistory());

        _sceneCallBack.Invoke(_exitData);
    }

    private GameHistoryData CreateHistory(bool isLevelCompleted = false)
    {
        GameHistoryData historyData = new GameHistoryData();
        historyData.IsCompleted = isLevelCompleted;

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
        AddToBehaviourMap<ICompleteLevelTrigger>();
        AddToBehaviourMap<ILevelCompleter>();
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