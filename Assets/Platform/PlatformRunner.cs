using System;

public static class PlatformRunner
{
    public static void RunOnDesktopPlatform(Action desktopAction, Action nonDesktopAction = null)
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        desktopAction?.Invoke();
#else
        nonDesktopAction?.Invoke();
#endif
    }

    public static void RunOnMobilePlatform(Action mobileAction, Action nonMobileAction = null)
    {
#if UNITY_ANDROID || UNITY_IOS                           
        mobileAction?.Invoke();
#else
        nonMobileAction?.Invoke();
#endif
    }

    public static void RunOnIOS(Action IOSAction, Action nonIOSAction = null)
    {
#if UNITY_IOS                           
        IOSAction?.Invoke();
#else
        nonIOSAction?.Invoke();
#endif
    }

    public static void RunOnEditor(Action editorAction, Action nonEditorAction = null)
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
