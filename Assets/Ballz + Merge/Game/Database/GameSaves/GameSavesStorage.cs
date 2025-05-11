using Mono.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class GameSavesStorage
{
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
                command.CommandText = $@"CREATE TABLE IF NOT EXISTS {GameSavesTable}
                                            ({Key} TEXT PRIMARY KEY,
                                            {Value} TEXT)";

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void Save(IDictionary<string, object> data)
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
                    command.Parameters.AddWithValue(Value, JsonConvert.SerializeObject(item.Value));
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }
    }

    public IDictionary<string, object> Get()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {GameSavesTable}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        data.Add(reader[Key].ToString(), reader[Value]);
                }
            }

            connection.Close();
        }

        return data;
    }
}