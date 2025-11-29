using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameFinishView : DependentColorUI
{
    [SerializeField] private Button _exit;
    [SerializeField] private List<DependentColorUI> _backgroundUIs;

    private Action _callback;

    private void OnEnable()
    {
        _exit.AddListener(OnTrigger);
    }

    private void OnDisable()
    {
        _exit.RemoveListener(OnTrigger);
    }

    public override void ApplyColors(GameColors gameColors)
    {
        foreach(var backgroundUI in _backgroundUIs)
            backgroundUI.ApplyColors(gameColors);
    }

    public void Show(Action callback)
    {
        gameObject.SetActive(true);
        _callback = callback;
    }

    private void OnTrigger()
    {
        _callback.Invoke();
    }
}
