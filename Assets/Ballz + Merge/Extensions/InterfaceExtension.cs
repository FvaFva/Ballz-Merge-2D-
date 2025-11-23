using UnityEngine;

public static class InterfaceExtension
{
    public static T GetInterface<T>(this MonoBehaviour component) where T : class
    {
        if (component == null)
            return null;

        if (component is T @interface)
            return @interface;

        void ShowError() => Debug.LogError($"{component.name} does not implement {typeof(T).Name}", component);
        PlatformRunner.Run(ShowError);

        return null;
    }
}