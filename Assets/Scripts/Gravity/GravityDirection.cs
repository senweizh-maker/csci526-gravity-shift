using UnityEngine;

public enum GravityDirection
{
    Up,
    Down,
    Left,
    Right
}

public static class GravityDirectionUtility
{
    public static Vector2 ToVector(GravityDirection direction)
    {
        switch (direction)
        {
            case GravityDirection.Up:
                return Vector2.up;

            case GravityDirection.Down:
                return Vector2.down;

            case GravityDirection.Left:
                return Vector2.left;

            case GravityDirection.Right:
                return Vector2.right;
        }

        return Vector2.down;
    }

    public static string ToArrow(GravityDirection direction)
    {
        switch (direction)
        {
            case GravityDirection.Up:
                return "↑";

            case GravityDirection.Down:
                return "↓";

            case GravityDirection.Left:
                return "←";

            case GravityDirection.Right:
                return "→";
        }

        return "";
    }
}