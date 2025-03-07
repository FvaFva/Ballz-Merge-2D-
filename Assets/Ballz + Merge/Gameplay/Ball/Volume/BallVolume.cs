using BallzMerge.Gameplay.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New volume", menuName = "Bellz+Merge/Drop/BallVolume", order = 51)]
public class BallVolume : ScriptableObject
{
    private const string RarityWightKey = "{WEIGHT}";

    [SerializeField] private BallVolumesTypes _type;
    [SerializeField] private BallVolumesSpecies _species;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;

    [Header("Input volume description. You can use "+ RarityWightKey + " keyword for output rarity Weight")]
    [TextArea(2, 8)]
    [SerializeField] private string _description;

    [Header("If the behavior depends on rarity, you can additionally fill in here")]
    [SerializeField] private List<DropRaritySuffix> _suffixes;

    public BallVolumesTypes Type => _type;
    public BallVolumesSpecies Species => _species;
    public Sprite Icon => _icon;
    public string Name => _name;

    public string GetSuffix(DropRarity rarity) => _suffixes.Where(s => s.Rarity == rarity).FirstOrDefault().Suffix;

    public string GetDescription(DropRarity rarity) => _description.Replace(RarityWightKey, rarity.Weight.ToString());
}
