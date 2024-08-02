using System;
using UnityEngine;

[Serializable]
public struct MoveSettingsCountBlocksPropertiesCountChance
{
    [Range(1, 4)] public int Count;
    [Range (0, 100)] public int Chance;
}