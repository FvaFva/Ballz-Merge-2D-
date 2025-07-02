using System;
using UnityEngine;

[Serializable]
public struct BlocksSpawnProperty
{
    public int Count;
    [Range (0, 1)] public float Chance;
}