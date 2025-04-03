using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.Level
{
    public class PhysicGrid : CyclicBehavior, ILevelStarter, ILevelFinisher
    {
        [SerializeField] private GridCell _prefab;
        [SerializeField] private Transform _cellParent;

        [Inject] private GridSettings _settings;

        private Dictionary<Vector2Int, GridCell> _grid = new Dictionary<Vector2Int, GridCell>();
        private Queue<GridCell> _cellsPool = new Queue<GridCell>();

        public void StartLevel()
        {
            SpawnColumn(true);
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

        public void SpawnColumn(bool isStart, int column = 1)
        {
            for (int i = column - 1; i < _settings.Size.x; i++)
            {
                for (int j = 0; j < _settings.Size.y; j++)
                    InitCell(i, j);
            }
        }

        public void SpawnRow(int row)
        {
            for (int i = 0; i < _settings.Size.x; i++)
            {
                for (int j = row - 1; j < _settings.Size.y; j++)
                    InitCell(i, j);
            }
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
                cell = Instantiate(_prefab, _cellParent); ;

            return cell;
        }
    }
}