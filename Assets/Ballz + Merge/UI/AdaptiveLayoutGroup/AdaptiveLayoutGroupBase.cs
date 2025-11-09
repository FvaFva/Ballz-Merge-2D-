using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AdaptiveLayoutGroupBase : LayoutGroup
{
    protected const float One = 1f;
    protected const float Zero = 0f;

    [SerializeField] private float _spacing;
    [SerializeField] private bool _isInversive;
    [SerializeField] private bool _isVertical;

    private Dictionary<RectTransform, LayoutElement> _cachedLayout = new Dictionary<RectTransform, LayoutElement>();

    protected readonly Dictionary<RectTransform, float> ChildrenCrossSizes = new Dictionary<RectTransform, float>();
    protected bool IsVertical => _isVertical;
    protected float Spacing => _spacing;

    public bool IsInverse => _isInversive;
    public RectTransform Transform { get; private set; }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif

    public virtual AdaptiveLayoutGroupBase Init()
    {
        Transform = GetComponent<RectTransform>();
        return this;
    }
    
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        ChildrenCrossSizes.Clear();

        ChildrenCrossSizes.AddRange(CalculateChildren());

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

    public void UpdateScreenOrientation(bool isVertical) => _isVertical = isVertical ^ _isInversive;

    protected abstract float GetCrossSize(LayoutElement element);
    protected abstract float GetMainSize(int axis);
    protected abstract float Calculate(float a, float b);
    protected abstract IEnumerable<KeyValuePair<RectTransform, float>> CalculateChildren();
    protected abstract void Place(float totalSpacing, float totalMainSize, float totalCrossSize);

    protected bool IsHaveLayout(RectTransform child, out LayoutElement layout)
    {
        if (!_cachedLayout.ContainsKey(child))
            _cachedLayout.Add(child, child.GetComponent<LayoutElement>());

        layout = _cachedLayout[child];
        return layout != null;
    }

    protected float CalculateTotalPreferredSize(int axis)
    {
        int count = ChildrenCrossSizes.Count;

        if (count == 0)
            return 0;

        float totalSpacing = _spacing * (count - 1);
        float totalMainSize = GetMainSize(axis);

        float paddingSize = (axis == 0) ? padding.horizontal : padding.vertical;

        return totalMainSize + totalSpacing + paddingSize;
    }
    
    private void ApplyLayout()
    {
        int count = ChildrenCrossSizes.Count;

        if (count == 0)
            return;

        int mainAxis = _isVertical ? 1 : 0;
        int crossAxis = 1 - mainAxis;
        float totalCrossSize = rectTransform.rect.size[crossAxis];
        float totalMainSize = rectTransform.rect.size[mainAxis];
        float totalSpacing = _spacing * (count - One);

        Place(totalSpacing, totalMainSize, totalCrossSize);
    }
}
