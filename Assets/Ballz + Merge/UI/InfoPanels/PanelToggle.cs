using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelToggle : MonoBehaviour
{
    [SerializeField] private Button _triggerButton;
    [SerializeField] private PanelToggleView _toggleView;
    [SerializeField] private RectTransform _content;

    public RectTransform Content => _content;

    public event Action<PanelToggle> Triggered;
    public event Action<RectTransform> Selected;
    public event Action<PanelToggle> Initialized;

    public void Initialize()
    {
        _toggleView.Initialized += OnInitialized;
        _toggleView.Initialize();
    }

    public void Enable()
    {
        _triggerButton.onClick.AddListener(OnClickButton);
    }

    public void Disable()
    {
        _triggerButton.onClick.RemoveListener(OnClickButton);
    }

    private void OnInitialized()
    {
        Initialized?.Invoke(this);
        _toggleView.Initialized -= OnInitialized;
    }

    private void OnClickButton()
    {
        Triggered?.Invoke(this);
    }

    public void Select()
    {
        _toggleView.Select();
        _content.gameObject.SetActive(true);
        Selected?.Invoke(_content);
    }

    public void Unselect()
    {
        _toggleView.Unselect();
        _content.gameObject.SetActive(false);
    }
}
