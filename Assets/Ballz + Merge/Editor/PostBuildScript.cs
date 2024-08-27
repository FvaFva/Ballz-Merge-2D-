using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class PostBuildScript
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.Android)
        {
            UnityEngine.Debug.LogWarning($"���� �� ������� ��������, �������� ��������� ������ �� �������� ����������...");
            InstallApkOnDevices(pathToBuiltProject);
        }
    }

    private static void InstallApkOnDevices(string apkPath)
    {
        string androidSdkPath = EditorPrefs.GetString("AndroidSdkRoot");

        if (string.IsNullOrEmpty(androidSdkPath))
        {
            UnityEngine.Debug.LogError("���� � Android SDK �� ������. ����������, ��������� Android SDK � Unity Preferences.");
            return;
        }

        string adbPath = Path.Combine(androidSdkPath, "platform-tools", "adb" + GetAdbExtension());

        if (!File.Exists(adbPath))
        {
            UnityEngine.Debug.LogError($"ADB �� ������ �� ����: {adbPath}");
            return;
        }

        string devicesOutput = RunProcess(adbPath, "devices");
        string[] lines = devicesOutput.Split('\n');

        if(lines.Length == 0)
            UnityEngine.Debug.LogError($"� ADB �� ������� devices");

        foreach (string line in lines)
        {
            string[] parts = line.Split('\t');

            if (parts.Length > 1 && parts[1].Trim() == "device")
                InstallApkOnDevice(adbPath, apkPath, parts[0].Trim());
        }
    }

    private static string RunProcess(string adbPath, string argument)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo();

        processInfo.FileName = adbPath;
        processInfo.Arguments = argument;
        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;

        Process adbDevicesProcess = Process.Start(processInfo);
        string output = adbDevicesProcess.StandardOutput.ReadToEnd();
        string error = adbDevicesProcess.StandardError.ReadToEnd();
        adbDevicesProcess.WaitForExit();

        if (string.IsNullOrEmpty(error) == false)
            UnityEngine.Debug.LogError($"������ ��� ���������� �������: {error}");

        return output;
    }

    private static void InstallApkOnDevice(string adbPath, string apkPath, string deviceId)
    {
        string arguments = $"-s {deviceId} install -r \"{apkPath}\"";
        string installOutput = RunProcess(adbPath, arguments);
        UnityEngine.Debug.Log($"��������� APK �� ���������� {deviceId}: {installOutput}");
    }

    private static string GetAdbExtension()
    {
        return Application.platform == RuntimePlatform.WindowsEditor ? ".exe" : "";
    }
}
