using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New move numberSettings", menuName = "Bellz+Merge/Move/CountBlocks", order = 51)]
public class MoveSettingsCountBlocks : ScriptableObject
{
    private const int FullChance = 100;

    [SerializeField] private MoveSettingsCountBlocksProperties[] _properties;

    public IEnumerable<MoveSettingsCountBlocksProperties> Properties => _properties;

    private void OnValidate()
    {
        foreach(var prop in _properties)
        {
            if (prop.CountBlocks.Length == 0)
                continue;

            int overhead = prop.CountBlocks.Sum(chance => chance.Chance) - FullChance;

            if (overhead != 0)
            {
                prop.CountBlocks[prop.CountBlocks.Length - 1].Chance -= overhead;
            }
        }
    }
}