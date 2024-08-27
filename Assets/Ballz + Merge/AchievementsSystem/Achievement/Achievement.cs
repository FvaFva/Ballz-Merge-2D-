using UnityEngine;

namespace BallzMerge.Achievement
{
    [CreateAssetMenu(fileName = "New Achievement", menuName = "Bellz+Merge/Achievements/Achievement", order = 51)]
    public class Achievement : ScriptableObject
    {
        [SerializeField] private string _googleId;

        public string GoogleId => _googleId;
    }
}
