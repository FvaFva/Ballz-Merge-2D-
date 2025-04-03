using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Root
{
    public class CamerasOperator : CyclicBehavior, IDependentScreenOrientation, IInitializable, ILevelStarter
    {
        private const float LandscapeSize = 12f;
        private const float PortraitSize = 22f;

        [SerializeField] private Camera _main;
        [SerializeField] private Camera _gameplay;
        [SerializeField] private Camera _effects;
        [SerializeField] private Camera _uI;

        private Dictionary<Camera, float> _sizeCoefficients = new Dictionary<Camera, float>();
        private float _currentSize;

        public Camera Main { get { return _main;} }
        public Camera Effects { get { return _effects; } }
        public Camera Gameplay {  get { return _gameplay; } }
        public Camera UI { get { return _uI; } }

        public void Init()
        {
            _currentSize = _currentSize == 0 ? LandscapeSize : _currentSize;
            
            if(_main !=  null)
                _sizeCoefficients.Add(_main, 0);

            if (_gameplay != null)
                _sizeCoefficients.Add(_gameplay, 0);

            if (_effects != null)
                _sizeCoefficients.Add(_effects, 0);

            if (_uI != null)
                _sizeCoefficients.Add(_uI, 0);

            UpdateCoefficients();
        }

        public void ChangeCoefficient(Camera camera, float coefficient)
        {
            _sizeCoefficients[camera] = coefficient;
            camera.orthographicSize = OrthographicSize(coefficient);
        }

        public void StartLevel()
        {
            foreach (var item in _sizeCoefficients.Keys.ToList())
            {
                _sizeCoefficients[item] = 0;
                item.orthographicSize = OrthographicSize(0);
            }
        }

        public void UpdateScreenOrientation(ScreenOrientation orientation)
        {
            _currentSize = orientation == ScreenOrientation.Portrait ? PortraitSize : LandscapeSize;
            UpdateCoefficients();
        }

        private void UpdateCoefficients()
        {
            foreach (var item in _sizeCoefficients)
                item.Key.orthographicSize = OrthographicSize(item.Value);
        }

        private float OrthographicSize(float coefficient) => _currentSize + (coefficient * _currentSize);
    }
}
