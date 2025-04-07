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
        private Dictionary<ScreenOrientation, CrossPosition> _positions;
        private bool _updatePositionOnEnable;

        public IDictionary<ScreenOrientation, CrossPosition> Positions => _positions;
        public AdaptiveLayoutGroup Group { get; private set; }

        private void OnEnable()
        {
            if (_updatePositionOnEnable)
            {
                _updatePositionOnEnable = false;
                StartCoroutine(UpdatePosition());
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

        public void PackUp(AdaptiveLayoutGroup root)
        {
            Group = root;

            if (isActiveAndEnabled)
                StartCoroutine(UpdatePosition());
            else
                _updatePositionOnEnable = true;
        }

        public void UnpackUp()
        {
            Group = null;
            _transform.SetParent(_parent);
        }

        private IEnumerator UpdatePosition()
        {
            yield return new WaitForEndOfFrame();
            _transform.SetParent(Group.Transform);
            _transform.position = new Vector3(_transform.position.x, _transform.position.y, 0);
        }
    }
}
