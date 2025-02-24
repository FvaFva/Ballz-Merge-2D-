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
                                               game.{IDColumnName} as {IDColumnName},
                                               game.{ScoreColumnName} as {ScoreColumnName},
                                               game.{NumberColumnName} as {NumberColumnName},
                                               strftime('%d.%m.', game.{DateColumnName}) || substr(strftime('%Y', game.{DateColumnName}), 3, 2) as {DateColumnName},
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

        public int GetBestScore()
        {
            int bestScore = 0;

            using(var connection =  new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT MAX({ScoreColumnName}) FROM {TableName}";
                    var result = command.ExecuteScalar();

                    if(result != DBNull.Value)
                        bestScore = Convert.ToInt32(result);
                }

                connection.Close();
            }

            return bestScore;
        }

        public void SaveResult(int score, BallVolumesBag volumes)
        {
            string gameUUID = Guid.NewGuid().ToString();

            using (var connection = new  SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {TableName} ({ScoreColumnName}, {IDColumnName}, {DateColumnName}) VALUES (@{ScoreColumnName}, @{IDColumnName}, @{DateColumnName})";
                    command.Parameters.AddWithValue(ScoreColumnName, score);
                    command.Parameters.AddWithValue(IDColumnName, gameUUID);
                    command.Parameters.AddWithValue(DateColumnName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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
                                            ({NumberColumnName} INTEGER PRIMARY KEY,
                                            {ScoreColumnName} INTEGER,
                                            {DateColumnName} TEXT,
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
                    string date = reader[DateColumnName].ToString();
                    data.Add(new GameHistoryData(lastGame, score, date, number));
                    currentDataId = data.Count - 1;
                }

                object dbValue = reader[_volumeStorage.ValueColumName];
                object dbVolume = reader[_volumeStorage.VolumeColumName];
                string volume = dbVolume.ToString();
                int value = Convert.IsDBNull(dbValue) ? 0 : Convert.ToInt32(dbValue);
                data[currentDataId].Add(volume, value);
            }
        }
    }
}
