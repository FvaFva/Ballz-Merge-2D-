using System;
using UnityEngine;

[Serializable]
public struct BlockAdditionalEffectsSettingsProperty
{
    public BlockAdditionalEffect Prefab;
    [Range(0, 1)] public float ChanceToPerform;
}
