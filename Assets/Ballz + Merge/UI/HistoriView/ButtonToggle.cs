using System;
using TMPro;
using UnityEngine;

public class ButtonToggle : DependentColorUI, IDisposable
{
    [SerializeField] private AnimatedButton _toggle;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private ToggleView _toggleView;

    public bool State { get; private set; }

    private string _originalLabel;
    private string _firstToggleLabel;
    private string _secondToggleLabel;

    private Action<ButtonToggle> _triggered;

    public ButtonToggle Initialize(string firstToggleLabel, string secondToggleLabel)
    {
        _originalLabel = _label.text;
        _toggle.Triggered += ChangeState;
        _toggleView.Initialize();

        _firstToggleLabel = firstToggleLabel;
        _secondToggleLabel = secondToggleLabel;
        return this;
    }

    public override void ApplyColors(GameColors gameColors)
    {
        _toggleView.ApplyColors(gameColors);
    }

    public void SetTrigger(Action<ButtonToggle> onTrigger) => _triggered = onTrigger;

    public void Dispose()
    {
        _toggle.PerformIfNotNull(toggle => toggle.Triggered -= ChangeState);
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