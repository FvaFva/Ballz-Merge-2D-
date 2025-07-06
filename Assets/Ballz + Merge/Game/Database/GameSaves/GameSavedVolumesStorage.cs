using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;

public class GameSavedVolumesStorage
{
    private const string TableName = "GameSavedVolumes";
    private const string CageID = "CageID";
    private const string Name = "Name";
    private const string Weight = "Weight";

    public GameSavedVolumesStorage(SqliteConnection connection)
    {
        CreateTable(connection);
    }

    public void Set(SqliteConnection connection, IEnumerable<SavedVolume> savedVolumes)
    {
        using (var command = connection.CreateCommand())
        {
            Delete(connection);

            foreach (SavedVolume savedVolume in savedVolumes)
            {
                command.CommandText = $@"   INSERT INTO {TableName}
                                            ({CageID}, {Name}, {Weight})
                                            VALUES
                                            (@{CageID}, @{Name} @{Weight})";

                command.Parameters.AddWithValue(CageID, savedVolume.ID);
                command.Parameters.AddWithValue(Name, savedVolume.Name);
                command.Parameters.AddWithValue(Weight, savedVolume.Weight);
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

    public IEnumerable<SavedVolume> Get(SqliteConnection connection)
    {
        List<SavedVolume> savedVolumes = new List<SavedVolume>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT {CageID}, {Name}, {Weight} FROM {TableName}";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    savedVolumes.Add(new SavedVolume(Convert.ToInt32(reader[CageID]), Convert.ToString(reader[Name]), Convert.ToInt32(reader[Weight])));
            }
        }

        return savedVolumes;
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

    private void CreateTable(SqliteConnection connection)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"   CREATE TABLE IF NOT EXISTS {TableName}
                                            ({CageID} INTEGER,
                                            {Name} TEXT,
                                            {Weight} INTEGER)";

            command.ExecuteNonQuery();
        }
    }
}
