using System.Collections.Generic;

namespace BallzMerge.Data
{
    public struct GameHistoryData
    {
        public Dictionary<string, int> Volumes;
        public int Score;
        public string ID;
        public string Date;
        public int Number;

        public GameHistoryData(string id, int score, string date, int number)
        {
            Score = score;
            Volumes = new Dictionary<string, int>();
            ID = id;
            Date = date;
            Number = number;
        }

        public void Add(string volume, int value) => Volumes[volume] = value;

        public string GetDateOrID(bool state) => state ? ID : Date;
    }
}
