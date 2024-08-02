using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New grid settings", menuName = "Bellz+Merge/Grid/Settings", order = 51)]
public class GridSettings : ScriptableObject
{
    [SerializeField] private float _cellSize;
    [SerializeField] private float _cellSpacing;
    [SerializeField] private Vector2Int _gridSize;

    public float CellSize => _cellSize;
    public Vector2Int GridSize => _gridSize;
    public float CellSpacing => _cellSpacing;
    public int FirstRowIndex => _gridSize.y - 1;
    public int LastRowIndex => 0;

    public List<int> GetPositionsInRow()
    {
        List<int> positions = new List<int>();

        for(int i = 0 ; i < _gridSize.x; i++)
            positions.Add(i);

        return positions;
    }
}
