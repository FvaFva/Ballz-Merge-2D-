using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using UnityEngine.Rendering;

namespace BallzMerge.Data
{
    public class GameHistoryVolumesStorage
    {
        private readonly string IDColumnName;

        public GameHistoryVolumesStorage(SqliteConnection connectionForInit, string idColumnName)
        {
            IDColumnName = idColumnName;
            CreateSettingsTable(connectionForInit);
        }

        public readonly string TableName = "GameVolumes";
        public readonly string VolumeColumnName = "Volume";
        public readonly string ValueColumnName = "Value";

        public void Set(SqliteConnection connection, string gameID, IReadOnlyDictionary<string, List<int>> volumes)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"   INSERT INTO {TableName}  
                                            ({VolumeColumnName}, {ValueColumnName}, {IDColumnName})  
                                            VALUES
                                            (@{VolumeColumnName}, @{ValueColumnName}, @{IDColumnName})";

                var idParam = command.Parameters.Add($"@{IDColumnName}", DbType.String);
                var nameParam = command.Parameters.Add($"@{VolumeColumnName}", DbType.String);
                var valueParam = command.Parameters.Add($"@{ValueColumnName}", DbType.Int64);

                foreach (var volume in volumes)
                {
                    if (volume.Value.Equals(0))
                        continue;

                    idParam.Value = gameID;
                    nameParam.Value = volume.Key;

                    foreach(var value in volume.Value)
                    {
                        valueParam.Value = value;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Erase(SqliteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"DELETE FROM {TableName}";
                command.ExecuteNonQuery();
            }
        }

        private void CreateSettingsTable(SqliteConnection connectionForInit)
        {
            using (var command = connectionForInit.CreateCommand())
            {
                command.CommandText = $@"   CREATE TABLE IF NOT EXISTS {TableName}
                                            (ID INTEGER PRIMARY KEY,
                                            {VolumeColumnName} TEXT,
                                            {ValueColumnName} INTEGER,
                                            {IDColumnName} TEXT)";

                command.ExecuteNonQuery();
            }
        }
    }
}
