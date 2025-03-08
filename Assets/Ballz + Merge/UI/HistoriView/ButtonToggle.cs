using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonToggle
{
    [SerializeField] private Button _toggle;
    [SerializeField] private TMP_Text _label;

    private string _originalLabel;
    private string _firstToggleLabel;
    private string _secondToggleLabel;
    public bool State { get; private set; }

    private Action<ButtonToggle> _triggered;

    public ButtonToggle Initialize(string firstToggleLabel, string secondToggleLabel, Action<ButtonToggle> onTrigger)
    {
        if (_label != null)
            _originalLabel = _label.text;

        if (_toggle != null)
            _toggle.onClick.AddListener(ChangeState);

        _triggered = onTrigger;
        _firstToggleLabel = firstToggleLabel;
        _secondToggleLabel = secondToggleLabel;
        return this;
    }

    public void Close()
    {
        if (_toggle != null)
            _toggle.onClick.RemoveListener(ChangeState);
    }

    public void ResetLabel()
    {
        if (_label != null)
            _label.text = _originalLabel;
    }

    public void ChangeState()
    {
        if (_label == null)
            return;

        State = !State;
        _label.text = State ? $"{_originalLabel}{_firstToggleLabel}" : $"{_originalLabel}{_secondToggleLabel}";
        _triggered?.Invoke(this);
    }
}