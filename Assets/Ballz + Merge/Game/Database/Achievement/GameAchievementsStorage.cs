using BallzMerge.Achievement;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.LookDev;

namespace BallzMerge.Data
{
    public class GameAchievementsStorage
    {
        private const string TableName = "Achievements";
        private const string StepColumnName = "Step";
        private const string PointsColumnName = "Points";
        private const string KeyColumnName = "Key";

        private string _dbPath;

        public GameAchievementsStorage(string dbPath)
        {
            _dbPath = dbPath;
            CreateTable();
        }

        public void AddPoints(AchievementsTypes key, int points)
        {
            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"UPDATE {TableName}
                                            SET {PointsColumnName} = @{PointsColumnName}
                                            WHERE {KeyColumnName} = @{KeyColumnName};";

                    command.Parameters.Add(new SqliteParameter($"@{PointsColumnName}", points));
                    command.Parameters.Add(new SqliteParameter($"@{KeyColumnName}", key.ToString()));
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public bool TryWrite(AchievementsTypes key, AchievementPointsStep value)
        {
            string sKey = key.ToString();
            bool isWroten = false;

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();
                var fromBase = GetPointsStep(connection, sKey);

                if (value.IsNewerThan(fromBase))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $@"INSERT OR REPLACE INTO {TableName} ({KeyColumnName}, {PointsColumnName}, {StepColumnName})
                                            VALUES (@{KeyColumnName}, @{PointsColumnName}, @{StepColumnName});";
                        command.Parameters.AddWithValue(KeyColumnName, sKey);
                        command.Parameters.AddWithValue(PointsColumnName, value.Points);
                        command.Parameters.AddWithValue(StepColumnName, value.Step);
                        command.ExecuteNonQuery();
                        isWroten = true;
                    }
                }

                connection.Close();
            }

            return isWroten;
        }

        public void DeleteAchievement(AchievementsTypes type)
        {
            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"DELETE FROM {TableName}
                                            WHERE {KeyColumnName} = @{KeyColumnName}";

                    command.Parameters.AddWithValue(KeyColumnName, type.ToString());
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public IDictionary<AchievementsTypes, AchievementPointsStep> GetAll()
        {
            Dictionary<AchievementsTypes, AchievementPointsStep> values = new Dictionary<AchievementsTypes, AchievementPointsStep>();

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT {PointsColumnName}, {StepColumnName}, {KeyColumnName} FROM {TableName};";
                    command.ExecuteNonQuery();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (Enum.TryParse<AchievementsTypes>(reader[KeyColumnName].ToString(), out var key))
                                values.Add(key, new AchievementPointsStep(reader[PointsColumnName], reader[StepColumnName]));
                        }
                    }
                }

                connection.Close();
            }

            return values;
        }

        public AchievementPointsStep GetPointsStep(AchievementsTypes type)
        {
            AchievementPointsStep pointsStep = new AchievementPointsStep();

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();
                pointsStep = GetPointsStep(connection, type.ToString());
                connection.Close();
            }

            return pointsStep;
        }

        private void CreateTable()
        {
            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"CREATE TABLE IF NOT EXISTS {TableName}
                                            ({KeyColumnName} TEXT PRIMARY KEY,
                                            {PointsColumnName} INTEGER,
                                            {StepColumnName} INTEGER)";

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private AchievementPointsStep GetPointsStep(SqliteConnection connection, string key)
        {
            AchievementPointsStep pointsStep = new AchievementPointsStep();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT {PointsColumnName}, {StepColumnName} FROM {TableName} WHERE {KeyColumnName} = @{KeyColumnName};";
                command.Parameters.AddWithValue(KeyColumnName, key);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pointsStep.Points = Convert.ToInt32(reader[PointsColumnName]);
                        pointsStep.Step = Convert.ToInt32(reader[StepColumnName]);
                    }
                }
            }

            return pointsStep;
        }
    }
}
