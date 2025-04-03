using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root
{
    [CreateAssetMenu(fileName = "DevelopersScenes", menuName = "Bellz+Merge/Development/DevelopersScenes", order = 51)]
    public class DevelopersScenes : ScriptableObject
    {
        [SerializeField] private List<string> _names;

        public IList<string> Scenes => _names;
    }
}
