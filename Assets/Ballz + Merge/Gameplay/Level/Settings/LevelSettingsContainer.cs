using System;
using UnityEngine;

[Serializable]
public class LevelSettingsContainer
{
    [SerializeField] private LevelSettings _default;

    private LevelSettings _current;

    public LevelSettings Get()
    {
        if (_current is null)
            return _default;
        else
            return _current;
    }

    public void Change(LevelSettings settings) => _current = settings;
}