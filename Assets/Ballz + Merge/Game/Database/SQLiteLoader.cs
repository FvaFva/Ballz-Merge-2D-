#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SQLiteLoader
{
    [DllImport("libsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_open_v2(string filename, out IntPtr db, int flags, IntPtr vfs);

    [DllImport("libdl.so", EntryPoint = "dlopen")]
    private static extern IntPtr Dlopen(string filename, int flags);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Debug.Log("[SQLiteLoader] Attempting to manually load sqlite3...");

        IntPtr handle = Dlopen("libsqlite3.so", 0x001); // RTLD_LAZY
        if (handle == IntPtr.Zero)
        {
            Debug.LogError("[SQLiteLoader] Failed to load libsqlite3.so with dlopen!");
        }
        else
        {
            Debug.Log("[SQLiteLoader] Successfully loaded libsqlite3.so with dlopen!");
        }

        // Теперь вызываем sqlite3_open_v2
        try
        {
            Debug.Log("[SQLiteLoader] Trying to call sqlite3_open_v2...");
            IntPtr db;
            int flags = 2;
            int result = sqlite3_open_v2(":memory:", out db, flags, IntPtr.Zero);

            if (result == 0)
            {
                Debug.Log("[SQLiteLoader] SQLite opened successfully!");
            }
            else
            {
                Debug.LogError($"[SQLiteLoader] SQLite open failed with error code: {result}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SQLiteLoader] Failed to load sqlite3: {e}");
        }
    }
}
#endif