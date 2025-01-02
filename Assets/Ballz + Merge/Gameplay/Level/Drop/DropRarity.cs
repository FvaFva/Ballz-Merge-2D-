using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [CreateAssetMenu(fileName = "New rarity", menuName = "Bellz+Merge/Drop/DropRarity", order = 51)]
    public class DropRarity : ScriptableObject
    {
        [SerializeField] private Color _color;
        [SerializeField] private int _countInPool;
        [SerializeField] private int _weight;

        public Color Color => _color;
        public int CountInPool => _countInPool;
        public int Weight => _weight;
    }
}
