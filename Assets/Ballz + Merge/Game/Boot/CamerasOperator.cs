using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay
{
    public class CamerasOperator : CyclicBehavior, IDependentScreenOrientation, IInitializable
    {
        private const float LandscapeSize = 12f;
        private const float PortraitSize = 22f;

        [SerializeField] private Camera _main;
        [SerializeField] private Camera _gameplay;
        [SerializeField] private Camera _effects;
        [SerializeField] private Camera _uI;

        private Dictionary<Camera, (float, Vector2)> _values;
        private Dictionary<Camera, Vector3> _startPositions;
        private float _currentSize;

        public Camera Main { get { return _main;} }
        public Camera Effects { get { return _effects; } }
        public Camera Gameplay {  get { return _gameplay; } }
        public Camera UI { get { return _uI; } }

        public void Init()
        {
            _values = new Dictionary<Camera, (float, Vector2)>();
            _startPositions = new Dictionary<Camera, Vector3>();
            _currentSize = _currentSize == 0 ? LandscapeSize : _currentSize;

            TryAddCamera(_main);
            TryAddCamera(_gameplay);
            TryAddCamera(_effects);
            TryAddCamera(_uI);

            UpdateValues();
        }

        public void AddValue(Camera camera, float size = 0, Vector2 position = default)
        {
            var current = _values[camera];
            _values[camera] = (current.Item1 + size, current.Item2 + position);
            camera.orthographicSize = OrthographicSize(size);
            camera.transform.position = Position(camera, position);
        }

        public void SetDefault()
        {
            foreach (var item in _values.Keys.ToList())
            {
                _values[item] = (0, Vector2.zero);
                item.orthographicSize = OrthographicSize(0);
            }
        }

        public void UpdateScreenOrientation(ScreenOrientation orientation)
        {
            _currentSize = orientation == ScreenOrientation.Portrait ? PortraitSize : LandscapeSize;
            UpdateValues();
        }

        private void UpdateValues()
        {
            foreach (var item in _values)
            {
                item.Key.orthographicSize = OrthographicSize(item.Value.Item1);
                item.Key.transform.position = Position(item.Key, item.Value.Item2);
            }
        }

        private void TryAddCamera(Camera camera)
        {
            if (camera != null)
            {
                _values.Add(camera, (0, Vector2.zero));
                _startPositions.Add(camera, camera.transform.position);
            }
        }

        private float OrthographicSize(float coefficient) => _currentSize + (coefficient * _currentSize);
        private Vector3 Position(Camera camera, Vector3 additional) => _startPositions[camera] + additional;
    }
}
