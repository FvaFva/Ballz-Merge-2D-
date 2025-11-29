using System;
using UnityEngine;

[Serializable]
public struct PanelViewProperty
{
    public InfoPanelView InfoPanelView;
    public Vector2 HorizontalAnchorsMin;
    public Vector2 HorizontalAnchorsMax;
    public Vector2 HorizontalAnchoredPosition;
    public Vector2 VerticalAnchorsMin;
    public Vector2 VerticalAnchorsMax;
    public Vector2 VerticalAnchoredPosition;

    public readonly void ChangeAnchors(Vector2 minAnchors, Vector2 maxAnchors, Vector2 anchoredPosition)
    {
        InfoPanelView.ChangeAnchors(minAnchors, maxAnchors, anchoredPosition);
    }
}