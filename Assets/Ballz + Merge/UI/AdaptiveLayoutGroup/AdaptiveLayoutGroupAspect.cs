using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveLayoutGroupAspect : AdaptiveLayoutGroupBase
{
    private const int MaxSeparating = 10;
    private const float SafeEpsilon = 1e-6f;

    [SerializeField] private LayoutAspectBehaviour _oversizeBehaviour;
    [SerializeField, Range(One, MaxSeparating)] private int _separate = 1;

    private delegate Vector2 OversizeFix(float childAspect, float availableCross, float totalSpacing, float totalMainSize, float totalCrossSize);
    private OversizeFix _oversizeFix;

    private void OnValidate()
    {
        base.OnValidate();
        CalculateOversize();
    }

    public override AdaptiveLayoutGroupBase Init()
    {
        base.Init();
        CalculateOversize();
        return this;
    }

    protected override float Calculate(float aspect, float availableSize)
    {
        return IsVertical ? availableSize / aspect : availableSize * aspect;
    }

    protected override IEnumerable<KeyValuePair<RectTransform, float>> CalculateChildren()
    {
        return rectChildren
            .Select(child =>
            {
                var data = IsHaveLayout(child, out var layout)
                ? (rect: child, value: GetCrossSize(layout), order: layout.layoutPriority)
                : (rect: child, value: One, order: Zero);
                return data;
            })
            .OrderByDescending(data => data.order)
            .ToDictionary(data => data.rect, data => data.value);
    }

    protected override float GetCrossSize(LayoutElement layout)
    {
        return layout.preferredHeight > 0
            ? layout.preferredWidth / layout.preferredHeight
            : One;
    }

    protected override float GetMainSize(int axis)
    {
        return ChildrenCrossSizes.Sum(x => Calculate(x.Value, rectTransform.rect.size[1 - axis]));
    }

    private void GetAnchorFactors(TextAnchor a, out float h, out float v)
    {
        switch (a)
        {
            case TextAnchor.UpperLeft:    h = 0f;   v = 0f;   break;
            case TextAnchor.UpperCenter:  h = 0.5f; v = 0f;   break;
            case TextAnchor.UpperRight:   h = 1f;   v = 0f;   break;
            case TextAnchor.MiddleLeft:   h = 0f;   v = 0.5f; break;
            case TextAnchor.MiddleCenter: h = 0.5f; v = 0.5f; break;
            case TextAnchor.MiddleRight:  h = 1f;   v = 0.5f; break;
            case TextAnchor.LowerLeft:    h = 0f;   v = 1f;   break;
            case TextAnchor.LowerCenter:  h = 0.5f; v = 1f;   break;
            case TextAnchor.LowerRight:   h = 1f;   v = 1f;   break;
            default:                      h = 0f;   v = 0f;   break;
        }
    }

    protected override void Place(float totalSpacing, float totalMainSize, float totalCrossSize)
    {
        int mainAxis = IsVertical ? 1 : 0;
        int crossAxis = 1 - mainAxis;

        float mainPadStart  = mainAxis == 0 ? padding.left : padding.top;
        float mainPadEnd    = mainAxis == 0 ? padding.right : padding.bottom;
        float crossPadStart = crossAxis == 0 ? padding.left : padding.top;
        float crossPadEnd   = crossAxis == 0 ? padding.right : padding.bottom;

        GetAnchorFactors(childAlignment, out float horiz, out float vert);
        float mainAlign  = IsVertical ? vert  : horiz;
        float crossAlign = IsVertical ? horiz : vert;

        float availableCross = Mathf.Max(0f, rectTransform.rect.size[crossAxis] - crossPadStart - crossPadEnd);

        float occupiedMain = ChildrenCrossSizes.Sum(x => Calculate(x.Value, availableCross)) + totalSpacing;
        float remainingMain = Mathf.Max(0f, totalMainSize - occupiedMain);
        float mainOffset = mainAlign * remainingMain;

        var items = ChildrenCrossSizes.ToList();
        int perLine = Mathf.Max(1, _separate);
        int idx = 0;

        float posMain = mainPadStart + mainOffset;

        while (idx < items.Count)
        {
            int countInLine = Mathf.Min(perLine, items.Count - idx);

            var line = new List<(RectTransform rect, float mainSize, float crossSize)>(countInLine);
            float occupiedCrossBySteps = 0f;

            for (int i = 0; i < countInLine; i++)
            {
                var pair = items[idx + i];
                var size = _oversizeFix(pair.Value, availableCross, totalSpacing, totalMainSize, totalCrossSize);
                float mainSize  = IsVertical ? size.y : size.x;
                float crossSize = IsVertical ? size.x : size.y;
                line.Add((pair.Key, mainSize, crossSize));
                occupiedCrossBySteps += mainSize;
            }
            if (countInLine > 1) occupiedCrossBySteps += Spacing * (countInLine - 1);

            float remainingCross = Mathf.Max(0f, availableCross - occupiedCrossBySteps);
            float crossOffset = crossAlign * remainingCross;
            float posCross = crossPadStart + crossOffset;

            for (int i = 0; i < line.Count; i++)
            {
                var (rect, mainSize, crossSize) = line[i];
                SetChildAlongAxis(rect, mainAxis, posMain, mainSize);
                SetChildAlongAxis(rect, crossAxis, posCross, crossSize);
                if (i < line.Count - 1) posCross += (mainSize + Spacing);
            }

            posMain += line.Count > 0 ? (line[0].mainSize + Spacing) : 0f;
            idx += countInLine;
        }
    }

    private Vector2 OversizeNone(float childAspect, float availableCross, float totalSpacing, float totalMainSize, float totalCrossSize)
    {
        float main = Calculate(childAspect, availableCross) / _separate;
        float cross = totalCrossSize / _separate;
        return new Vector2(IsVertical ? cross : main, IsVertical ? main : cross);
    }

    private Vector2 OversizeScaleMain(float childAspect, float availableCross, float totalSpacing, float totalMainSize, float totalCrossSize)
    {
        float baseMain = Calculate(childAspect, availableCross) / _separate;

        float sumMainPerLine = 0f;
        foreach (var kv in ChildrenCrossSizes)
            sumMainPerLine += Calculate(kv.Value, availableCross) / _separate;

        float baseTotalMain = sumMainPerLine + totalSpacing;
        float scale = baseTotalMain > 0f ? Mathf.Min(1f, (totalMainSize - totalSpacing) / Mathf.Max(SafeEpsilon, baseTotalMain - totalSpacing)) : 1f;
        scale = Mathf.Clamp01(scale);

        float main = baseMain * scale;
        float cross = totalCrossSize / _separate * scale;
        return new Vector2(IsVertical ? cross : main, IsVertical ? main : cross);
    }

    private Vector2 OversizeAdjustCrossEqually(float childAspect, float availableCross, float totalSpacing, float totalMainSize, float totalCrossSize)
    {
        int count = Mathf.Max(1, ChildrenCrossSizes.Count);
        float targetAspect;
        if (IsVertical)
        {
            float denom = Mathf.Max(SafeEpsilon, totalMainSize - totalSpacing);
            targetAspect = count * availableCross / denom;
        }
        else
        {
            float denom = Mathf.Max(SafeEpsilon, count * availableCross);
            targetAspect = (totalMainSize - totalSpacing) / denom;
        }
        targetAspect = Mathf.Max(SafeEpsilon, targetAspect);

        float main = Calculate(targetAspect, availableCross) / _separate;
        float cross = totalCrossSize / _separate;
        return new Vector2(IsVertical ? cross : main, IsVertical ? main : cross);
    }

    private void CalculateOversize()
    {
        _oversizeFix = _oversizeBehaviour switch
        {
            LayoutAspectBehaviour.None => OversizeNone,
            LayoutAspectBehaviour.ScaleMainToFit => OversizeScaleMain,
            LayoutAspectBehaviour.AdjustCrossEqually => OversizeAdjustCrossEqually,
            _ => OversizeNone
        };
    }
}