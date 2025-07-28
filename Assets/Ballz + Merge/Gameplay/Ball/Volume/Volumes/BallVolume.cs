using BallzMerge.Gameplay.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BallVolume : ScriptableObject
{
    private const string WeightKey = "{WEIGHT}";
    private const string SuffixKey = "{SUFFIX}";

    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [SerializeField] private ParticleSystem _effect;

    [Header("Input volume description. You can use " + WeightKey + " keyword for output weight or " + SuffixKey + " for output suffix")]
    [TextArea(2, 8)]
    [SerializeField] private string _description;

    [Header("If the behavior depends on rarity, you can additionally fill in here. Correct suffix will replace " + SuffixKey + " in description")]
    [SerializeField] private List<DropRaritySuffix> _suffixes;

    public Sprite Icon => _icon;
    public string Name => _name;
    public ParticleSystem Effect=> _effect;

    public abstract int GetValue(DropRarity rarity);
    public abstract string GetDescription(DropRarity rarity);

    public string GetDescription(int weight)
    {
        string result = _description.Replace(WeightKey, weight.ToString());
        string suffix = _suffixes.Where(s => s.Weight == weight).FirstOrDefault().Suffix;
        result = result.Replace(SuffixKey, suffix);
        return result;
    }
}
