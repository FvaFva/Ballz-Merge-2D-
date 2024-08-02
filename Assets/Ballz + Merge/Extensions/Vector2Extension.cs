using UnityEngine;

public static class Vector2Extension
{
    public static Vector2Int CalculateDirection(this Vector2 first, Vector2 second)
    {
        Vector2 direction = first - second;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x < 0 ? Vector2Int.right : Vector2Int.left;
        else
            return direction.y > 0 ? Vector2Int.down : Vector2Int.up;
    }
}