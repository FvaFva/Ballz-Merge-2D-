using System;
using UnityEngine;

public static class UnityObjectExtension
{
    public static void SetActive<T>(this T @object, bool state) where T : Component
    {
        if (@object != null)
            @object.gameObject.SetActive(state);
    }

    public static void PerformIfNotNull<T>(this T @object, Action<T> action) where T : Component
    {
        if (@object != null)
            action.Invoke(@object);
    }
}
