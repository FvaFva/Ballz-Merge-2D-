using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root
{
    public class UIRootContainers : MonoBehaviour, IDependentScreenOrientation
    {
        [SerializeField] private AdaptiveLayoutGroup _left;
        [SerializeField] private AdaptiveLayoutGroup _rightTop;
        [SerializeField] private AdaptiveLayoutGroup _rightCentre;
        [SerializeField] private AdaptiveLayoutGroup _top;
        [SerializeField] private AdaptiveLayoutGroup _bottomRight;
        [SerializeField] private AdaptiveLayoutGroup _bottomCentre;

        private Dictionary<CrossPosition, AdaptiveLayoutGroup> _groups;
        private List<UIRootContainerItem> _items;
        private ScreenOrientation _orientation;

        public void Init()
        {
            _items = new List<UIRootContainerItem>();
            _groups = new Dictionary<CrossPosition, AdaptiveLayoutGroup>
            {
                { CrossPosition.Left, _left.Init() },
                { CrossPosition.RightTop, _rightTop.Init() },
                { CrossPosition.RightCentre, _rightCentre.Init() },
                { CrossPosition.Top, _top.Init() },
                { CrossPosition.BottomCentre, _bottomCentre.Init() },
                { CrossPosition.BottomRight, _bottomRight.Init() }
            };
        }

        public void UpdateScreenOrientation(ScreenOrientation orientation)
        {
            _orientation = orientation;

            _top.UpdateScreenOrientation(orientation);
            _bottomRight.UpdateScreenOrientation(orientation);
            _rightCentre.UpdateScreenOrientation(orientation);
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

        public void PackItem(UIRootContainerItem item)
        {
            if (_items.Contains(item) == false)
                _items.Add(item);

            var newItemPosition = item.Positions[_orientation];
            var newItemGroup = _groups[newItemPosition];

            if (newItemGroup != item.Group)
            {
                item.UnpackUp();
                item.PackUp(newItemGroup);
            }
        }

        public void UnpackItem(UIRootContainerItem item)
        {
            if (_items.Contains(item))
            {
                item.UnpackUp();
                _items.Remove(item);
            }
        }

        private void UpdateItemsPositions()
        {
            foreach(var item in _items)
                PackItem(item);
        }
    }
}
