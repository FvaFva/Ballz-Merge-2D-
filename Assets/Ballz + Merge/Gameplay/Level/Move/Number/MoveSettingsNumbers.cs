using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New move numberSettings", menuName = "Bellz+Merge/Move/Numbers", order = 51)]
public class MoveSettingsNumbers : ScriptableObject
{
    [SerializeField] private MoveSettingsNumbersProperties[] _properties;

    public IEnumerable<MoveSettingsNumbersProperties> Properties => _properties;
}