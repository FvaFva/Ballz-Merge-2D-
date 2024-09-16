using BallzMerge.Gameplay.BallSpace;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.Level
{
    public class PhysicGrid : CyclicBehavior, ILevelFinisher, ILevelStarter
    {
        [SerializeField] private GridCell _prefab;
        [SerializeField] private Transform _cellParent;
        [SerializeField] private VirtualWorldFactory _factory;

        [Inject] private GridSettings _settings;

        private Dictionary<Vector2Int, GridCell> _grid = new Dictionary<Vector2Int, GridCell>();
        private Queue<GridCell> _cells = new Queue<GridCell>();

        public void StartLevel()
        {
            SpawnColumn();
        }

        public void SpawnColumn(int column = 1)
        {
            BoxCollider2D[,] boxes = _factory.CreateBoxes(_settings);

            for (int i = column - 1; i < _settings.GridSize.x; i++)
                for (int j = 0; j < _settings.GridSize.y; j++)
                    InitCell(i, j, boxes[i, j]);
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

        public void FinishLevel()
        {
            _grid.Clear();

            foreach (GridCell activeCells in GetActiveCells())
                ChangeCellActivity(activeCells, false);
        }

        private void InitCell(int x, int y, BoxCollider2D virtualCollider)
        {
            GridCell cell;

            if (_cells.Count >= _settings.GridSize.x * _settings.GridSize.y)
                cell = _cells.Dequeue();
            else
                cell = GenerateCell(x, y);

            Vector2Int gridPosition = new Vector2Int(x, y);
            cell.Init(gridPosition, _settings.CellSize, virtualCollider);
            _grid.Add(gridPosition, cell);
        }

        private IEnumerable<GridCell> GetActiveCells()
        {
            return _grid.Values.Where(cell => cell.IsActive);
        }

        private GridCell GenerateCell(int x, int y)
        {
            GridCell cell = Instantiate(_prefab, _cellParent);
            cell.name = $"[{x}] - [{y}]";
            _cells.Enqueue(cell);
            return cell;
        }
    }
}