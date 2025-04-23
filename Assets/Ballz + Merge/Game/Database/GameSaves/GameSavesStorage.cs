using Mono.Data.Sqlite;

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

    public void TemporarySave(string key, float value)
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT OR REPLACE INTO {TemporaryGameSavesTable} ({Key}, {Value}) VALUES (@{Key}, @{Value})";
                command.Parameters.AddWithValue(Key, key);
                command.Parameters.AddWithValue(Value, value);
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void Save()
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO {GameSavesTable} ({Key}, {Value}) SELECT {Key}, {Value} FROM {TemporaryGameSavesTable}";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public float Get(string key)
    {
        float outputValue = 0;
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT {Value} FROM {GameSavesTable} WHERE {Key} = @{Key}";
                command.Parameters.AddWithValue($"@{Key}", key);
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