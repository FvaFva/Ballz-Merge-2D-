using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class ButtonToggle
{
    public Button Toggle;
    public TMP_Text Label;

    private string _originalLabel;
    private string _ascendingText;
    private string _descendingText;
    private bool _isAscending;

    private Action<ButtonToggle> _triggered;

    public void Initialize(string ascendingText, string descendingText, Action<ButtonToggle> onTrigger)
    {
        if (Label != null)
            _originalLabel = Label.text;

        if (Toggle != null)
            Toggle.onClick.AddListener(AppendLabel);

        _triggered = onTrigger;

        _ascendingText = ascendingText;
        _descendingText = descendingText;
    }

    public void Close()
    {
        if (Toggle != null)
            Toggle.onClick.RemoveListener(AppendLabel);
    }

    public void ResetLabel()
    {
        if (Label != null)
            Label.text = _originalLabel;
    }

    private void AppendLabel()
    {
        if (Label == null)
            return;

        _isAscending = !_isAscending;
        Label.text = _isAscending ? $"{_originalLabel} {_ascendingText}" : $"{_originalLabel} {_descendingText}";
        _triggered?.Invoke(this);
    }
}