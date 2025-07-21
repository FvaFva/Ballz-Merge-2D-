using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level settings map", menuName = "Bellz+Merge/Level/SettingsMap", order = 51)]
public class LevelSettingsMap : ScriptableObject
{
    [SerializeField] private List<LevelSettings> _available;

    public IEnumerable<LevelSettings> Available => _available;
}