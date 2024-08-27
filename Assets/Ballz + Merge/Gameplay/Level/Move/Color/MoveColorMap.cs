using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [CreateAssetMenu(fileName = "New move color map", menuName = "Bellz+Merge/Move/Colors", order = 51)]
    public class MoveColorMap : ScriptableObject
    {
        [Header("Index + 1 = Number")]
        [SerializeField] Color[] _colors;

        public Color GetColor(int blockNumber)
        {
            if (--blockNumber < 0 || blockNumber >= _colors.Length)
                return Color.white;

            return _colors[blockNumber];
        }
    }
}