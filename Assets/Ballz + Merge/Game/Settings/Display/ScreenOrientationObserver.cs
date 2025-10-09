using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.ScreenOrientations
{
    public class ScreenOrientationObserver : MonoBehaviour
    {
        private List<IDependentScreenOrientation> _sceneElements = new List<IDependentScreenOrientation>();
        private List<IDependentScreenOrientation> _rootElements = new List<IDependentScreenOrientation>();

        private bool _last;

        private Func<bool> _orientation;

        private void Awake()
        {
            PlatformRunner.RunOnDesktopPlatform(
                desktopAction: () =>
                {
                    _orientation = StandalonePC;
                },
                nonDesktopAction: () =>
                {
                    _orientation = Mobile;
                });

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

        public void CheckInRoot(IDependentScreenOrientation element) => CheckInElement(element, _rootElements);

        public void CheckInSceneElements(IDependentScreenOrientation element) => CheckInElement(element, _sceneElements);

        public void CheckOutScene()
        {
            _sceneElements.Clear();
        }

        private void CheckInElement(IDependentScreenOrientation element, List<IDependentScreenOrientation> elements)
        {
            if (elements.Contains(element))
                return;

            elements.Add(element);
            element.UpdateScreenOrientation(_last);
        }

        private void UpdateElements()
        {
            foreach (var element in _sceneElements.Concat(_rootElements))
                element.UpdateScreenOrientation(_last);
        }
    }
}