using BallzMerge.Gameplay;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using BallzMerge.Root;
using ModestTree;
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
    [SerializeField] private List<CyclicBehavior> _components;

    private List<ILevelFinisher> _finishers = new List<ILevelFinisher>();
    private List<IInitializable> _initializedComponents = new List<IInitializable>();
    private List<IWaveUpdater> _wavers = new List<IWaveUpdater>();
    private List<IDependentScreenOrientation> _orientators = new List<IDependentScreenOrientation>();
    private List<ILevelLoader> _loaders = new List<ILevelLoader>();
    private List<ILevelSaver> _savers = new List<ILevelSaver>();
    private Action<SceneExitData> _sceneCallBack;
    private SceneExitData _exitData;
    private ConductorBetweenWaves _conductor;

    [Inject] private BlocksBinder _blocksBus;
    [Inject] private UserQuestioner _userQuestioner;
    [Inject] private Ball _ball;
    [Inject] private UIRootView _rootUI;

    public IEnumerable<IInitializable> InitializedComponents => _initializedComponents;
    public IEnumerable<IDependentScreenOrientation> Orientators => _orientators;
    public bool IsAvailable { get; private set; }

    private void Awake()
    {
        int i = 0;

        foreach (var cyclical in _components.OrderBy(item => item.Order))
        {
            Debug.Log($"{++i} {cyclical.gameObject.name} {cyclical.Order}");

            if (cyclical is IInitializable initializeComponent)
                _initializedComponents.Add(initializeComponent);

            if (cyclical is ILevelFinisher finisher)
                _finishers.Add(finisher);

            if (cyclical is IWaveUpdater waver)
                _wavers.Add(waver);

            if (cyclical is IDependentScreenOrientation orientator)
                _orientators.Add(orientator);

            if (cyclical is ILevelLoader loader)
                _loaders.Add(loader);

            if (cyclical is ILevelSaver saver)
                _savers.Add(saver);

            if (cyclical is Dropper dropper)
                _conductor = new ConductorBetweenWaves(_ball.GetBallComponent<BallAwaitBreaker>(), dropper, _blocksBus);
        }

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

    public void Init(Action<SceneExitData> callback, IDictionary<string, object> loadData = null)
    {
        if (_conductor == null)
        {
            Debug.LogError("WARNING!! CONDUCTOR WAS FIRED!");
            callback.Invoke(new SceneExitData(ScenesNames.MAINMENU));
            return;
        }

        _sceneCallBack = callback;
        _mainUI.Init();
        _rootUI.AttachSceneUI(_mainUI, _operator.UI);
        RestartLevel(loadData);
        _orientators.Clear();
    }

    private void OnBallLeftGame()
    {
        _conductor.Continue();
    }

    private void RestartLevel(IDictionary<string, object> loadData = null)
    {
        if (loadData == null || ContainsEmptyValue(loadData))
        {
            LoadLevel();
            _conductor.Start();
        }
        else
        {
            LoadLevel(loadData);
        }
    }

    private bool ContainsEmptyValue(IDictionary<string, object> data)
    {
        foreach (KeyValuePair<string, object> pair in data)
        {
            if (string.IsNullOrEmpty(pair.Value?.ToString()))
                return true;
        }

        return false;
    }

    private void LoadLevel()
    {
        foreach (ILevelLoader loader in _loaders)
            loader.StartLevel();
    }

    private void LoadLevel(IDictionary<string, object> loadData)
    {
        foreach (ILevelLoader loader in _loaders)
            loader.Load(loadData);
    }

    private void OnWaveLoaded()
    {
        foreach (IWaveUpdater waver in _wavers)
            waver.UpdateWave();
    }

    private void OnGameFinished()
    {
        foreach (ILevelFinisher finisher in _finishers)
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
                _sceneCallBack.Invoke(new SceneExitData(ScenesNames.MAINMENU));
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
                StartQuest(SaveQuestName, "Do you want yo save your progress?");
            }
        }
        else if (answer.Name == SaveQuestName)
        {
            _userQuestioner.Answer -= OnUserAnswer;

            if (answer.IsPositiveAnswer)
                _exitData.ConnectSavers(_savers);

            _sceneCallBack.Invoke(_exitData);
        }
    }
}