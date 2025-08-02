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

        private string _dbPath;
        private GameHistoryVolumesStorage _volumeStorage;

        public GameHistoryStorage(string basePath)
        {
            _dbPath = basePath;
            CreateSettingsTable();
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
                                               strftime('%d.%m.', game.{DateColumnName}) || substr(strftime('%Y', game.{DateColumnName}), 3, 2) || strftime(' %H:', game.{DateColumnName}) || strftime('%M', game.{DateColumnName}) as {DateColumnName},
                                               volumes.{_volumeStorage.ValueColumName} as {_volumeStorage.ValueColumName},
                                               volumes.{_volumeStorage.VolumeColumName} as {_volumeStorage.VolumeColumName}
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

        public int GetBestScore()
        {
            int bestScore = 0;

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT MAX({ScoreColumnName}) FROM {TableName}";
                    var result = command.ExecuteScalar();

                    if (result != DBNull.Value)
                        bestScore = Convert.ToInt32(result);
                }

                connection.Close();
            }

            return bestScore;
        }

        public void SaveResult(GameHistoryData data)
        {
            string gameUUID = Guid.NewGuid().ToString();

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {TableName} ({ScoreColumnName}, {IDColumnName}, {DateColumnName}, {LevelColumnName}) VALUES (@{ScoreColumnName}, @{IDColumnName}, @{DateColumnName}, @{LevelColumnName})";
                    command.Parameters.AddWithValue(ScoreColumnName, data.Score);
                    command.Parameters.AddWithValue(IDColumnName, gameUUID);
                    command.Parameters.AddWithValue(DateColumnName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue(LevelColumnName, data.Level);

                    command.ExecuteNonQuery();
                }

                _volumeStorage.Set(connection, gameUUID, data.Volumes);
                connection.Close();
            }
        }

        private void CreateSettingsTable()
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
                    data.Add(new GameHistoryData(lastGame, score, date, number, level));
                    currentDataId = data.Count - 1;
                }

                object value = reader[_volumeStorage.ValueColumName];
                object volume = reader[_volumeStorage.VolumeColumName];

                if (value == DBNull.Value || volume == DBNull.Value)
                    continue;

                data[currentDataId].Add(volume.ToString(), Convert.ToInt32(value));
            }
        }
    }
}
