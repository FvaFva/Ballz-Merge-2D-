using System;

public static class PlatformRunner
{
    public static void Run(Action basic, Action editorAction = null, Action windowsAction = null, Action macAction = null, Action androidAction = null, Action iosAction = null)
    {
        editorAction ??= basic;
        windowsAction ??= basic;
        macAction ??= basic;
        androidAction ??= basic;
        iosAction ??= basic;

#if UNITY_EDITOR
        editorAction?.Invoke();
#elif UNITY_STANDALONE_WIN
        windowsAction?.Invoke();
#elif UNITY_STANDALONE_OSX
        macAction?.Invoke();
#elif UNITY_ANDROID
        androidAction?.Invoke();
#elif UNITY_IOS
        iosAction?.Invoke();
#endif
    }

    public static void QuitPlayMode()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}