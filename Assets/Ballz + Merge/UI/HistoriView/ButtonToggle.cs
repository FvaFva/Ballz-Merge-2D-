using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonToggle : IDisposable
{
    [SerializeField] private Button _toggle;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private ToggleView _toggleView;

    private string _originalLabel;
    private string _firstToggleLabel;
    private string _secondToggleLabel;
    public bool State { get; private set; }

    private Action<ButtonToggle> _triggered;

    public ButtonToggle Initialize(string firstToggleLabel, string secondToggleLabel)
    {
        _originalLabel = _label.text;
        _toggle.onClick.AddListener(ChangeState);
        _toggleView.Initialize();

        _firstToggleLabel = firstToggleLabel;
        _secondToggleLabel = secondToggleLabel;
        return this;
    }

    public void SetTrigger(Action<ButtonToggle> onTrigger) => _triggered = onTrigger;

    public void Dispose()
    {
        if (_toggle != null)
            _toggle.onClick.RemoveListener(ChangeState);
    }

    public void ResetLabel()
    {
        _label.text = _originalLabel;
        _toggleView.Unselect();
    }

    public void ChangeState()
    {
        if (_label == null)
            return;

        State = !State;
        _label.text = State ? $"{_originalLabel}{_firstToggleLabel}" : $"{_originalLabel}{_secondToggleLabel}";
        _toggleView.Select();
        _triggered?.Invoke(this);
    }
}