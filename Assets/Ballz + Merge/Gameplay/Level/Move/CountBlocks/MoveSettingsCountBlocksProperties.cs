using BallzMerge.Gameplay.Level;
using System;

[Serializable]
public struct MoveSettingsCountBlocksProperties
{
    public MoveSettingsRange Range;
    public BlocksSpawnProperty[] BlocksProperties;

    public bool IsEmpty()
    {
        return BlocksProperties == null || BlocksProperties.Length == 0;
    }
}