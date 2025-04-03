using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Layout/Adaptive Layout Group")]
public class AdaptiveLayoutGroup : LayoutGroup
{
    [SerializeField] private float _spacing;
    [SerializeField] private bool _isInversive;
    [SerializeField] private bool _isUseAspectForMainAxis;
    [SerializeField] private bool _isVertical;

    private Dictionary<RectTransform, float> _childrenCrossSizes = new Dictionary<RectTransform, float>();
    private float _totalWeight;
    private float _calculatorCoefficient;

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
        _totalWeight = 0f;
        Func<LayoutElement, float> weightGetter = _isVertical ? (elem) => elem.flexibleHeight : (elem) => elem.flexibleWidth;

        if (_isUseAspectForMainAxis)
        {
            foreach (var child in rectChildren)
            {
                var layout = child.GetComponent<LayoutElement>();
                float aspect = 1f;
                if (layout != null && layout.preferredHeight > 0)
                    aspect = layout.preferredWidth / layout.preferredHeight;

                _childrenCrossSizes.Add(child, aspect);
            }
        }
        else
        {
            foreach (var child in rectChildren)
            {
                var layout = child.GetComponent<LayoutElement>();
                float weight = layout != null
                    ? Mathf.Max(weightGetter(layout), 0.0001f)
                    : 1f;

                _totalWeight += weight;
                _childrenCrossSizes.Add(child, weight);
            }
        }

        _totalWeight = _totalWeight == 0 ? 0 : 1 / _totalWeight;
    }

    public override void CalculateLayoutInputVertical(){}

    public override void SetLayoutHorizontal()
    {
        ApplyLayout();
    }

    public override void SetLayoutVertical()
    {
        ApplyLayout();
    }

    public void UpdateScreenOrientation(ScreenOrientation orientation)
    {
        _isVertical = orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight;
        _isVertical = _isVertical ^ _isInversive;
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
        Func<float, float> calculator;

        if (_isUseAspectForMainAxis)
        {
            _calculatorCoefficient = totalCrossSize;
            calculator = CalculateByAspect;
        }
        else
        {
            float totalMainSize = rectTransform.rect.size[mainAxis];
            float totalSpacing = _spacing * (count - 1);
            float totalPadding = _isVertical ? padding.vertical : padding.horizontal;
            float availableSize = totalMainSize - totalSpacing - totalPadding;
            _calculatorCoefficient = availableSize;
            calculator = CalculateByWeight;
        }

        foreach (var child in _childrenCrossSizes)
        {
            float mainSize = calculator(child.Value);

            SetChildAlongAxis(child.Key, mainAxis, pos, mainSize);
            SetChildAlongAxis(child.Key, crossAxis, 0, totalCrossSize);

            pos += mainSize + _spacing;
        }
    }

    private float CalculateByAspect(float aspect)
    {
        return _isVertical ? _calculatorCoefficient / aspect : _calculatorCoefficient * aspect;
    }

    private float CalculateByWeight(float weight)
    {
        return _calculatorCoefficient * weight * _totalWeight;
    }
}
