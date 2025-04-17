using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Layout/Adaptive Layout Group")]
public class AdaptiveLayoutGroup : LayoutGroup
{
    private const float One = 1f;
    private const float MinValue = 0.0001f;
    private const float Zero = 0f;

    [SerializeField] private float _spacing;
    [SerializeField] private bool _isInversive;
    [SerializeField] private bool _isUseAspectForMainAxis;
    [SerializeField] private bool _isVertical;
    [SerializeField] private AlignmentWeight _alignment = AlignmentWeight.Start;

    private Dictionary<RectTransform, float> _childrenCrossSizes = new Dictionary<RectTransform, float>();
    private Dictionary<RectTransform, LayoutElement> _mapLayout = new Dictionary<RectTransform, LayoutElement>();
    private Func<LayoutElement, float> _weightGetter;
    private float _totalChildrenCrossSize;

    public RectTransform Transform { get; private set; }

    public AdaptiveLayoutGroup Init()
    {
        Transform = GetComponent<RectTransform>();
        return this;
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        _childrenCrossSizes.Clear();
        _totalChildrenCrossSize = Zero;
        _weightGetter = _isVertical ? (elem) => elem.flexibleHeight : (elem) => elem.flexibleWidth;
        Func<LayoutElement, float> crossSizeGetter = _isUseAspectForMainAxis ? GetChildAspect : GetChildWeight;

        _childrenCrossSizes = rectChildren
            .Select(child =>
            {
                var data = IsHaveLayout(child, out var layout)
                ? (rect: child, value: crossSizeGetter(layout), order: layout.layoutPriority)
                : (rect: child, value: One, order: Zero);

                _totalChildrenCrossSize += data.value;
                return data;
            })
            .OrderByDescending(data => data.order)
            .ToDictionary(data => data.rect, data => data.value);

        if (!_isUseAspectForMainAxis)
            _totalChildrenCrossSize = _totalChildrenCrossSize == Zero ? Zero : One / _totalChildrenCrossSize;


        float preferredSize = _isVertical
            ? CalculateTotalPreferredSize(1)
            : CalculateTotalPreferredSize(0);

        SetLayoutInputForAxis(preferredSize, preferredSize, -1, 0);
    }

    public override void CalculateLayoutInputVertical()
    {
        float preferredSize = _isVertical
        ? CalculateTotalPreferredSize(1)
        : CalculateTotalPreferredSize(0);

        SetLayoutInputForAxis(preferredSize, preferredSize, -1, 1);
    }

    public override void SetLayoutHorizontal() => ApplyLayout();

    public override void SetLayoutVertical() => ApplyLayout();

    public void UpdateScreenOrientation(ScreenOrientation orientation)
    {
        _isVertical = orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown;
        _isVertical = _isVertical ^ _isInversive;
    }

    private float CalculateTotalPreferredSize(int axis)
    {
        int count = _childrenCrossSizes.Count;
        if (count == 0)
            return 0;

        float totalSpacing = _spacing * (count - 1);
        float totalMainSize = _isUseAspectForMainAxis
            ? _childrenCrossSizes.Sum(x => CalculateByAspect(x.Value, rectTransform.rect.size[1 - axis]))
            : 0f;

        float paddingSize = (axis == 0) ? padding.horizontal : padding.vertical;

        return totalMainSize + totalSpacing + paddingSize;
    }

    private void ApplyLayout()
    {
        int count = _childrenCrossSizes.Count;

        if (count == 0)
            return;

        int mainAxis = _isVertical ? 1 : 0;
        int crossAxis = 1 - mainAxis;

        float totalCrossSize = rectTransform.rect.size[crossAxis];
        float pos = (mainAxis == 0) ? padding.left : padding.top;

        Func<float, float, float> calculator;
        float calculatorCoefficient;

        float totalMainSize = rectTransform.rect.size[mainAxis];
        float totalSpacing = _spacing * (count - One);

        if (_isUseAspectForMainAxis)
        {
            float offsetCoefficient = _alignment switch
            {
                AlignmentWeight.Start => 0f,
                AlignmentWeight.Center => 0.5f,
                AlignmentWeight.End => 1f,
                _ => 0f
            };

            float availableCrossSize = rectTransform.rect.size[crossAxis];
            calculatorCoefficient = availableCrossSize;
            calculator = CalculateByAspect;
            float totalOccupiedMainSize = _childrenCrossSizes.Sum(x => CalculateByAspect(x.Value, availableCrossSize)) + totalSpacing;

            float remainingSpace = totalMainSize - totalOccupiedMainSize;
            float offset = offsetCoefficient * remainingSpace;

            pos = (mainAxis == 0 ? padding.left : padding.top) + offset;
        }
        else
        {
            float totalPadding = _isVertical ? padding.vertical : padding.horizontal;
            float availableSize = totalMainSize - totalSpacing - totalPadding;

            calculatorCoefficient = availableSize;
            calculator = CalculateByWeight;
        }

        foreach (var child in _childrenCrossSizes)
        {
            float mainSize = calculator(child.Value, calculatorCoefficient);

            SetChildAlongAxis(child.Key, mainAxis, pos, mainSize);
            SetChildAlongAxis(child.Key, crossAxis, Zero, totalCrossSize);

            pos += mainSize + _spacing;
        }
    }

    private float CalculateByAspect(float aspect, float availableSize) => _isVertical ? availableSize / aspect : availableSize * aspect;

    private float CalculateByWeight(float weight, float totalCrossSize) => totalCrossSize * weight * _totalChildrenCrossSize;

    private float GetChildAspect(LayoutElement layout)
    {
        return layout.preferredHeight > 0
            ? layout.preferredWidth / layout.preferredHeight
            : One;
    }

    private float GetChildWeight(LayoutElement layout) => Mathf.Max(_weightGetter(layout), MinValue);

    private bool IsHaveLayout(RectTransform child, out LayoutElement layout)
    {
        if (!_mapLayout.ContainsKey(child))
            _mapLayout.Add(child, child.GetComponent<LayoutElement>());

        layout = _mapLayout[child];
        return layout != null;
    }
}
