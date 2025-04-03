using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Root
{
    public class UIRootContainers : MonoBehaviour, IDependentScreenOrientation
    {
        [SerializeField] private AdaptiveLayoutGroup _left;
        [SerializeField] private AdaptiveLayoutGroup _right;
        [SerializeField] private AdaptiveLayoutGroup _top;
        [SerializeField] private AdaptiveLayoutGroup _bottom;

        private Dictionary<CrossPosition, AdaptiveLayoutGroup> _groups;
        private List<UIRootContainerItem> _items;
        private ScreenOrientation _orientation;

        public void Init()
        {
            _items= new List<UIRootContainerItem>();
            _groups = new Dictionary<CrossPosition, AdaptiveLayoutGroup>
            {
                { CrossPosition.Left, _left.Init() },
                { CrossPosition.Right, _right.Init() },
                { CrossPosition.Top, _top.Init() },
                { CrossPosition.Bottom, _bottom.Init() }
            };
        }

        public void UpdateScreenOrientation(ScreenOrientation orientation)
        {
            _orientation = orientation;

            _top.UpdateScreenOrientation(orientation);

            UpdateItemsPositions();
        }

        public void TakeNewItems(IEnumerable<UIRootContainerItem> items)
        {
            foreach(var item in _items)
                item.UnpackUp();

            _items.Clear();

            foreach (var item in items.OrderBy(i => i.OrderInGroup)) 
                _items.Add(item);

            UpdateItemsPositions();
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
