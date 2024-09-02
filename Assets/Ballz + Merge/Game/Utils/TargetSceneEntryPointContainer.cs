using UnityEngine;

namespace BallzMerge.Root
{
    public class TargetSceneEntryPointContainer
    {
        public ISceneEnterPoint Current { get ; private set; }
        private bool _isReady;

        public void Clear()
        {
            Current = null;
            _isReady = false;
        }

        public void Set(ISceneEnterPoint newValue)
        {
            if (_isReady)
                Debug.LogError("WARNING! Error! DEEP WRONG PENETRATION IN SCENE");

            _isReady = true;
            Current = newValue;
        }

        public bool IsReady() => _isReady && Current != null && Current.IsAvailable;
    }
}
