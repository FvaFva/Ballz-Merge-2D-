using BallzMerge.Achievement;
using BallzMerge.Data;
using BallzMerge.Root;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HistoryManager : CyclicBehavior, IInitializable
{
    [SerializeField] private Button _openButton;
    [SerializeField] private AnimatedButton _openButtonView;
    [SerializeField] private GameHistoryView _historyView;

    [Inject] private DataBaseSource _source;
    [Inject] private UIRootView _uiRoot;

    private bool _isInited;

    private void OnEnable()
    {
        _openButton.AddListener(OpenView);
    }

    private void OnDisable()
    {
        _openButton.RemoveListener(OpenView);
    }

    public void Init()
    {
        ChangeInit(_historyView.SetData(_source.History.GetData(), _uiRoot.LoadScreen, EraseData));
    }

    private void ChangeInit(bool state)
    {
        _isInited = state;
        _openButton.interactable = _isInited;
        _openButtonView.SetState(_isInited);
    }

    private void OpenView()
    {
        _uiRoot.InfoPanelShowcase.Show(_historyView);
    }

    private void EraseData()
    {
        _source.History.EraseData();
        _source.Achievements.DeleteAchievement(AchievementsTypes.levelComplete);
        _uiRoot.InfoPanelShowcase.Close();
        ChangeInit(false);
    }
}
