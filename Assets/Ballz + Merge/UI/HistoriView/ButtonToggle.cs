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
    private string _ascendingText;
    private string _descendingText;
    public bool State { get; private set; }

    private Action<ButtonToggle> _triggered;

    public ButtonToggle Initialize(string ascendingText, string descendingText, Action<ButtonToggle> onTrigger)
    {
        if (_label != null)
            _originalLabel = _label.text;

        if (_toggle != null)
            _toggle.onClick.AddListener(ChangeState);

        _triggered = onTrigger;
        _ascendingText = ascendingText;
        _descendingText = descendingText;
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
        _label.text = State ? $"{_originalLabel} {_ascendingText}" : $"{_originalLabel} {_descendingText}";
        _triggered?.Invoke(this);
    }
}