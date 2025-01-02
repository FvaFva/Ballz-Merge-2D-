using UnityEngine;
using UnityEngine.UI;

public abstract class PopupBinder<T> : WindowBinder<T> where T : WindowViewModel
{
    [SerializeField] private Button _closeButton;

    protected virtual void Start()
    {
        _closeButton?.onClick.AddListener(OnCloseButtonClick);
    }

    protected virtual void OnDestroy()
    {
        _closeButton?.onClick.RemoveListener(OnCloseButtonClick);
    }

    protected virtual void OnCloseButtonClick()
    {
        ViewModel.RequestClose();
    }
}
