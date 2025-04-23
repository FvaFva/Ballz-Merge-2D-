using Mono.Data.Sqlite;
using System.Collections.Generic;
using Unity.Services.CloudSave.Models;

public class GameSavesStorage
{
    private const string TemporaryGameSavesTable = "TemporaryGameSaves";
    private const string GameSavesTable = "GameSaves";
    private const string Key = "Key";
    private const string Value = "Value";

    private string _dbPath;

    public GameSavesStorage(string dbPath)
    {
        _dbPath = dbPath;
        CreateTable();
    }

    private void CreateTable()
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"CREATE TABLE IF NOT EXISTS {TemporaryGameSavesTable}
                                            ({Key} TEXT PRIMARY KEY,
                                            {Value} REAL)";

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void Save(IDictionary<string, float> data)
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                foreach (var item in data)
                {
                    command.CommandText = $"INSERT OR REPLACE INTO {GameSavesTable} ({Key}, {Value}) VALUES (@{Key}, @{Value})";
                    command.Parameters.AddWithValue(Key, item.Key);
                    command.Parameters.AddWithValue(Value, item.Value);
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }
    }

    public IDictionary<string, float> Get()
    {
        float outputValue = 0;
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {GameSavesTable}";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        outputValue = reader.GetFloat(0);
                }
            }

            connection.Close();
        }

        return outputValue;
    }
}