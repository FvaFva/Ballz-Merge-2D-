using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root
{
    public class UIRootContainerItem : MonoBehaviour
    {
        [SerializeField] private CrossPosition _positionVertical;
        [SerializeField] private CrossPosition _positionHorizontal;

        private RectTransform _transform;
        private RectTransform _parent;
        private bool _updatePositionOnEnable;
        private Coroutine _positionUpdater;
        private Dictionary<ScreenOrientation, CrossPosition> _positions;

        public IDictionary<ScreenOrientation, CrossPosition> Positions => _positions;
        public AdaptiveLayoutGroupBase Group { get; private set; }

        private void OnEnable()
        {
            if (_updatePositionOnEnable)
            {
                _updatePositionOnEnable = false;
                UpdatePositionByGroup();
            }
        }

        public void Init()
        {
            _transform = GetComponent<RectTransform>();
            _parent = (RectTransform)_transform.parent;
            _positions = new Dictionary<ScreenOrientation, CrossPosition>
            {
                { ScreenOrientation.LandscapeLeft, _positionHorizontal },
                { ScreenOrientation.LandscapeRight, _positionHorizontal },
                { ScreenOrientation.Portrait, _positionVertical },
                { ScreenOrientation.PortraitUpsideDown, _positionVertical }
            };
        }

        public void PuckUp(RectTransform transform)
        {
            _transform?.SetParent(transform);
            _transform.position = _transform.position.DropZ();
        }

        public void PackUp(AdaptiveLayoutGroupBase root)
        {
            Group = root;

            if (isActiveAndEnabled)
                UpdatePositionByGroup();
            else
                _updatePositionOnEnable = true;
        }

        public void UnpackUp()
        {
            Group = null;
            _transform.PerformIfNotNull(rectTransform => rectTransform.SetParent(_parent));
        }

        public void UpdatePositionByGroup()
        {
            if (_positionUpdater != null)
                StopCoroutine(_positionUpdater);
            
            _positionUpdater = StartCoroutine(DelayedUpdatePositionByGroup());
        }

        private IEnumerator DelayedUpdatePositionByGroup()
        {
            yield return new WaitForEndOfFrame();
            _transform.SetParent(Group.Transform);
            _transform.position = _transform.position.DropZ();
            _positionUpdater = null;
        }
    }
}
