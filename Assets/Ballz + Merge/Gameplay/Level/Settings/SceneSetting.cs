using BallzMerge.Data;
using System;
using System.Collections.Generic;

public class SceneSetting : IDisposable
{
    public const string DynamicBoards = "Dynamic boards";

    private Dictionary<string, IGameSettingData> _data = new Dictionary<string, IGameSettingData>();

    public readonly GameColors Colors;

    public IEnumerable<IGameSettingData> GameSettings => _data.Values;

    public event Action Changed;

    public SceneSetting(GameColors gameColors)
    {
        GenerateSetting(DynamicBoards);
        Colors = gameColors;
        SubscribeSetting(Colors);
    }

    public void Dispose()
    {
        foreach (var setting in _data.Values)
            setting.Changed -= OnChanged;
    }

    private void GenerateSetting(string name)
    {
        var temp = new SceneSettingData(name);
        _data.Add(name, temp);
        SubscribeSetting(temp);
    }

    private void SubscribeSetting(IGameSettingData data)
    {
        data.Changed += OnChanged;
    }

    public float GetValue(string name)
    {
        if (_data.TryGetValue(name, out var setting))
            return setting.Value;

        return 0;
    }

    private void OnChanged() => Changed?.Invoke();
}
