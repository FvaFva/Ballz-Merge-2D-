using BallzMerge.Gameplay.BallSpace;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.Level
{
    public class PhysicGrid : CyclicBehavior, ILevelStarter, ILevelFinisher
    {
        [SerializeField] private GridCell _prefab;
        [SerializeField] private Transform _cellParent;
        [SerializeField] private VirtualWorldFactory _factory;

        [Inject] private GridSettings _settings;

        private Dictionary<Vector2Int, GridCell> _grid = new Dictionary<Vector2Int, GridCell>();
        private Queue<GridCell> _cellsPool = new Queue<GridCell>();
        private BoxCollider2D[,] _boxes;

        public void StartLevel()
        {
            SpawnColumn(true);
        }

        public void FinishLevel()
        {
            foreach (GridCell cell in _grid.Values)
            {
                cell.Deactivate();
                _cellsPool.Enqueue(cell);
            }

            foreach (GridCell activeCells in GetActiveCells())
                ChangeCellActivity(activeCells, false);

            _grid.Clear();
        }

        public void SpawnColumn(bool isStart, int column = 1)
        {
            if (isStart)
                _boxes = _factory.CreateBoxes(_settings);
            else
                _boxes = _factory.CreateBoxes(_settings, column);

            for (int i = column - 1; i < _settings.GridSize.x; i++)
                for (int j = 0; j < _settings.GridSize.y; j++)
                    InitCell(i, j, _boxes[i, j]);
        }

        public void SpawnRow(int row)
        {
            _boxes = _factory.CreateBoxes(_settings, GridSizeY: row);

            for (int i = 0; i < _settings.GridSize.x; i++)
                for (int j = row - 1; j < _settings.GridSize.y; j++)
                    InitCell(i, j, _boxes[i, j]);
        }

        public void ChangeCellActivity(Vector2Int position, bool isActive)
        {
            if (_grid.ContainsKey(position) == false)
                return;

            ChangeCellActivity(_grid[position], isActive);
        }

        public void ChangeCellActivity(GridCell cell, bool isActive)
        {
            cell.ChangeActivity(isActive);
        }

        private void InitCell(int x, int y, BoxCollider2D virtualCollider)
        {
            Vector2Int gridPosition = new Vector2Int(x, y);
            GridCell cell = GetCell();
            _grid.Add(gridPosition, cell);
            cell.Activate(gridPosition, _settings.CellSize, virtualCollider);
        }

        private IEnumerable<GridCell> GetActiveCells()
        {
            return _grid.Values.Where(cell => cell.IsActive);
        }

        private GridCell GetCell()
        {
            if (_cellsPool.TryDequeue(out GridCell cell) == false)
                cell = GenerateCell();

            return cell;
        }

        private GridCell GenerateCell()
        {
            GridCell cell = Instantiate(_prefab, _cellParent);
            return cell;
        }
    }
}