using BallzMerge.Gameplay.Level;
using System;

[Serializable]
public struct MoveSettingsNumbersProperties
{
    public MoveSettingsRange Range;
    public int[] NumbersToSpawn;

    public bool IsEmpty()
    {
        return NumbersToSpawn == null || NumbersToSpawn.Length == 0;
    }
}