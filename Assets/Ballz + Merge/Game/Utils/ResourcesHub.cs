using BallzMerge.Achievement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root
{
    public class ResourcesHub
    {
        private Dictionary<Type, UnityEngine.Object> _loadedResources;
        private Dictionary<Type, string> _resourceNames;

        private readonly string ROOT_UI = "UIRoot";
        private readonly string DEVELOPERS_SCENES = "DevelopersScenes";
        private readonly string GLOBAL_EFFECTS = "[Effects]";
        private readonly string ACHIEVEMENT = "AchievementsBus";

        public ResourcesHub()
        {
            _loadedResources = new Dictionary<Type, UnityEngine.Object>();

            _resourceNames = new Dictionary<Type, string>
            {
                { typeof(UIRootView), ROOT_UI },
                { typeof(DevelopersScenes), DEVELOPERS_SCENES },
                { typeof(GlobalEffects), GLOBAL_EFFECTS },
                { typeof(AchievementsBus), ACHIEVEMENT }
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
