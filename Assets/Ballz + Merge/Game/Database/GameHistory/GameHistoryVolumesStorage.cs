using Mono.Data.Sqlite;

namespace BallzMerge.Data
{
    public class GameHistoryVolumesStorage
    {
        private readonly string IDColumName;

        public GameHistoryVolumesStorage(SqliteConnection connectionForInit, string iDColumName)
        {
            IDColumName = iDColumName;
            CreateSettingsTable(connectionForInit);
        }

        public readonly string TableName = "GameVolumes";
        public readonly string VolumeColumName = "Volume";
        public readonly string ValueColumName = "Value";

        public void Set(SqliteConnection connection, string gameID, BallVolumesBag volumes)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"   INSERT INTO {TableName}  
                                            ({VolumeColumName}, {ValueColumName}, {IDColumName})  
                                            VALUES
                                            (@{VolumeColumName}, @{ValueColumName}, @{IDColumName})";

                command.Parameters.AddWithValue($"@{IDColumName}", gameID);

                foreach (BallVolumesBagCell<BallVolume> cell in volumes.All)
                {
                    if (cell.Value.Equals(0))
                        continue;

                    command.Parameters.AddWithValue($"@{VolumeColumName}", cell.Name);
                    command.Parameters.AddWithValue($"@{ValueColumName}", cell.Value);
                    command.ExecuteNonQuery();
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
                                            {VolumeColumName} TEXT,
                                            {ValueColumName} INTEGER,
                                            {IDColumName} TEXT)";

                command.ExecuteNonQuery();
            }
        }
    }
}
