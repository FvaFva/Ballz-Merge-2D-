using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveLayoutGroupStretching : AdaptiveLayoutGroupBase
{
    private const float MinValue = 0.0001f;
    private float _totalChildrenCrossSize;

    protected override float Calculate(float weight, float totalCrossSize) => totalCrossSize * weight * _totalChildrenCrossSize;

    protected override float GetCrossSize(LayoutElement layout) => Mathf.Max(GetWeight(layout), MinValue);

    protected override float GetMainSize(int axis) => 0;

    protected override void Place(float totalSpacing, float totalMainSize, float totalCrossSize)
    {
        int mainAxis = IsVertical ? 1 : 0;
        int crossAxis = 1 - mainAxis;
        float totalPadding = IsVertical ? padding.vertical : padding.horizontal;
        float availableSize = totalMainSize - totalSpacing - totalPadding;
        float pos = (mainAxis == 0) ? padding.left : padding.top;

        foreach (var child in ChildrenCrossSizes)
        {
            float mainSize = Calculate(child.Value, availableSize);

            SetChildAlongAxis(child.Key, mainAxis, pos, mainSize);
            SetChildAlongAxis(child.Key, crossAxis, Zero, totalCrossSize);

            pos += mainSize + Spacing;
        }
    }

    private float GetWeight(LayoutElement layout)
    {
        return IsVertical ? layout.flexibleHeight : layout.flexibleWidth;
    }

    protected override IEnumerable<KeyValuePair<RectTransform, float>> CalculateChildren()
    {
        _totalChildrenCrossSize = 0;

        var result = rectChildren
            .Select(child =>
            {
                var data = IsHaveLayout(child, out var layout)
                ? (rect: child, value: GetCrossSize(layout), order: layout.layoutPriority)
                : (rect: child, value: One, order: Zero);

                _totalChildrenCrossSize += data.value;
                return data;
            })
            .OrderByDescending(data => data.order)
            .ToDictionary(data => data.rect, data => data.value);

        _totalChildrenCrossSize = _totalChildrenCrossSize == Zero ? Zero : One / _totalChildrenCrossSize;

        return result;
    }
}
