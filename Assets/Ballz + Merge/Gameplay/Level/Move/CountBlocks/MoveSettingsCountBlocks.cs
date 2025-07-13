using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New move numberSettings", menuName = "Bellz+Merge/Move/CountBlocks", order = 51)]
public class MoveSettingsCountBlocks : ScriptableObject
{
    [SerializeField] private List<WaveSpawnProperty> _properties;
    public List<WaveSpawnProperty> Properties { get { return _properties; } }

    public Vector2Int GetNumberRange()
    {
        return new Vector2Int(_properties.Min(p => p.Number.Min(n => n.Value)), _properties.Max(p => p.Number.Max(n => n.Value)));
    }
}