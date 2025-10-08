using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public abstract class PanelToggleBase : MonoBehaviour
{
    [SerializeField] private Button _triggerButton;
    [SerializeField] private PanelToggleView _toggleView;
    [SerializeField] private RectTransform _content;

    public Button TriggerButton => _triggerButton;
    public PanelToggleView ToggleView => _toggleView;
    public RectTransform Content => _content;
    public bool IsInitialized { get; private set; }

    public event Action<PanelToggleBase> Triggered;

    public virtual void Initialize(Action<PanelToggleBase> afterInitialized = null)
    {
        ToggleView.Initialize();
        IsInitialized = true;
    }

    public virtual void Enable()
    {
        TriggerButton.onClick.AddListener(OnClickButton);
    }

    public virtual void Disable()
    {
        TriggerButton.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        Triggered?.Invoke(this);
    }

    public virtual void Select(Action<RectTransform> applyContent = null)
    {
        ToggleView.Select();
        _content.SetActiveIfNotNull(true);
    }

    public virtual void Unselect()
    {
        ToggleView.Unselect();
        _content.SetActiveIfNotNull(false);
    }

    public abstract RectTransform GetContent(PanelSubToggleType subToggleType = default);
}
