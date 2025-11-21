using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public class PanelSwitch : DependentColorUI
{
    [SerializeField] private List<PanelToggle> _panelToggles;
    [SerializeField] private ScrollRect _scrollRect;

    private PanelToggleBase _currentToggle;
    private bool _isInited;

    public event Action PanelSwitched;

    public override void ApplyColors(GameColors gameColors)
    {
        foreach(var panelToggle in _panelToggles)
            panelToggle.ApplyColors(gameColors);

        if (!_isInited)
        {
            Init();
            return;
        }

        ApplyCurrentPanelToggle();
    }

    private void OnEnable()
    {
        foreach (var panelToggle in _panelToggles)
        {
            panelToggle.Triggered += ApplyPanelToggle;
            panelToggle.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (var panelToggle in _panelToggles)
        {
            panelToggle.Triggered -= ApplyPanelToggle;
            panelToggle.Disable();
        }
    }

    public RectTransform GetContent(PanelToggleType panelToggleType, PanelSubToggleType panelSubToggleType)
    {
        PanelToggle panelToggle = _panelToggles.FirstOrDefault(pt => pt.PanelToggleType == panelToggleType);

        if (panelToggle != null)
            return panelToggle.GetContent(panelSubToggleType);

        return null;
    }

    private void Init()
    {
        foreach (var panelToggle in _panelToggles)
            panelToggle.Initialize(OnInitialized);

        _isInited = true;
    }

    private void OnInitialized(PanelToggleBase panelToggle)
    {
        if (!_panelToggles.Where(toggle => !toggle.IsInitialized).Any())
        {
            PanelToggle startPanelToggle = _panelToggles.FirstOrDefault(panelToggle => panelToggle.PanelToggleType == PanelToggleType.GeneralToggle);
            ApplyPanelToggleWithoutNotify(startPanelToggle);
        }
    }

    private void ApplyPanelToggleWithoutNotify(PanelToggleBase panelToggle)
    {
        if (_currentToggle == panelToggle)
            return;

        _currentToggle = panelToggle;

        ApplyCurrentPanelToggle();
    }

    private void ApplyPanelToggle(PanelToggleBase triggeredPanelToggle)
    {
        ApplyPanelToggleWithoutNotify(triggeredPanelToggle);
        PanelSwitched?.Invoke();
    }

    private void ApplyCurrentPanelToggle()
    {
        foreach (PanelToggle panelToggle in _panelToggles)
            panelToggle.Unselect();

        _currentToggle.Select(ApplyContent);
    }

    private void ApplyContent(RectTransform content)
    {
        if (_scrollRect.content != content)
            _scrollRect.content = content;
    }
}
