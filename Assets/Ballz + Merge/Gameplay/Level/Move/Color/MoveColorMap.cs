using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [CreateAssetMenu(fileName = "New move color map", menuName = "Bellz+Merge/Move/Colors", order = 51)]
    public class MoveColorMap : ScriptableObject
    {
        [SerializeField] private Color _base;

        [Header("Index + 1 = Number")]
        [SerializeField] Color[] _colors;

        public Color Base => _base;

        public Color GetColor(int blockNumber)
        {
            if (--blockNumber < 0 || blockNumber >= _colors.Length)
                return Color.white;

            return _colors[blockNumber];
        }
    }
}