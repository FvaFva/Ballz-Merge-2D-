using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 DropZ(this Vector3 vector) => (Vector2)vector;
}