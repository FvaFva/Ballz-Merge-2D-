using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallVolumeCarrierOrientationAdapter : MonoBehaviour, IDependentScreenOrientation
{
    private const int VERTICAL_SEPARATE_CAGE = 2;
    private const int HORIZONTAL_SEPARATE_CAGE = 1;
    private const int VERTICAL_SEPARATE_INVENTORY = 2;
    private const int HORIZONTAL_SEPARATE_INVENTORY = 3;
    private const float HEADER_FLEX_VERTICAL = 3;
    private const float HEADER_FLEX_HORIZONTAL = 1;

    [SerializeField] private AdaptiveLayoutGroupBase _body;
    [SerializeField] private AdaptiveLayoutGroupAspect _inventory;
    [SerializeField] private RectTransform _currentView;
    [SerializeField] private RectTransform _header;
    [SerializeField] private LayoutElement _headerLayout;
    [SerializeField] private RectTransform _volumes;
    [SerializeField] private List<LayoutElement> _verticalOrder;
    [SerializeField] private List<LayoutElement> _horizontalOrder;

    public RectTransform CagePosition { get; private set; }
    public int CageSeparate { get; private set; }

    public void UpdateScreenOrientation(bool isVertical)
    {
        _body.UpdateScreenOrientation(isVertical);

        if (isVertical)
            RebuildToVertical();
        else
            RebuildToHorizontal();
    }

    private void RebuildToVertical()
    {
        CageSeparate = VERTICAL_SEPARATE_CAGE;
        CagePosition = _volumes;
        _currentView.SetParent(_header);
        _headerLayout.flexibleHeight = HEADER_FLEX_VERTICAL;
        _inventory.SetProperty(VERTICAL_SEPARATE_INVENTORY);
        UpdateOrder(_verticalOrder);
    }

    private void RebuildToHorizontal()
    {
        CageSeparate = HORIZONTAL_SEPARATE_CAGE;
        CagePosition = _header;
        _currentView.SetParent(_body.transform);
        _headerLayout.flexibleHeight = HEADER_FLEX_HORIZONTAL;
        _inventory.SetProperty(HORIZONTAL_SEPARATE_INVENTORY);
        UpdateOrder(_horizontalOrder);
    }

    private void UpdateOrder(List<LayoutElement> order)
    {
        for (int i = 0; i < order.Count; i++)
            order[i].layoutPriority = i;
    }
}