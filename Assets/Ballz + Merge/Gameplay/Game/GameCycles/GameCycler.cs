using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Root;
using System;
using System.Collections;
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
    SceneExitData _quiteRequireData;

    [Inject] private BlocksBus _blocksBus;
    [Inject] private UserQuestioner _userQuestioner;
    [Inject] private Ball _ball;
    [Inject] private UIRootView _rootUI;

    public IEnumerable<IInitializable> InitializedComponents => _initializedComponents;

    public bool IsAvailable {  get; private set; }

    private void Start()
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
        }

        IsAvailable = true;
    }

    private void OnEnable()
    {
        _blocksBus.BlockFinished += OnBlockFinished;
        _ball.EnterAim += OnBallEnterAim;
        _rootUI.SettingsMenu.QuitRequired += OnMenuQuitRequire;
    }

    private void OnDisable()
    {
        _blocksBus.BlockFinished -= OnBlockFinished;
        _ball.EnterAim -= OnBallEnterAim;
        _rootUI.SettingsMenu.QuitRequired -= OnMenuQuitRequire;
    }

    private void OnDestroy()
    {
        IsAvailable = false;
    }

    public void Init(Action<SceneExitData> callback)
    {
        _sceneCallBack = callback;
        _rootUI.AttachSceneUI(_mainUI);
        RestartLevel();
    }

    private void RestartLevel()
    {
        foreach (ILevelStarter starter in _starters)
            starter.StartLevel();
    }

    private void OnBallEnterAim()
    {
        foreach(IWaveUpdater waver in _wavers)
            waver.UpdateWave();
    }

    private void OnBlockFinished()
    {
        StartCoroutine(DelayedLevelFinish());
    }

    private void OnMenuQuitRequire(SceneExitData exitData)
    {
        _quiteRequireData = exitData;
        StartQuest(QuitQuestName, "Really left dat the best run?");
    }

    private IEnumerator DelayedLevelFinish()
    {
        yield return new WaitForFixedUpdate();

        foreach (ILevelFinisher finisher in _finishers)
            finisher.FinishLevel();

        StartQuest(RestartQuestName, "Want one more game?");
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
            _sceneCallBack.Invoke(_quiteRequireData);
        }
    }
}