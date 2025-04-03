using BallzMerge.Gameplay.Level;
using System;

[Serializable]
public struct MoveSettingsCountBlocksProperties
{
    public MoveSettingsRange Range;
    public MoveSettingsCountBlocksPropertiesCountChance[] CountBlocks;

    public bool IsEmpty()
    {
        return CountBlocks == null || CountBlocks.Length == 0;
    }
}