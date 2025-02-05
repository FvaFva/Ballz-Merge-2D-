using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class PanelSwitch : MonoBehaviour
{
    [SerializeField] private List<PanelToggleProperty> _listOfPanels;
    [SerializeField] private ScrollRect _scrollRect;

    private Dictionary<PanelToggleType, PanelToggle> _panelToggles = new();
    private int _initializedCount = 0;

    private void Awake()
    {
        foreach(var panel in _listOfPanels)
            _panelToggles.Add(panel.PanelToggleType, panel.PanelToggle);

        foreach (var panelToggle in _panelToggles)
        {
            panelToggle.Value.Initialized += OnInitialized;
            panelToggle.Value.Initialize();
        }
    }

    private void OnEnable()
    {
        foreach (var panelToggle in _panelToggles)
        {
            panelToggle.Value.Triggered += ApplyPanelToggle;
            panelToggle.Value.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (var panelToggle in _panelToggles)
        {
            panelToggle.Value.Triggered -= ApplyPanelToggle;
            panelToggle.Value.Disable();
        }
    }

    public RectTransform GetContent(PanelToggleType panelToggleType)
    {
        return _panelToggles[panelToggleType].Content;
    }

    private void OnInitialized(PanelToggle panelToggle)
    {
        panelToggle.Initialized -= OnInitialized;
        _initializedCount++;

        if (_initializedCount == _panelToggles.Count)
        {
            ApplyPanelToggle(_panelToggles[PanelToggleType.AudioToggle]);
            _initializedCount = 0;
        }
    }

    private void ApplyPanelToggle(PanelToggle triggeredPanelToggle)
    {
        triggeredPanelToggle.Selected += ApplyContent;

        foreach (var panelToggle in _panelToggles)
            panelToggle.Value.Unselect();

        triggeredPanelToggle.Select();
        triggeredPanelToggle.Selected -= ApplyContent;
    }

    private void ApplyContent(RectTransform content)
    {
        if (_scrollRect.content != content)
            _scrollRect.content = content;
    }
}
