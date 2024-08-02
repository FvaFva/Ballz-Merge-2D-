using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GameCycler: MonoBehaviour 
{
    private const string RestartQuestName = "Restart";

    [SerializeField] private List<CyclicBehaviour> _components;

    private List<ILevelFinisher> _finishers = new List<ILevelFinisher>();
    private List<ILevelStarter> _starters = new List<ILevelStarter>();
    private List<IWaveUpdater> _wavers = new List<IWaveUpdater>();

    [Inject] private MainInputMap _userInput;
    [Inject] private BlocksBus _blocksBus;
    [Inject] private UserQuestioner _userQuestioner;
    [Inject] private Ball _ball;

    private void Start()
    {
        foreach (var cyclical in _components.OrderBy(item => item.Order))
        {
            cyclical.Init();

            if(cyclical is ILevelFinisher finisher)
                _finishers.Add(finisher);

            if(cyclical is ILevelStarter starter)
                _starters.Add(starter);

            if (cyclical is IWaveUpdater waver)
                _wavers.Add(waver);
        }

        _userInput.Enable();
        RestartLevel();
    }

    private void OnEnable()
    {
        _blocksBus.BlockFinished += OnBlockFinished;
        _ball.EnterAim += OnWaveFinish;
    }

    private void OnDisable()
    {
        _blocksBus.BlockFinished -= OnBlockFinished;
        _ball.EnterAim -= OnWaveFinish;
    }

    private void OnDestroy()
    {
        _userInput.Disable();
    }

    private void OnWaveFinish()
    {
        foreach(IWaveUpdater waver in _wavers)
            waver.UpdateWave();
    }

    private void OnBlockFinished()
    {
        foreach(ILevelFinisher finisher in _finishers)
            finisher.FinishLevel();

        _userQuestioner.Show(new UserQuestion(RestartQuestName, $"Want one more game?"));
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
                Quit();
        }    
    }

    private void RestartLevel()
    {
        foreach(ILevelStarter starter in _starters)
            starter.StartLevel();
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}