using UnityEngine;

namespace BallzMerge.Data
{
    public class DataBaseSource
    {
        private string _dbPath;

        public DataBaseSource()
        {
            _dbPath = "URI=file:" + Application.persistentDataPath + "/game_data.db";
            Settings = new GameSettingsStorage(_dbPath);
            History = new GameHistoryStorage(_dbPath);
        }

        public readonly GameSettingsStorage Settings;
        public readonly GameHistoryStorage History;
    }
}