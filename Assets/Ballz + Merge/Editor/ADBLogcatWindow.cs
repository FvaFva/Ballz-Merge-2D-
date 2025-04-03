using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;

public class ADBLogcatWindow : EditorWindow
{
    private List<string> _errorsInConsole = new List<string>();
    private Thread _logcatThread;
    private bool _isRunning;
    private Vector2 _scrollPosition;
    private string _logText = "";
    private string _adbPath;
    private bool _logUpdated;
    private string _additionalProperty = "-s Unity DEBUG ERROR";
    private bool _isInDraw = false;
    private bool _isInLog;

    [MenuItem("Tools/ADB Logcat")]
    public static void ShowWindow()
    {
        GetWindow<ADBLogcatWindow>("ADB Logcat");
    }

    private void OnEnable()
    {
        string androidSdkPath = EditorPrefs.GetString("AndroidSdkRoot");

        if (!string.IsNullOrEmpty(androidSdkPath))
            _adbPath = Path.Combine(androidSdkPath, "platform-tools", "adb" + GetAdbExtension());
        else
            UnityEngine.Debug.LogError("Путь к Android SDK не найден.");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        float totalWidth = position.width;

        float labelWidth = totalWidth * 0.2f;
        float textFieldWidth = totalWidth * 0.25f;
        float buttonWidth = totalWidth * 0.1f;

        GUILayout.Label("Extra property:", GUILayout.Width(labelWidth));
        _additionalProperty = GUILayout.TextField(_additionalProperty, GUILayout.Width(textFieldWidth));
        _isInLog = GUILayout.Toggle(_isInLog, "In log", GUILayout.Width(textFieldWidth));

        if (GUILayout.Button("Start Logcat", GUILayout.Width(buttonWidth)))
            StartLogcat();

        if (GUILayout.Button("Stop Logcat", GUILayout.Width(buttonWidth)))
            StopLogcat();

        if (GUILayout.Button("Clear", GUILayout.Width(buttonWidth)))
            ClearLog();

        GUILayout.EndHorizontal();

        if (_isInDraw)
            return;

        _isInDraw = true;
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        string[] latestLogs = _logText.Split('\n');

        if (_isInLog == false)
        {
            foreach (string line in latestLogs)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                LogInGUI(line);
            }
        }

        GUILayout.EndScrollView();

        if (_logUpdated)
        {
            _scrollPosition.y = float.MaxValue;
            _logUpdated = false;
            Repaint();
        }

        GUI.color = Color.white;
        _isInDraw = false;
    }

    private void StartLogcat()
    {
        if (_adbPath == null)
        {
            UnityEngine.Debug.LogError("adbPath не инициализирован.");
            return;
        }

        _isRunning = true;
        _logcatThread = new Thread(ReadLogcat);
        _logcatThread.Start();
    }

    private void StopLogcat()
    {
        _isRunning = false;
        if (_logcatThread != null && _logcatThread.IsAlive)
        {
            _logcatThread.Abort();
            _logcatThread = null;
        }
    }

    private void ClearLog()
    {
        _logText = "";
        _scrollPosition = Vector2.zero;
        _errorsInConsole.Clear();
        Repaint();
    }

    private void ReadLogcat()
    {
        if (string.IsNullOrEmpty(_adbPath))
        {
            UnityEngine.Debug.LogError("Путь к Android SDK не найден.");
            return;
        }

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = _adbPath,
            Arguments = $"logcat {_additionalProperty}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(processInfo))
        using (StreamReader reader = process.StandardOutput)
        {
            while (_isRunning && !reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    _logText += line + "\n";
                    _logUpdated = true;

                    if(line.Contains("Log(") == false)
                        UnityEngine.Debug.Log(line);
                }
            }
        }
    }

    private string GetAdbExtension()
    {
        return Application.platform == RuntimePlatform.WindowsEditor ? ".exe" : "";
    }

    private void UpdateLog()
    {
        Repaint();
        EditorApplication.update -= UpdateLog;
    }

    private void LogInGUI(string line)
    {
        if (line.ToLower().Contains("error") || line.ToLower().Contains("exception"))
        {
            if (_errorsInConsole.Contains(line) == false)
            {
                _errorsInConsole.Add(line);
                UnityEngine.Debug.LogError($"WOW! Android did BRRRR - \n[{line}]");
            }

            GUI.color = Color.red;
        }
        else
        {
            GUI.color = Color.white;
        }

        GUILayout.Label(line);
    }
}
