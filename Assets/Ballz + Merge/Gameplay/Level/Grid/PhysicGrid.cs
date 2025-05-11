using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.Level
{
    public class PhysicGrid : CyclicBehavior, ILevelFinisher
    {
        [SerializeField] private GridCell _prefab;
        [SerializeField] private Transform _cellParent;

        [Inject] private GridSettings _settings;

        private Dictionary<Vector2Int, GridCell> _grid = new Dictionary<Vector2Int, GridCell>();
        private Queue<GridCell> _cellsPool = new Queue<GridCell>();

        public void InitGrid()
        {
            for (int i = 0; i < _settings.Size.x; i++)
                for (int j = 0; j < _settings.Size.y; j++)
                    InitCell(i, j);
        }

        public void FinishLevel()
        {
            foreach (GridCell cell in _grid.Values)
            {
                cell.ChangeActivity(false);
                _cellsPool.Enqueue(cell);
            }

            _grid.Clear();
        }

        public void SpawnColumn()
        {
            _settings.AddSize(Vector2Int.right);

            for (int i = 0; i < _settings.Size.y; i++)
                InitCell(_settings.Size.x - 1, i);
        }

        public void SpawnRow()
        {
            _settings.AddSize(Vector2Int.up);

            for (int i = 0; i < _settings.Size.x; i++)
                InitCell(i, _settings.Size.y - 1);
        }

        private void InitCell(int x, int y)
        {
            Vector2Int gridPosition = new Vector2Int(x, y);
            GridCell cell = GetCell();
            _grid.Add(gridPosition, cell);
            cell.Init(gridPosition, _settings.CellSize);
        }

        private GridCell GetCell()
        {
            if (_cellsPool.TryDequeue(out GridCell cell) == false)
                cell = Instantiate(_prefab, _cellParent);

            return cell;
        }
    }
}