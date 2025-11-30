using System;
using UnityEngine;

public class PanelSubToggle : PanelToggleBase
{
    [SerializeField] private PanelSubToggleType _panelSubToggleType;

    public PanelSubToggleType PanelSubToggleType => _panelSubToggleType;

    public override void Select(Action<RectTransform> applyContent = null)
    {
        gameObject.SetActive(true);
        base.Select();
        applyContent?.Invoke(Content);
    }

    public override void Unselect()
    {
        base.Unselect();
    }

    public void SetState(bool state)
    {
        gameObject.SetActive(state);
    }

    public override RectTransform GetContent(PanelSubToggleType subToggleType = default)
    {
        return Content;
    }
}
