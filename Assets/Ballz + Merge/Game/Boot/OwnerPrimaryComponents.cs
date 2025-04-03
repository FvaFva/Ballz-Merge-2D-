using BallzMerge.Data;
using BallzMerge.Root.Settings;
using BallzMerge.ScreenOrientations;
using UnityEngine;

namespace BallzMerge.Root
{
    internal class OwnerPrimaryComponents
    {
        public OwnerPrimaryComponents()
        {
            Data = new DataBaseSource();
            TimeScaler = new TimeScaler();
            UserInput = new MainInputMap();
            UserInput.Enable();
            Hub = new ResourcesHub();
            Coroutines = AddDontDestroyOnLoad<Coroutines>("[COROUTINES]");
            OrientationObserver = AddDontDestroyOnLoad<ScreenOrientationObserver>("[ORIENTSTION_OBSERVER]");
        }

        public readonly Coroutines Coroutines;
        public readonly ResourcesHub Hub;
        public readonly MainInputMap UserInput;
        public readonly TimeScaler TimeScaler;
        public readonly DataBaseSource Data;
        public readonly ScreenOrientationObserver OrientationObserver;

        private T AddDontDestroyOnLoad<T>(string name)where T : Component
        {
            T result = new GameObject(name).AddComponent<T>();
            Object.DontDestroyOnLoad(result.gameObject);
            return result;
        }
    }
}
