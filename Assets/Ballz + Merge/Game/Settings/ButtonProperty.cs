using System;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public struct ButtonProperty
{
    public AnimatedButton AnimatedButton;
    public Button Button;

    public readonly void ChangeState(bool state)
    {
        AnimatedButton.SetState(state);
        Button.interactable = state;
    }

    public readonly void ChangeListeningState(UnityAction action, bool isActive) => Button.ChangeListeningState(action, isActive);
}