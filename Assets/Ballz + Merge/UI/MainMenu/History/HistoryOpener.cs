using BallzMerge.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HistoryOpener : MonoBehaviour
{
    [SerializeField] private Button _openButton;
    [SerializeField] private GameHistoryView _historyView;

    [Inject] private DataBaseSource _source;
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
        if (_historyView.SetData(_source.History.GetData()))
            _infoPanelShowcase.Show(_historyView);
    }
}
