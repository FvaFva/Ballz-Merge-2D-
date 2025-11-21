using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PanelToggle : PanelToggleBase
{
    [SerializeField] private List<PanelSubToggle> _subToggles;
    [SerializeField] private PanelToggleType _panelToggleType;

    private PanelSubToggle _currentSubToggle;
    private Action<RectTransform> _applyContent;

    public PanelToggleType PanelToggleType => _panelToggleType;

    public override void Initialize(Action<PanelToggleBase> afterInitialized = null)
    {
        base.Initialize(afterInitialized);

        foreach (PanelSubToggle panelSubToggle in _subToggles)
            panelSubToggle.Initialize(afterInitialized);

        afterInitialized?.Invoke(this);
    }

    public override void ApplyColors(GameColors gameColors)
    {
        base.ApplyColors(gameColors);

        foreach (PanelSubToggle panelSubToggle in _subToggles)
            panelSubToggle.ApplyColors(gameColors);
    }

    public override void Enable()
    {
        base.Enable();

        foreach (PanelSubToggle panelSubToggle in _subToggles)
            panelSubToggle.Enable();
    }

    public override void Disable()
    {
        base.Disable();

        foreach (PanelSubToggle panelSubToggle in _subToggles)
            panelSubToggle.Disable();
    }

    public override void Select(Action<RectTransform> applyContent)
    {
        _applyContent = applyContent;
        base.Select(_applyContent);

        if (!_subToggles.Any())
        {
            applyContent?.Invoke(Content);
            return;
        }

        foreach (var subToggle in _subToggles)
        {
            subToggle.Triggered += ChangeCurrentSubToggle;
            subToggle.SetState(true);
        }

        if (_currentSubToggle == null)
        {
            _currentSubToggle = _subToggles.FirstOrDefault();
            _currentSubToggle.Select();
            applyContent?.Invoke(_currentSubToggle.GetContent());
            return;
        }

        SelectSubToggle(_currentSubToggle);
    }

    public override void Unselect()
    {
        base.Unselect();

        if (!_subToggles.Any())
            return;

        foreach (var subToggle in _subToggles)
        {
            subToggle.Triggered -= ChangeCurrentSubToggle;
            subToggle.Unselect();
            subToggle.SetState(false);
        }
    }

    public override RectTransform GetContent(PanelSubToggleType subToggleType)
    {
        if (!_subToggles.Any())
            return Content;

        return _subToggles.Find(subToggle => subToggle.PanelSubToggleType == subToggleType).GetContent();
    }

    private void ChangeCurrentSubToggle(PanelToggleBase panelSubToggle)
    {
        if (_currentSubToggle == panelSubToggle)
            return;

        SelectSubToggle(panelSubToggle);
    }

    private void SelectSubToggle(PanelToggleBase panelSubToggle)
    {
        foreach (var subToggle in _subToggles)
            subToggle.Unselect();

        _currentSubToggle = panelSubToggle as PanelSubToggle;
        _currentSubToggle.Select();
        _applyContent?.Invoke(_currentSubToggle.GetContent());
    }
}
