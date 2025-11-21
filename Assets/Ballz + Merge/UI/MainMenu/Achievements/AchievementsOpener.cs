using BallzMerge.Achievement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AchievementsOpener : MonoBehaviour
{
    [SerializeField] private Button _openButton;
    [SerializeField] private AchievementsHistoryView _historyView;

    [Inject] private AchievementsBus _achievementsBus;
    [Inject] private InfoPanelShowcase _infoPanelShowcase;

    private void OnEnable()
    {
        _openButton.AddListener(OpenView);
    }

    private void OnDisable()
    {
        _openButton.RemoveListener(OpenView);
    }

    private void OpenView()
    {
        if (_historyView.SetData(_achievementsBus.GetSettings()))
            _infoPanelShowcase.Show(_historyView);
    }
}
