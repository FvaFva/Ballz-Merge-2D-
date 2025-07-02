using BallzMerge.Gameplay.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New volume", menuName = "Bellz+Merge/Drop/BallVolume", order = 51)]
public class BallVolume : ScriptableObject
{
    private const string WeightKey = "{WEIGHT}";
    private const string SuffixKey = "{SUFFIX}";
    private const int DefaultWeight = 1;

    [Header("If your gonna try take description by rarity without this property it return by 1")]
    [SerializeField] private bool _isUseRarityWeight;
    [SerializeField] private BallVolumesTypes _type;
    [SerializeField] private BallVolumesSpecies _species;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;

    [Header("Input volume description. You can use " + WeightKey + " keyword for output weight or " + SuffixKey + " for output suffix")]
    [TextArea(2, 8)]
    [SerializeField] private string _description;

    [Header("If the behavior depends on rarity, you can additionally fill in here. Correct suffix will replace " + SuffixKey + " in description")]
    [SerializeField] private List<DropRaritySuffix> _suffixes;

    public BallVolumesTypes Type => _type;
    public BallVolumesSpecies Species => _species;
    public Sprite Icon => _icon;
    public string Name => _name;

    public string GetDescription(DropRarity rarity) => GetDescription(_isUseRarityWeight ? rarity.Weight : DefaultWeight);

    public string GetDescription(int weight)
    {
        string result = _description.Replace(WeightKey, weight.ToString());
        string suffix = _suffixes.Where(s => s.Weight == weight).FirstOrDefault().Suffix;
        result = result.Replace(SuffixKey, suffix);
        return result;
    }
}
