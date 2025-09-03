using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level settings map", menuName = "Bellz+Merge/Level/SettingsMap", order = 51)]
public class LevelSettingsMap : ScriptableObject
{
    [SerializeField] private List<LevelSettings> _all;
    [SerializeField] private List<LevelSettings> _available;
    [SerializeField] private int _settingsIDCounter;

    public IReadOnlyList<LevelSettings> Available => _available;

    public int GetID() => _settingsIDCounter++;

    public LevelSettings GetSetting(int id) => _all.Where(s => s.ID == id).FirstOrDefault();
}