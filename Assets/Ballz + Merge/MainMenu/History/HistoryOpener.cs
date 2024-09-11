using BallzMerge.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HistoryOpener : MonoBehaviour
{
    [SerializeField] private Button _openButton;
    [SerializeField] private GameHistoryView _historyView;

    [Inject] private DataBaseSource _source;

    private void OnEnable()
    {
        _openButton.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _openButton.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        _historyView.Show(_source.History.GetData());
    }
}
