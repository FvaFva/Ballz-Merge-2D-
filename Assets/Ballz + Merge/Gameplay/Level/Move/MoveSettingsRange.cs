using System;

namespace BallzMerge.Gameplay.Level
{
    [Serializable]
    public struct MoveSettingsRange
    {
        public int InitialWave;
        public int TerminalWave;

        public bool IsInRange(int wave)
        {
            return InitialWave <= wave && wave <= TerminalWave;
        }
    }
}