using System;
using System.Collections.Generic;

namespace BallzMerge.Root
{
    public interface ISceneEnterPoint
    {
        public IEnumerable<IInitializable> InitializedComponents { get; }
        public IEnumerable<IDependentScreenOrientation> Orientators { get; }
        public void Init(Action<SceneExitData> callback, bool isLoad);
        public bool IsAvailable { get; }
    }
}
