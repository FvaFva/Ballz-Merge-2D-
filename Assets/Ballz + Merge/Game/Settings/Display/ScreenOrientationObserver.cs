using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.ScreenOrientations
{
    public class ScreenOrientationObserver : MonoBehaviour
    {
        private List<IDependentScreenOrientation> _elements = new List<IDependentScreenOrientation>();

        private ScreenOrientation _last;

        private Func<ScreenOrientation> _orientation;

        private void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            _orientation = Mobile;
#else
            _orientation = StandalonePC;
#endif
            _last = _orientation();
        }

        private void Update()
        {
            if(_orientation() != _last)
            {
                _last = _orientation();
                UpdateElements();
            }
        }

        public void CheckOutAll()
        {
            _elements.Clear();
        }

        public void CheckIn(IDependentScreenOrientation element)
        {
            if (_elements.Contains(element))
                return;

            _elements.Add(element);
            element.UpdateScreenOrientation(_last);
        }

        private ScreenOrientation StandalonePC()
        {
            return  Screen.width > Screen.height ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
        }

        private ScreenOrientation Mobile()
        {
            return Screen.orientation;
        }

        private void UpdateElements()
        {
            foreach (var element in _elements)
                element.UpdateScreenOrientation(_last);
        }
    }
}