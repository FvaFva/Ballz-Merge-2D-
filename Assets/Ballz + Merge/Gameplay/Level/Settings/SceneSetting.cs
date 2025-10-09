using BallzMerge.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetting : IDisposable
{
    private Dictionary<string, SceneSettingData> _data = new Dictionary<string, SceneSettingData>();

    public const string DynamicBoards = "Dynamic boards";

    public IEnumerable<IGameSettingData> GameSettings => _data.Values;

    public event Action Changed;

    public SceneSetting()
    {
        GenerateSetting(DynamicBoards);
    }

    public void Dispose()
    {
        foreach (var setting in _data.Values)
            setting.StateChanged -= OnChanged;
    }

    private void GenerateSetting(string name)
    {
        var temp = new SceneSettingData(name);
        _data.Add(name, temp);
        temp.StateChanged += OnChanged;
    }

    public float GetValue(string name)
    {
        if (_data.TryGetValue(name, out var setting))
            return setting.Value;

        return 0;
    }

    private void OnChanged(bool _) => Changed?.Invoke();
}
