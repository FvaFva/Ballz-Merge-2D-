using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [CreateAssetMenu(fileName = "New move color map", menuName = "Bellz+Merge/Move/Colors", order = 51)]
    public class MoveColorMap : ScriptableObject
    {
        [SerializeField] private Color _base;
        [SerializeField] private Gradient _color;
        [SerializeField] private MoveSettingsCountBlocks _settings;

        private Vector2Int _minMax;
        public Color Base => _base;

        void OnEnable()
        {
            _minMax = _settings.GetNumberRange();
        }
        
        public Color GetColor(int blockNumber)
        {
            var pos = Mathf.InverseLerp(_minMax.x, _minMax.y, blockNumber);
            return _color.Evaluate(pos);
        }
    }
}