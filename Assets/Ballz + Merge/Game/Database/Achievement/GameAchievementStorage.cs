using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace BallzMerge.Data
{
    public class GameAchievementStorage
    {
        private string _dbPath;

        public GameAchievementStorage(string basePath)
        {
            _dbPath = basePath;
        }
    }
}
