using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

public class GameSavedBlocksEffectsStorage
{
    private const string TableName = "GameSavedBlocksEffects";
    private const string Name = "Name";
    private const string EffectBlock = "EffectBlock";
    private const string ConnectBlock = "ConnectBlock";

    public GameSavedBlocksEffectsStorage(SqliteConnection connection, string connectTable, string connectField)
    {
        CreateTable(connection, connectTable, connectField);
    }

    public void Set(SqliteConnection connection, IEnumerable<SavedBlockEffect> savedEffects)
    {
        using (var command = connection.CreateCommand())
        {
            Delete(connection);

            foreach (SavedBlockEffect savedEffect in savedEffects)
            {
                command.CommandText = $@"   INSERT INTO {TableName}
                                            ({Name}, {EffectBlock}, {ConnectBlock})
                                            VALUES
                                            (@{Name}, @{EffectBlock}, @{ConnectBlock})";

                command.Parameters.AddWithValue(Name, savedEffect.Name);
                command.Parameters.AddWithValue(EffectBlock, savedEffect.EffectBlock);
                command.Parameters.AddWithValue(ConnectBlock, savedEffect.ConnectBlock);
                command.ExecuteNonQuery();
            }
        }
    }

    public void Delete(SqliteConnection connection)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {TableName}";
            command.ExecuteNonQuery();
        }
    }

    public IEnumerable<SavedBlockEffect> Get(SqliteConnection connection)
    {
        List<SavedBlockEffect> savedEffects = new List<SavedBlockEffect>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT {Name}, {EffectBlock}, {ConnectBlock} FROM {TableName}";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    savedEffects.Add(new SavedBlockEffect(Convert.ToString(reader[Name]), Convert.ToInt32(reader[EffectBlock]), Convert.ToInt32(reader[ConnectBlock])));
            }
        }

        return savedEffects;
    }

    public bool IsExist(SqliteConnection connection)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT EXISTS(SELECT 1 FROM {TableName})";
            object result = command.ExecuteScalar();
            return Convert.ToBoolean(result);
        }
    }

    private void CreateTable(SqliteConnection connection, string connectTable, string connectField)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"   CREATE TABLE IF NOT EXISTS {TableName}
                                            ({Name} TEXT,
                                            {EffectBlock} INTEGER,
                                            {ConnectBlock} INTEGER,
                                            FOREIGN KEY ({EffectBlock}) REFERENCES {connectTable}({connectField}) ON DELETE CASCADE,
                                            FOREIGN KEY ({ConnectBlock}) REFERENCES {connectTable}({connectField}) ON DELETE CASCADE)";

            command.ExecuteNonQuery();
        }
    }
}
