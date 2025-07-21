using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BlocksSettings
{
    private const float MAX_COLOR = 1;

    [SerializeField] private AnimationCurve _additionalEffectChance;
    [SerializeField] private Gradient _numberGradient;
    [SerializeField] private List<WaveSpawnProperty> _spawnProperties;

    private Dictionary<int, Color> _colorMap;

    public List<WaveSpawnProperty> SpawnProperties { get { return _spawnProperties; } }

    public void RebuildColorMap()
    {
        _colorMap = new Dictionary<int, Color>();
        var numbers = _spawnProperties.SelectMany(p => p.Number.Select(n => n.Value)).Distinct().ToArray();
        var range = new Vector2Int(Mathf.Min(numbers), Mathf.Max(numbers));

        for (int i = 0; i < numbers.Count(); i++)
            _colorMap.Add(numbers[i], _numberGradient.Evaluate(Mathf.InverseLerp(range.x, range.y, numbers[i])));
    }

    public Color GetColor(int blockNumber)
    {
        if (_colorMap.ContainsKey(blockNumber))
            return _colorMap[blockNumber];
        else
            return _numberGradient.Evaluate(MAX_COLOR);
    }
}