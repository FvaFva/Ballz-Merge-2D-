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

    public void Save(SaveDataContainer save)
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            {
                string commandText = $@"INSERT OR REPLACE INTO {GameSavesTable}
                                        ({Key}, {Value})
                                        VALUES
                                        (@{Key}, @{Value})";

                using (var command = new SqliteCommand(commandText, connection, transaction))
                {
                    command.Prepare();
                    var pValue = command.Parameters.Add($"@{Value}", System.Data.DbType.Double);
                    var pKey = command.Parameters.Add($"@{Key}", System.Data.DbType.String);

                    foreach (var saveDate in save.Main)
                    {
                        pValue.Value = saveDate.Value;
                        pKey.Value = saveDate.Key;
                        command.ExecuteNonQuery();
                    }
                }

                _blocksStorage.Set(connection, save.Blocks, transaction);
                _volumesStorage.Set(connection, save.Volumes, transaction);
                _blockEffectsStorage.Set(connection, save.BlockEffects, transaction);
                transaction.Commit();
            }

            connection.Close();
        }
    }

    public SaveDataContainer Get()
    {
        SaveDataContainer data = new SaveDataContainer();

        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            if (CheckSaves(connection))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT {Value}, {Key} FROM {GameSavesTable}";
                    Dictionary<string, float> main = new Dictionary<string, float>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            main.Add(Convert.ToString(reader[Key]), float.Parse(reader[Value].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture));
                    }

                    data = new SaveDataContainer(main, GetSavedBlocks(connection), GetSavedVolumes(connection), GetSavedBlocksEffects(connection));
                }
            }
            connection.Close();
        }

        return data;
    }

    public void EraseAllData()
    {
        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                Delete(connection, transaction);
                _blockEffectsStorage.Delete(connection, transaction);
                _blocksStorage.Delete(connection, transaction);
                _volumesStorage.Delete(connection, transaction);

                transaction.Commit();
            }

            connection.Close();
        }
    }

    public bool CheckSaves()
    {
        bool isSaved = false;

        using (var connection = new SqliteConnection(_dbPath))
        {
            connection.Open();
            isSaved = CheckSaves(connection);
            connection.Close();
        }

        return isSaved;
    }

    private bool CheckSaves(SqliteConnection connection) => IsExist(connection) || _blockEffectsStorage.IsExist(connection) || _blocksStorage.IsExist(connection) || _volumesStorage.IsExist(connection);

    private void Delete(SqliteConnection connection, SqliteTransaction transaction)
    {
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
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

    private IEnumerable<SavedBlock> GetSavedBlocks(SqliteConnection connection)
    {
        IEnumerable<SavedBlock> savedBlocks;
        savedBlocks = _blocksStorage.Get(connection);
        return savedBlocks;
    }

    private IEnumerable<SavedVolume> GetSavedVolumes(SqliteConnection connection)
    {
        IEnumerable<SavedVolume> savedVolumes;
        savedVolumes = _volumesStorage.Get(connection);
        return savedVolumes;
    }

    private IEnumerable<SavedBlockEffect> GetSavedBlocksEffects(SqliteConnection connection)
    {
        IEnumerable<SavedBlockEffect> savedEffects;
        savedEffects = _blockEffectsStorage.Get(connection);
        return savedEffects;
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
}