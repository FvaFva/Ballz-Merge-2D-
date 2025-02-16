using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root
{
    public interface ISceneEnterPoint
    {
        public IEnumerable<IInitializable> InitializedComponents { get; }
        public void Init(Action<SceneExitData> callback);
        public bool IsAvailable { get; }
    }
}
