using System;
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
        [SerializeField] private AdaptiveLayoutGroup _mid;

        private Dictionary<CrossPosition, AdaptiveLayoutGroup> _groups;
        private List<UIRootContainerItem> _items;
        private ScreenOrientation _orientation;
        private Dictionary<UIRootContainerItem, Action> _midItems;

        public void Init()
        {
            _midItems = new Dictionary<UIRootContainerItem, Action>();
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

            _mid.Init();
        }

        public void UpdateScreenOrientation(ScreenOrientation orientation)
        {
            _orientation = orientation;

            _top.UpdateScreenOrientation(orientation);
            _bottomRight.UpdateScreenOrientation(orientation);
            _rightCentre.UpdateScreenOrientation(orientation);
            _mid.UpdateScreenOrientation(orientation);

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

        public void ShowInMid(UIRootContainerItem item)
        {
            var groupSnapshot = item.Group;
            Action callback = groupSnapshot is null ? item.UnpackUp : () => item.PackUp(groupSnapshot);
            _midItems.Add(item, callback);
            item.PackUp(_mid);
        }

        public void HideMid()
        {
            foreach (var callback in _midItems.Values)
                callback();
            
            _midItems.Clear();
        }

        private void UpdateItemsPositions()
        {
            foreach(var item in _items)
            {
                var newItemPosition = item.Positions[_orientation];
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
