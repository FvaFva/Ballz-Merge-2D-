using System;

public static class PlatformRunner
{
    public static void RunOnSpecificPlatform(Action X64Action, Action ARMAction)
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        X64Action?.Invoke();
#elif UNITY_ANDROID || UNITY_IOS
        ARMAction?.Invoke();
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
