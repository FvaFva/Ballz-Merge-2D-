using System.Collections.Generic;

namespace BallzMerge.Data
{
    public struct GameHistoryData
    {
        public Dictionary<string, List<int>> Volumes;
        public int Score;
        public string ID;
        public string Date;
        public int Number;
        public int Level;
        public bool IsCompleted;

        public GameHistoryData(string id, int score, string date, int number, int level, bool isCompleted)
        {
            Score = score;
            Volumes = new Dictionary<string, List<int>>();
            ID = id;
            Date = date;
            Number = number;
            Level = level;
            IsCompleted = isCompleted;
        }

        public readonly string GetDateOrID(bool state) => state ? ID : Date;

        public readonly bool IsEmpty() => Volumes is null || Score == 0;
    }
}
