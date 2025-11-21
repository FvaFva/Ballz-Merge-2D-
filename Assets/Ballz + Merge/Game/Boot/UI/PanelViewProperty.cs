using System;
using UnityEngine;

[Serializable]
public struct PanelViewProperty
{
    public InfoPanelView InfoPanelView;
    public Vector2 HorizontalAnchorsMin;
    public Vector2 HorizontalAnchorsMax;
    public Vector2 VerticalAnchorsMin;
    public Vector2 VerticalAnchorsMax;

    public void ChangeAnchors(Vector2 minAnchors, Vector2 maxAnchors)
    {
        InfoPanelView.ChangeAnchors(minAnchors, maxAnchors);
    }
}