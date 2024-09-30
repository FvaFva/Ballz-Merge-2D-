using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using BallzMerge.Root;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GameCycler: MonoBehaviour, ISceneEnterPoint
{
    private const string QuitQuestName = "Quit";
    private const string RestartQuestName = "Restart";

    [SerializeField] private UIView _mainUI;
    [SerializeField] private List<CyclicBehavior> _components;

    private List<ILevelFinisher> _finishers = new List<ILevelFinisher>();
    private List<IInitializable> _initializedComponents = new List<IInitializable>();
    private List<ILevelStarter> _starters = new List<ILevelStarter>();
    private List<IWaveUpdater> _wavers = new List<IWaveUpdater>();
    private Action<SceneExitData> _sceneCallBack;
    private SceneExitData _quiteRequireData;
    private ConductorBetweenWaves _conductor;

    [Inject] private BlocksBus _blocksBus;
    [Inject] private UserQuestioner _userQuestioner;
    [Inject] private Ball _ball;
    [Inject] private UIRootView _rootUI;
    [Inject] private GridSettings _gridSettings;

    public IEnumerable<IInitializable> InitializedComponents => _initializedComponents;

    public bool IsAvailable {  get; private set; }

    private void Awake()
    {
        foreach (var cyclical in _components.OrderBy(item => item.Order))
        {
            if (cyclical is IInitializable initializeComponent)
                _initializedComponents.Add(initializeComponent);

            if (cyclical is ILevelFinisher finisher)
                _finishers.Add(finisher);

            if (cyclical is ILevelStarter starter)
                _starters.Add(starter);

            if (cyclical is IWaveUpdater waver)
                _wavers.Add(waver);

            if(cyclical is Dropper dropper)
                _conductor = new ConductorBetweenWaves(_ball.GetBallComponent<BallAwaitBreaker>(), dropper, _blocksBus);
        }

        IsAvailable = true;
    }

    private void OnEnable()
    {
        _ball.LeftGame += OnBallLeftGame;
        _conductor.GameFinished += OnGameFinished;
        _conductor.WaveLoaded += OnWaveLoaded;
        _rootUI.EscapeMenu.QuitRequired += OnMenuQuitRequire;
    }

    private void OnDisable()
    {
        _ball.LeftGame -= OnBallLeftGame;
        _conductor.GameFinished -= OnGameFinished;
        _conductor.WaveLoaded -= OnWaveLoaded;
        _rootUI.EscapeMenu.QuitRequired -= OnMenuQuitRequire;
    }

    private void OnDestroy()
    {
        IsAvailable = false;
    }

    public void Init(Action<SceneExitData> callback)
    {
        if (_conductor == null)
        {
            Debug.LogError("WARNING!! CONDUCTOR WAS FIRED!");
            callback.Invoke(new SceneExitData(ScenesNames.MAINMENU));
            return;
        }

        _sceneCallBack = callback;
        _rootUI.AttachSceneUI(_mainUI);
        RestartLevel();
    }

    private void OnBallLeftGame()
    {
        _conductor.Start();
    }

    private void RestartLevel()
    {
        _gridSettings.ReloadGridSize();

        foreach (ILevelStarter starter in _starters)
            starter.StartLevel();

        _conductor.Start();
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

    private void OnMenuQuitRequire(SceneExitData exitData)
    {
        _quiteRequireData = exitData;
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
        else if (answer is {Name: QuitQuestName, IsPositiveAnswer: true})
        {
            _userQuestioner.Answer -= OnUserAnswer;
            _sceneCallBack.Invoke(_quiteRequireData);
        }
    }
}