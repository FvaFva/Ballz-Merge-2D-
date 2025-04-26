using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay
{
    public class CamerasOperator : CyclicBehavior, IDependentScreenOrientation, IInitializable, ILevelStarter
    {
        private const float LandscapeSize = 12f;
        private const float PortraitSize = 20f;
        private const float LandscapeXDelta = 1.2f;
        private const float LandscapeYDelta = 0.7f;

        [SerializeField] private Vector2 _portraitAvailableSpacePrecent;
        [SerializeField] private Vector2 _landscapeAvailableSpacePrecent;
        [SerializeField] private Camera _main;
        [SerializeField] private Camera _gameplay;
        [SerializeField] private Camera _effects;
        [SerializeField] private Camera _uI;

        private Dictionary<Camera, PositionScaleProperty> _values;
        private Dictionary<Camera, Vector3> _startPositions;
        private float _currentSize;
        private Vector2 _currentAvailableSpacePrecent;
        private float _aspect;
        private Vector2 _boardSize = Vector2.one;
        private Vector3 _uIDelta;

        public Camera Effects { get { return _effects; } }
        public Camera Gameplay {  get { return _gameplay; } }
        public Camera UI { get { return _uI; } }

        public void Init()
        {
            _values = new Dictionary<Camera, PositionScaleProperty>();
            _startPositions = new Dictionary<Camera, Vector3>();
            _currentSize = _currentSize == 0 ? LandscapeSize : _currentSize;
            _aspect = _gameplay.aspect;

            TryAddCamera(_main);
            TryAddCamera(_gameplay);
            TryAddCamera(_effects);
            TryAddCamera(_uI);

            UpdateValues();
        }

        public void AddValue(Camera camera, float size = 0, Vector2 position = default)
        {
            _values[camera] = _values[camera].Add(size, position);
            camera.orthographicSize = OrthographicSize(_values[camera].UnionScale);
            camera.transform.position = Position(camera, _values[camera].Position);
        }

        public void StartLevel()
        {
            foreach (var item in _values.Keys.ToList())
            {
                _values[item] = new PositionScaleProperty(1);
                item.orthographicSize = OrthographicSize(1);
            }
        }

        public void UpdateScreenOrientation(ScreenOrientation orientation)
        {
            if (orientation == ScreenOrientation.Portrait)
            {
                _currentSize = PortraitSize;
                _currentAvailableSpacePrecent = _portraitAvailableSpacePrecent;
                _uIDelta = Vector2.zero;
                UpdateGameplaySize();
            }
            else
            {
                _uIDelta = new Vector2(LandscapeXDelta, LandscapeYDelta);
                _currentSize = LandscapeSize;
                _currentAvailableSpacePrecent = _landscapeAvailableSpacePrecent;
                UpdateGameplaySize();
            }

            UpdateValues();
        }

        public void SetGameplayBoardSize(Vector2 boardSize)
        {
            _boardSize = boardSize;
            UpdateGameplaySize();
        }

        private void UpdateValues()
        {
            foreach (var item in _values)
            {
                item.Key.orthographicSize = OrthographicSize(item.Value.UnionScale);
                item.Key.transform.position = Position(item.Key, item.Value.Position);
            }
        }

        private void TryAddCamera(Camera camera)
        {
            if (camera != null)
            {
                _values.Add(camera, new PositionScaleProperty(1));
                _startPositions.Add(camera, camera.transform.position);
            }
        }

        private float OrthographicSize(float coefficient) => coefficient * _currentSize;
        private Vector3 Position(Camera camera, Vector3 additional) => _startPositions[camera] + additional + _uIDelta;

        private void UpdateGameplaySize()
        {
            float height = _currentSize * 2f;
            float width = height * _aspect;
            var availableSpace = Vector2.Scale(new Vector2(width, height), _currentAvailableSpacePrecent);
            float maxDelta = Mathf.Max(_boardSize.x / availableSpace.x, _boardSize.y / availableSpace.y);
            _gameplay.orthographicSize = OrthographicSize(maxDelta);
            var current = _values[_gameplay];
            _values[_gameplay] = new PositionScaleProperty(maxDelta, current.Position);
        }
    }
}
