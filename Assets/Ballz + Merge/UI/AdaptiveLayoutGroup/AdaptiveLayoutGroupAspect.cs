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
    private Vector2 _anchorFactor;

    protected override void OnValidate()
    {
        base.OnValidate();
        CalculateAttributes();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CalculateAttributes();
    }

    public override AdaptiveLayoutGroupBase Init()
    {
        base.Init();
        CalculateAttributes();
        return this;
    }

    public void SetOversizeBehavior(LayoutAspectBehaviour layoutAspectBehavior)
    {
        _oversizeBehaviour = layoutAspectBehavior;
        CalculateAttributes();
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

    protected override void Place(float totalSpacing, float totalMainSize, float totalCrossSize)
    {
        int mainAxis = IsVertical ? 1 : 0;
        int crossAxis = 1 - mainAxis;

        float mainPadStart  = mainAxis == 0 ? padding.left : padding.top;
        float mainPadEnd    = mainAxis == 0 ? padding.right : padding.bottom;
        float crossPadStart = crossAxis == 0 ? padding.left : padding.top;
        float crossPadEnd   = crossAxis == 0 ? padding.right : padding.bottom;

        float mainAlign  = IsVertical ? _anchorFactor.y  : _anchorFactor.x;
        float crossAlign = IsVertical ? _anchorFactor.x : _anchorFactor.y;

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

    private void CalculateAttributes()
    {
        _oversizeFix = _oversizeBehaviour switch
        {
            LayoutAspectBehaviour.None => OversizeNone,
            LayoutAspectBehaviour.ScaleMainToFit => OversizeScaleMain,
            LayoutAspectBehaviour.AdjustCrossEqually => OversizeAdjustCrossEqually,
            _ => OversizeNone
        };

        _anchorFactor = childAlignment switch
        {
            TextAnchor.UpperLeft => Vector2.zero,
            TextAnchor.UpperCenter => new Vector2(0.5f,0),   
            TextAnchor.UpperRight => new Vector2(1f,0),   
            TextAnchor.MiddleLeft => new Vector2(0,0.5f), 
            TextAnchor.MiddleCenter => new Vector2(0.5f,0.5f),   
            TextAnchor.MiddleRight => new Vector2(1f,0.5f),
            TextAnchor.LowerLeft => new Vector2(0,1),   
            TextAnchor.LowerCenter => new Vector2(0.5f,1),  
            TextAnchor.LowerRight => Vector2.one,   
            _ => Vector2.zero
        };
    }
}