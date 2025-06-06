using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;

public class GameSavesStorage
{
    private const string GameSavesTable = "GameSaves";
    private const string Key = "Key";
    private const string Value = "Value";

    private string _dbPath;
    private GameSavedBlocksStorage _blocksStorage;
    private GameSavedVolumesStorage _volumesStorage;
    private GameSavedBlocksEffectsStorage _blockEffectsStorage;

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
                                        {Value} REAL)";

                command.ExecuteNonQuery();
            }

            _blocksStorage = new GameSavedBlocksStorage(connection);
            _volumesStorage = new GameSavedVolumesStorage(connection);
            _blockEffectsStorage = new GameSavedBlocksEffectsStorage(connection, _blocksStorage.GetTableName(), _blocksStorage.GetIDName());

            connection.Close();
        }
    }

    public void Save(KeyValuePair<string, float> data)
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"INSERT OR REPLACE INTO {GameSavesTable}
                                        ({Key}, {Value})
                                        VALUES
                                        (@{Key}, @{Value})";

                command.Parameters.AddWithValue(Key, data.Key);
                command.Parameters.AddWithValue(Value, data.Value);
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void SaveBlocks(IEnumerable<SavedBlock> savedBlocks)
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            _blocksStorage.Set(connection, savedBlocks);
            connection.Close();
        }
    }

    public void SaveVolumes(IEnumerable<SavedVolume> savedVolumes)
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            _volumesStorage.Set(connection, savedVolumes);
            connection.Close();
        }
    }

    public void SaveBlocksEffects(IEnumerable<SavedBlockEffect> savedBlocksEffects)
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            _blockEffectsStorage.Set(connection, savedBlocksEffects);
            connection.Close();
        }
    }

    public float Get(string key)
    {
        float value = 0;

        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT {Value} FROM {GameSavesTable} WHERE {Key} = @{Key}";
                command.Parameters.AddWithValue(Key, key);
                object result = command.ExecuteScalar();

                if (result != null)
                    value = float.Parse(result.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            connection.Close();
        }

        return value;
    }

    public IEnumerable<SavedBlock> GetSavedBlocks()
    {
        IEnumerable<SavedBlock> savedBlocks;

        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            savedBlocks = _blocksStorage.Get(connection);
            connection.Close();
        }

        return savedBlocks;
    }

    public IEnumerable<SavedVolume> GetSavedVolumes()
    {
        IEnumerable<SavedVolume> savedVolumes;

        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            savedVolumes = _volumesStorage.Get(connection);
            connection.Close();
        }

        return savedVolumes;
    }

    public IEnumerable<SavedBlockEffect> GetSavedBlocksEffects()
    {
        IEnumerable<SavedBlockEffect> savedEffects;

        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            savedEffects = _blockEffectsStorage.Get(connection);
            connection.Close();
        }

        return savedEffects;
    }

    public void EraseAllData()
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            Delete(connection);
            _blockEffectsStorage.Delete(connection);
            _blocksStorage.Delete(connection);
            _volumesStorage.Delete(connection);
            connection.Close();
        }
    }

    public bool CheckSaves()
    {
        bool isSaved = false;

        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            if (IsExist(connection) || _blockEffectsStorage.IsExist(connection) || _blocksStorage.IsExist(connection) || _volumesStorage.IsExist(connection))
                isSaved = true;

            connection.Close();
        }

        return isSaved;
    }

    private void Delete(SqliteConnection connection)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {GameSavesTable}";
            command.ExecuteNonQuery();
        }
    }

    private bool IsExist(SqliteConnection connection)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT EXISTS(SELECT 1 FROM {GameSavesTable})";
            object result = command.ExecuteScalar();
            return Convert.ToBoolean(result);
        }
    }
}