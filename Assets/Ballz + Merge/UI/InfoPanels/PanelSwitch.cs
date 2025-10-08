using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public class PanelSwitch : MonoBehaviour
{
    [SerializeField] private List<PanelToggle> _panelToggles;
    [SerializeField] private ScrollRect _scrollRect;

    private PanelToggleBase _currentToggle;

    public event Action PanelSwitched;

    private void Awake()
    {
        foreach (var panelToggle in _panelToggles)
            panelToggle.Initialize(OnInitialized);
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

    private void OnInitialized(PanelToggleBase panelToggle)
    {
        if (!_panelToggles.Where(toggle => !toggle.IsInitialized).Any())
        {
            PanelToggle defaultPanelToggle = _panelToggles.FirstOrDefault(panelToggle => panelToggle.PanelToggleType == PanelToggleType.AudioToggle);
            ApplyPanelToggle(defaultPanelToggle);
        }
    }

    private void ApplyPanelToggle(PanelToggleBase triggeredPanelToggle)
    {
        if (_currentToggle == triggeredPanelToggle)
            return;

        _currentToggle = triggeredPanelToggle;

        foreach (PanelToggle panelToggle in _panelToggles)
            panelToggle.Unselect();

        _currentToggle.Select(ApplyContent);
        PanelSwitched?.Invoke();
    }

    private void ApplyContent(RectTransform content)
    {
        if (_scrollRect.content != content)
            _scrollRect.content = content;
    }
}
