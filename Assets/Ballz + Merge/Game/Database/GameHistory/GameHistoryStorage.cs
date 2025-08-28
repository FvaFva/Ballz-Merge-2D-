using ModestTree;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace BallzMerge.Data
{
    public class GameHistoryStorage
    {
        private const string TableName = "GameHistory";
        private const string ScoreColumnName = "Score";
        private const string IDColumnName = "GameID";
        private const string DateColumnName = "Date";
        private const string NumberColumnName = "Number";
        private const string LevelColumnName = "Level";
        private const string IsCompletedColumnName = "IsCompleted";

        private string _dbPath;
        private GameHistoryVolumesStorage _volumeStorage;

        public GameHistoryStorage(string basePath)
        {
            _dbPath = basePath;
            CreateTable();
        }

        public List<GameHistoryData> GetData()
        {
            List<GameHistoryData> data = new List<GameHistoryData>();

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @$"SELECT 
                                               game.{IDColumnName} as {IDColumnName},
                                               game.{ScoreColumnName} as {ScoreColumnName},
                                               game.{NumberColumnName} as {NumberColumnName},
                                               game.{LevelColumnName} as {LevelColumnName},
                                               game.{IsCompletedColumnName} as {IsCompletedColumnName},
                                               strftime('%d.%m.', game.{DateColumnName}) || substr(strftime('%Y', game.{DateColumnName}), 3, 2) || strftime(' %H:', game.{DateColumnName}) || strftime('%M', game.{DateColumnName}) as {DateColumnName},
                                               volumes.{_volumeStorage.ValueColumnName} as {_volumeStorage.ValueColumnName},
                                               volumes.{_volumeStorage.VolumeColumnName} as {_volumeStorage.VolumeColumnName}
                                          FROM {TableName} as game
                                              LEFT JOIN {_volumeStorage.TableName} as volumes
                                              ON game.{IDColumnName} = volumes.{IDColumnName}
                                          ORDER BY {IDColumnName}";

                    using (var reader = command.ExecuteReader())
                        GenerateDataByReader(reader, data);
                }

                connection.Close();
            }

            return data;
        }

        public void EraseData()
        {
            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM {TableName}";
                    command.ExecuteNonQuery();
                }

                _volumeStorage.Erase(connection);
                connection.Close();
            }
        }

        public List<int> GetCompleted()
        {
            var completed = new List<int>();

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT DISTINCT {LevelColumnName} FROM {TableName} WHERE {IsCompletedColumnName} = 1";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            completed.Add(Convert.ToInt32(reader[LevelColumnName]));
                    }
                }

                connection.Close();
            }

            return completed;
        }

        public void SaveResult(GameHistoryData data)
        {
            string gameUUID = Guid.NewGuid().ToString();

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {TableName} ({ScoreColumnName}, {IDColumnName}, {DateColumnName}, {LevelColumnName}, {IsCompletedColumnName}) VALUES (@{ScoreColumnName}, @{IDColumnName}, @{DateColumnName}, @{LevelColumnName}, @{IsCompletedColumnName})";
                    command.Parameters.AddWithValue(ScoreColumnName, data.Score);
                    command.Parameters.AddWithValue(IDColumnName, gameUUID);
                    command.Parameters.AddWithValue(DateColumnName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue(LevelColumnName, data.Level);
                    command.Parameters.AddWithValue(IsCompletedColumnName, data.IsCompleted ? 1 : 0);

                    command.ExecuteNonQuery();
                }

                _volumeStorage.Set(connection, gameUUID, data.Volumes);
                connection.Close();
            }
        }

        private void CreateTable()
        {
            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"CREATE TABLE IF NOT EXISTS {TableName}
                                            ({NumberColumnName} INTEGER PRIMARY KEY,
                                            {ScoreColumnName} INTEGER,
                                            {DateColumnName} TEXT,
                                            {LevelColumnName} INTEGER,
                                            {IsCompletedColumnName} INTEGER,
                                            {IDColumnName} TEXT NOT NULL UNIQUE)";

                    command.ExecuteNonQuery();
                }

                _volumeStorage = new GameHistoryVolumesStorage(connection, IDColumnName);

                connection.Close();
            }
        }

        private void GenerateDataByReader(SqliteDataReader reader, List<GameHistoryData> data)
        {
            string lastGame = "";
            int currentDataId = 0;

            while (reader.Read())
            {
                string currentGame = reader[IDColumnName].ToString();

                if (currentGame.Equals(lastGame) == false)
                {
                    lastGame = currentGame;
                    int score = Convert.ToInt32(reader[ScoreColumnName]);
                    int number = Convert.ToInt32(reader[NumberColumnName]);
                    int level = Convert.ToInt32(reader[LevelColumnName]);
                    string date = reader[DateColumnName].ToString();
                    bool completed = Convert.ToInt32(reader[IsCompletedColumnName]) == 1;
                    data.Add(new GameHistoryData(lastGame, score, date, number, level, completed));
                    currentDataId = data.Count - 1;
                }

                object volume = reader[_volumeStorage.VolumeColumnName];
                object value = reader[_volumeStorage.ValueColumnName];

                if (value == DBNull.Value || volume == DBNull.Value)
                    continue;

                string volumeName = volume.ToString();
                int volumeValue = Convert.ToInt32(value);

                if (!data[currentDataId].Volumes.TryGetValue(volumeName, out var list))
                {
                    list = new List<int>();
                    data[currentDataId].Volumes[volumeName] = list;
                }

                list.Add(volumeValue);
            }
        }
    }
}
