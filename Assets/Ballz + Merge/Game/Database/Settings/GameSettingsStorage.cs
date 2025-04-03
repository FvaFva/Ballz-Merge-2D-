using Mono.Data.Sqlite;
using System.Text;

namespace BallzMerge.Data
{
    public class GameSettingsStorage
    {
        private const string TableName = "GameSettings";
        private const string ColumNameKey = "Key";
        private const string ColumNameValue = "Value";

        private string _dbPath;

        public GameSettingsStorage(string basePath)
        {
            _dbPath = basePath;
            CreateSettingsTable();
        }

        public void Set(IGameSettingData setting)
        {
            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"INSERT OR REPLACE INTO {TableName}
                                            ({ColumNameKey}, {ColumNameValue})
                                             VALUES (@{ColumNameKey}, @{ColumNameValue})";

                    command.Parameters.AddWithValue($"@{ColumNameKey}", setting.Name);
                    command.Parameters.AddWithValue($"@{ColumNameValue}", setting.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public float Get(IGameSettingData setting)
        {
            float outputValue = setting.Value;

            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @$"SELECT {ColumNameValue} FROM {TableName} WHERE {ColumNameKey} = @{ColumNameKey}";
                    command.Parameters.AddWithValue($"@{ColumNameKey}", setting.Name);

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

        private void CreateSettingsTable()
        {
            using (var connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    StringBuilder builder = new StringBuilder();
                    command.CommandText = 
                    @$"CREATE TABLE IF NOT EXISTS {TableName} 
                    (
                        {ColumNameKey} TEXT PRIMARY KEY,
                        {ColumNameValue} REAL
                    )";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}