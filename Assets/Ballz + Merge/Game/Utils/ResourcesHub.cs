using BallzMerge.Achievement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace BallzMerge.Root
{
    public class ResourcesHub
    {
        private const string ROOT_UI = "UIRoot";
        private const string DEVELOPERS_SCENES = "DevelopersScenes";
        private const string GLOBAL_EFFECTS = "[Effects]";
        private const string ACHIEVEMENT = "AchievementsBus";
        private const string AUDIO = "AudioMixer";

        private Dictionary<Type, UnityEngine.Object> _loadedResources;
        private Dictionary<Type, string> _resourceNames;

        public ResourcesHub()
        {
            _loadedResources = new Dictionary<Type, UnityEngine.Object>();

            _resourceNames = new Dictionary<Type, string>
            {
                { typeof(UIRootView), ROOT_UI },
                { typeof(DevelopersScenes), DEVELOPERS_SCENES },
                { typeof(GlobalEffects), GLOBAL_EFFECTS },
                { typeof(AchievementsBus), ACHIEVEMENT },
                { typeof(AudioMixer), AUDIO }
            };
        }

        public T Get<T>() where T : UnityEngine.Object
        {
            var type = typeof(T);

            if (_loadedResources.ContainsKey(type) && _loadedResources[typeof(T)] is T cashedValue)
                return cashedValue;
            else
                return Resources.Load<T>(_resourceNames[type]);
        }
    }
}
