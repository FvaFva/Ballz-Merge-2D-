using System;

public static class PlatformRunner
{
    public static void RunOnSpecificPlatform<T>(T target, Action<T> X64Action, Action<T> ARMAction)
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        X64Action?.Invoke(target);
#elif UNITY_ANDROID || UNITY_IOS
        ARMAction?.Invoke(target);
#endif
    }
}
