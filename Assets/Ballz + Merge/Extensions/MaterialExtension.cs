using System;
using UnityEngine;

public static class MaterialExtension
{
    public static void PerformIfPropertyExist(this Material material, string property, Action<Material> action)
    {
        if (material == null)
            return;

        if (material.HasProperty(property))
            action.Invoke(material);
    }
}
