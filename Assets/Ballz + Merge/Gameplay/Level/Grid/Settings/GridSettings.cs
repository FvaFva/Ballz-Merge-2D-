using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [CreateAssetMenu(fileName = "New grid settings", menuName = "Bellz+Merge/Grid/Settings", order = 51)]
    public class GridSettings : ScriptableObject
    {
        private const int StartPosition = 0;

        [SerializeField] private float _cellSize;
        [SerializeField] private float _cellSpacing;
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private Vector2 _viewPosition;
        [SerializeField] private float _moveTime = 0.3f;

        private Vector2Int _extraSize;

        public float CellSize => _cellSize;
        public float CellSpacing => _cellSpacing;
        public Vector2Int Size => _gridSize + _extraSize;
        public Vector2 ViewPosition => _viewPosition;
        public int FirstRowIndex => Size.y - 1;
        public int LastRowIndex => 0;
        public float MoveTime => _moveTime;

        public void AddSize(Vector2Int additionalSize)
        {
            _extraSize += additionalSize;
        }

        public List<int> GetPositionsInRow()
        {
            List<int> positions = new List<int>();

            for (int i = 0; i < _gridSize.x + _extraSize.x; i++)
                positions.Add(i);

            return positions;
        }

        public bool IsOutside(Vector2Int point) => point.x < StartPosition || point.x >= Size.x || point.y > FirstRowIndex;

        public void ReloadSize()
        {
            _extraSize = Vector2Int.zero;
        }
    }
}