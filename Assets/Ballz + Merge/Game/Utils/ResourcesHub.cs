using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root
{
    public class ResourcesHub
    {
        private Dictionary<(string, Type), UnityEngine.Object> _loadedResources;

        public ResourcesHub()
        {
            _loadedResources = new Dictionary<(string, Type), UnityEngine.Object>();
        }

        public readonly string ROOT_UI = "UIRoot";
        public readonly string DEVELOPERS_SCENES = "DevelopersScenes";
        public readonly string GLOBAL_EFFECTS = "[Effects]";

        public T Get<T>(string name) where T : UnityEngine.Object
        {
            var key = (name, typeof(T));

            if (_loadedResources.ContainsKey(key) && _loadedResources[key] is T cashedValue)
                return cashedValue;
            else
                return Load<T>(name);
        }

        private T Load<T>(string name) where T : UnityEngine.Object
        {
            return Resources.Load<T>(name);
        }
    }
}
