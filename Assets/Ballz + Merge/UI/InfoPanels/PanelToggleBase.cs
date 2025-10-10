using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class PanelToggleBase : MonoBehaviour
{
    [SerializeField] private Button _triggerButton;
    [SerializeField] private PanelToggleView _toggleView;
    [SerializeField] private RectTransform _content;

    public RectTransform Content => _content;
    public bool IsInitialized { get; private set; }

    public event Action<PanelToggleBase> Triggered;

    public virtual void Initialize(Action<PanelToggleBase> afterInitialized = null)
    {
        _toggleView.Initialize();
        IsInitialized = true;
    }

    public virtual void Enable()
    {
        _triggerButton.onClick.AddListener(OnClickButton);
    }

    public virtual void Disable()
    {
        _triggerButton.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        Triggered?.Invoke(this);
    }

    public virtual void Select(Action<RectTransform> applyContent = null)
    {
        _toggleView.Select();
        _content.SetActiveIfNotNull(true);
    }

    public virtual void Unselect()
    {
        _toggleView.Unselect();
        _content.SetActiveIfNotNull(false);
    }

    public abstract RectTransform GetContent(PanelSubToggleType subToggleType = default);
}
