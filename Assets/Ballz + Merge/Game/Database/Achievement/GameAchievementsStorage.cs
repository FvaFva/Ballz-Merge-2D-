using BallzMerge.Achievement;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace BallzMerge.Data
{
    public class GameAchievementsStorage
    {
        private const string TableName = "Achievements";
        private const string StepColumName = "Step";
        private const string PointsColumName = "Points";
        private const string KeyColumName = "Key";

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
                                            SET {PointsColumName} = {PointsColumName} + @{PointsColumName}
                                            WHERE {KeyColumName} = @{KeyColumName};";

                    command.Parameters.Add(new SqliteParameter($"@{PointsColumName}", points));
                    command.Parameters.Add(new SqliteParameter($"@{KeyColumName}", key.ToString()));
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
                        command.CommandText = $@"INSERT OR REPLACE INTO {TableName} ({KeyColumName}, {PointsColumName}, {StepColumName})
                                            VALUES (@{KeyColumName}, @{PointsColumName}, @{StepColumName});";
                        command.Parameters.AddWithValue(KeyColumName, sKey);
                        command.Parameters.AddWithValue(PointsColumName, value.Points);
                        command.Parameters.AddWithValue(StepColumName, value.Step);
                        command.ExecuteNonQuery();
                        isWroten = true;
                    }
                }

                connection.Close();
            }

            return isWroten;
        }

        public IDictionary<AchievementsTypes, AchievementPointsStep> GetAll()
        {
            Dictionary<AchievementsTypes, AchievementPointsStep> values = new Dictionary<AchievementsTypes, AchievementPointsStep>();

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT {PointsColumName}, {StepColumName}, {KeyColumName} FROM {TableName};";
                    command.ExecuteNonQuery();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if(Enum.TryParse<AchievementsTypes>(reader[KeyColumName].ToString(), out var key))
                                values.Add(key, new AchievementPointsStep(reader[PointsColumName],reader[StepColumName]));
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
                                            ({KeyColumName} TEXT PRIMARY KEY,
                                            {PointsColumName} INTEGER,
                                            {StepColumName} INTEGER)";

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
                command.CommandText = $"SELECT {PointsColumName}, {StepColumName} FROM {TableName} WHERE {KeyColumName} = @{KeyColumName};";
                command.Parameters.AddWithValue(KeyColumName, key);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pointsStep.Points = Convert.ToInt32(reader[PointsColumName]);
                        pointsStep.Step = Convert.ToInt32(reader[StepColumName]);
                    }
                }
            }

            return pointsStep;
        }
    }
}
