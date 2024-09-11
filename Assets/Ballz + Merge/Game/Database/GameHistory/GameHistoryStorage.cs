using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace BallzMerge.Data
{
    public class GameHistoryStorage
    {
        private const string TableName = "GameHistory";
        private const string ScoreColumName = "Score";
        private const string IDColumName = "GameID";
        private const string DateColumName = "Date";
        private const string NumberColumName = "Number";

        private string _dbPath;
        private GameHistoryVolumesStorage _volumeStorage;

        public GameHistoryStorage(string basePath)
        {
            _dbPath = basePath;
            CreateSettingsTable();
        }

        public List<GameHistoryData> GetData()
        {
            List <GameHistoryData> data = new List<GameHistoryData>();

            using(var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using(var command = connection.CreateCommand())
                {
                    command.CommandText = @$"SELECT 
                                               game.{IDColumName} as {IDColumName},
                                               game.{ScoreColumName} as {ScoreColumName},
                                               game.{NumberColumName} as {NumberColumName},
                                               game.{DateColumName} as {DateColumName},
                                               volumes.{_volumeStorage.ValueColumName} as {_volumeStorage.ValueColumName},
                                               volumes.{_volumeStorage.VolumeColumName} as {_volumeStorage.VolumeColumName}
                                          FROM {TableName} as game
                                              LEFT JOIN {_volumeStorage.TableName} as volumes
                                              ON game.{IDColumName} = volumes.{IDColumName}
                                          ORDER BY {IDColumName}";

                    using (var reader = command.ExecuteReader())
                        GenerateDataByReader(reader, data);
                }

                connection.Close();
            }

            return data;
        }

        public int GetBestScore()
        {
            int bestScore = 0;

            using(var connection =  new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT MAX({ScoreColumName}) FROM {TableName}";
                    var result = command.ExecuteScalar();

                    if(result != DBNull.Value)
                        bestScore = Convert.ToInt32(result);
                }

                connection.Close();
            }

            return bestScore;
        }

        public void SaveResult(int score, BallGlobalVolume volumes)
        {
            string gameUUID = Guid.NewGuid().ToString();

            using (var connection = new  SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {TableName} ({ScoreColumName}, {IDColumName}, {DateColumName}) VALUES (@{ScoreColumName}, @{IDColumName}, @{DateColumName})";
                    command.Parameters.AddWithValue(ScoreColumName, score);
                    command.Parameters.AddWithValue(IDColumName, gameUUID);
                    command.Parameters.AddWithValue(DateColumName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    command.ExecuteNonQuery();
                }

                _volumeStorage.Set(connection, gameUUID, volumes);
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
                                            ({NumberColumName} INTEGER PRIMARY KEY,
                                            {ScoreColumName} INTEGER,
                                            {DateColumName} TEXT,
                                            {IDColumName} TEXT NOT NULL UNIQUE)";

                    command.ExecuteNonQuery();
                }

                _volumeStorage = new GameHistoryVolumesStorage(connection, IDColumName);

                connection.Close();
            }
        }
        
        private void GenerateDataByReader(SqliteDataReader reader, List<GameHistoryData> data)
        {
            string lastGame = "";
            int currentDataId = 0;

            while (reader.Read())
            {
                string currentGame = reader[IDColumName].ToString();

                if (currentGame.Equals(lastGame) == false)
                {
                    lastGame = currentGame;
                    int score = Convert.ToInt32(reader[ScoreColumName]);
                    int number = Convert.ToInt32(reader[NumberColumName]);
                    string date = reader[DateColumName].ToString();
                    data.Add(new GameHistoryData(lastGame, score, date, number));
                    currentDataId = data.Count - 1;
                }

                string volume = reader[_volumeStorage.VolumeColumName].ToString();
                float value = Convert.ToSingle(reader[_volumeStorage.ValueColumName]);
                data[currentDataId].Add(volume, value);
            }
        }
    }
}
