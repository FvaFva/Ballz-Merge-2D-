using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveLayoutGroupAspect : AdaptiveLayoutGroupBase
{
    private const int MaxSeparating = 100;
    
    [SerializeField, Range(One, MaxSeparating)] private int _separate = 1;
    [SerializeField] private AlignmentWeight _alignment = AlignmentWeight.Start;

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

        float offsetCoefficient = _alignment switch
            {
                AlignmentWeight.Start => 0f,
                AlignmentWeight.Center => 0.5f,
                AlignmentWeight.End => 1f,
                _ => 0f
            };

        float availableCrossSize = rectTransform.rect.size[crossAxis];
        float totalOccupiedMainSize = ChildrenCrossSizes.Sum(x => Calculate(x.Value, availableCrossSize)) + totalSpacing;

        float remainingSpace = totalMainSize - totalOccupiedMainSize;
        float offset = offsetCoefficient * remainingSpace;

        float pos = (mainAxis == 0 ? padding.left : padding.top) + offset;
        float crossSize = totalCrossSize / _separate;
        int crossQueue = 0;
        float crossPos = Zero;

        foreach (var child in ChildrenCrossSizes)
        {
            float mainSize = Calculate(child.Value, availableCrossSize) / _separate;
            float step = mainSize + Spacing;

            SetChildAlongAxis(child.Key, mainAxis, pos, mainSize);
            SetChildAlongAxis(child.Key, crossAxis, crossPos, crossSize);

            if (++crossQueue == _separate)
            {
                crossQueue = 0;
                crossPos = Zero;
                pos += step;
            }
            else
            {
                crossPos += step;
            }
        }
    }
}
