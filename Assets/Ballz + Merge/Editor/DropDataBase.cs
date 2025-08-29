using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine;

public class DropDataBase
{
    [MenuItem("Tools/Game/Drop DB/Drop history")]
    public static void DropHistory() => DropTable("GameHistory");

    [MenuItem("Tools/Game/Drop DB/Drop Achievements")]
    public static void DropAchievements() => DropTable("Achievements");

    private static void DropTable(string tableName)
    {
        string dbPath = "URI=file:" + Application.persistentDataPath + "/game_data.db";
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"DROP TABLE IF EXISTS [{tableName}];";
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }
}
