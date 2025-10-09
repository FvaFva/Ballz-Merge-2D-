using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root
{
    public class UIRootContainers : MonoBehaviour, IDependentScreenOrientation
    {
        [SerializeField] private AdaptiveLayoutGroupBase _left;
        [SerializeField] private AdaptiveLayoutGroupBase _rightTop;
        [SerializeField] private AdaptiveLayoutGroupBase _rightCentre;
        [SerializeField] private AdaptiveLayoutGroupBase _top;
        [SerializeField] private AdaptiveLayoutGroupBase _bottomRight;
        [SerializeField] private AdaptiveLayoutGroupBase _bottomCentre;
        [SerializeField] private AdaptiveLayoutGroupAspect _rootRightTop;

        private Dictionary<CrossPosition, AdaptiveLayoutGroupBase> _groups;
        private List<UIRootContainerItem> _items;
        private bool _isVertical;

        public void Init()
        {
            _items = new List<UIRootContainerItem>();
            _groups = new Dictionary<CrossPosition, AdaptiveLayoutGroupBase>
            {
                { CrossPosition.Left, _left.Init() },
                { CrossPosition.RightTop, _rightTop.Init() },
                { CrossPosition.RightCentre, _rightCentre.Init() },
                { CrossPosition.Top, _top.Init() },
                { CrossPosition.BottomCentre, _bottomCentre.Init() },
                { CrossPosition.BottomRight, _bottomRight.Init() },
                { CrossPosition.RootRightTop, _rootRightTop.Init() }
            };
        }

        public void UpdateScreenOrientation(bool isVertical)
        {
            _isVertical = isVertical;

            _top.UpdateScreenOrientation(isVertical);
            _bottomRight.UpdateScreenOrientation(isVertical);
            _rightCentre.UpdateScreenOrientation(isVertical);
            UpdateItemsPositions();
        }

        public void TakeNewItems(IEnumerable<UIRootContainerItem> items)
        {
            foreach(var item in _items)
                item.UnpackUp();

            _items.Clear();

            foreach (var item in items) 
                _items.Add(item);

            UpdateItemsPositions();
        }

        public void SetSettings(LayoutAspectBehaviour layoutAspectBehavior)
        {
            _rootRightTop.SetOversizeBehavior(layoutAspectBehavior);
        }

        private void UpdateItemsPositions()
        {
            foreach (var item in _items)
            {
                var newItemPosition = item.Positions[_isVertical];
                var newItemGroup = _groups[newItemPosition];

                if (newItemGroup != item.Group)
                {
                    item.UnpackUp();
                    item.PackUp(newItemGroup);
                }
            }
        }
    }
}
