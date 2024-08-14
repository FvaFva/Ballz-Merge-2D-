using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New block additional effect settings", menuName = "Bellz+Merge/Block/AdditionalBlockEffectSettings", order = 51)]
public class BlockAdditionalEffectSettings : ScriptableObject
{
    [SerializeField] private BlockAdditionalEffectsSettingsProperty[] _properties;

    public IEnumerable<BlockAdditionalEffectsSettingsProperty> Properties => _properties;

    private void OnValidate()
    {
        float chanceToPerform = 0;

        foreach (var property in _properties)
        {
            chanceToPerform += property.ChanceToPerform;
        }

        if (chanceToPerform != 1)
        {
            
        }
    }

    public BlockAdditionalEffect GetPrefab()
    {
        return _properties[Random.Range(0, _properties.Length)].Prefab;
    }
}
