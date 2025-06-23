using BallzMerge.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HistoryOpener : CyclicBehavior, IInitializable
{
    [SerializeField] private Button _openButton;
    [SerializeField] private GameHistoryView _historyView;

    [Inject] private DataBaseSource _source;
    [Inject] private InfoPanelShowcase _infoPanelShowcase;

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
        _isInited = _historyView.SetData(_source.History.GetData());
    }

    private void OpenView()
    {
        if (_isInited)
            _infoPanelShowcase.Show(_historyView);
    }
}
