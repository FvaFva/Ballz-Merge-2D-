using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

public class GameSavedBlocksStorage
{
    private const string TableName = "GameSavedBlocks";
    private const string ID = "ID";
    private const string Number = "Number";
    private const string GridPositionX = "GridPositionX";
    private const string GridPositionY = "GridPositionY";

    public GameSavedBlocksStorage(SqliteConnection connectionForInit)
    {
        CreateTable(connectionForInit);
    }

    public string GetTableName() => TableName;

    public string GetIDName() => ID;

    public void Set(SqliteConnection connection, IEnumerable<SavedBlock> savedBlocks, SqliteTransaction transaction)
    {
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            Delete(connection, transaction);

            foreach (SavedBlock savedBlock in savedBlocks)
            {
                command.CommandText = $@"   INSERT INTO {TableName}
                                            ({ID}, {Number}, {GridPositionX}, {GridPositionY})
                                            VALUES
                                            (@{ID}, @{Number}, @{GridPositionX}, @{GridPositionY})";

                command.Parameters.AddWithValue(ID, savedBlock.ID);
                command.Parameters.AddWithValue(Number, savedBlock.Number);
                command.Parameters.AddWithValue(GridPositionX, savedBlock.GridPositionX);
                command.Parameters.AddWithValue(GridPositionY, savedBlock.GridPositionY);
                command.ExecuteNonQuery();
            }
        }
    }

    public void Delete(SqliteConnection connection, SqliteTransaction transaction)
    {
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = $"DELETE FROM {TableName}";
            command.ExecuteNonQuery();
        }
    }

    public IEnumerable<SavedBlock> Get(SqliteConnection connection)
    {
        List<SavedBlock> savedBlocks = new List<SavedBlock>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT {ID}, {Number}, {GridPositionX}, {GridPositionY} FROM {TableName}";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    savedBlocks.Add(new SavedBlock(Convert.ToInt32(reader[ID]), Convert.ToInt32(reader[Number]), Convert.ToInt32(reader[GridPositionX]), Convert.ToInt32(reader[GridPositionY])));
            }
        }

        return savedBlocks;
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

    private void CreateTable(SqliteConnection connectionForInit)
    {
        using (var command = connectionForInit.CreateCommand())
        {
            command.CommandText = $@"   CREATE TABLE IF NOT EXISTS {TableName}
                                            ({ID} INTEGER PRIMARY KEY,
                                            {Number} INTEGER,
                                            {GridPositionX} INTEGER,
                                            {GridPositionY} INTEGER)";

            command.ExecuteNonQuery();
        }
    }
}
