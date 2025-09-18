using System;

public static class PlatformRunner
{
    public static void RunOnDesktopMobilePlatform(Action desktopAction, Action mobileAction)
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        desktopAction?.Invoke();
#elif UNITY_ANDROID || UNITY_IOS
        mobileAction?.Invoke();
#endif
    }

    public static void RunOnEditor(Action editorAction, Action nonEditorAction)
    {
#if UNITY_EDITOR
        editorAction?.Invoke();
#else
        nonEditorAction?.Invoke();
#endif
    }

    public static void QuitPlayMode()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
