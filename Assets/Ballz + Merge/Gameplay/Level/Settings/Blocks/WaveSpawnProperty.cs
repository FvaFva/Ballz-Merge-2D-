using System;
using System.Collections.Generic;

[Serializable]
public struct WaveSpawnProperty
{
    public List<BlocksSpawnProperty> Count;
    public List<BlocksSpawnProperty> Number;

    public bool IsEmpty()
    {
        return Count == null || Number == null || Count.Count == 0 || Number.Count == 0;
    }
}