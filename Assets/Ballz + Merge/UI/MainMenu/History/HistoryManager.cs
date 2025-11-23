using BallzMerge.Achievement;
using BallzMerge.Data;
using BallzMerge.Root;
using UnityEngine;
using Zenject;

public class HistoryManager : MonoBehaviour
{
    [SerializeField] private ButtonProperty _openButton;
    [SerializeField] private GameHistoryView _historyView;

    [Inject] private DataBaseSource _source;
    [Inject] private UIRootView _uiRoot;

    private bool _isInited;

    private void OnEnable()
    {
        _openButton.ChangeListeningState(OpenView, true);
    }

    private void OnDisable()
    {
        _openButton.ChangeListeningState(OpenView, false);
    }

    public void Init()
    {
        ChangeInit(_historyView.SetData(_source.History.GetData(), _uiRoot.LoadScreen, EraseData));
    }

    private void ChangeInit(bool state)
    {
        _isInited = state;
        _openButton.ChangeState(_isInited);
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
