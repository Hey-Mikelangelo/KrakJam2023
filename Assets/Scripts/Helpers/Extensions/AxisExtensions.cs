using UnityEngine;

public enum Axis
{
    None = 0,
    X = 1,
    Y = 2,
    Z = 3
}


public static class AxisExtensions
{
    public static Vector3 GetAxisVector(this Axis axis)
    {
        switch (axis)
        {
            case Axis.None: return Vector3.zero;
            case Axis.X: return new Vector3(1, 0, 0);
            case Axis.Y: return new Vector3(0, 1, 0);
            case Axis.Z: return new Vector3(0, 0, 1);
            default: return Vector3.zero;

        }
    }

    public static Vector3 GetAxisDirection(this Transform transform, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return transform.right;
            case Axis.Y: return transform.up;
            case Axis.Z: return transform.forward;
            default: return Vector3.zero;

        }
    }
    public static float GetAxisValue(this Vector3 vector, Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return vector.x;
            case Axis.Y:
                return vector.y;
            case Axis.Z:
                return vector.z;
            default:
                return 0;
        }
    }

    public static Vector3 GetVector3WithValueOnAxisY(this Vector2 vector2, float value)
    {
        return new Vector3(vector2.x, value, vector2.y);
    }

    public static Vector2 GetVector2WithRemovedValueOnAxisY(this Vector3 vector3)
    {
        return new Vector3(vector3.x, vector3.z);
    }

    public static Vector3 GetVector3WithValueOnAxis(this Vector2 vector2, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.X:
                return new Vector3(value, vector2.y, vector2.x);
            case Axis.Y:
                return new Vector3(vector2.x, value, vector2.y);
            case Axis.Z:
                return new Vector3(vector2.x, vector2.y, value);
            default: throw new System.Exception("Parameter axis cannot be None");
        }
    }

    public static Vector2 GetVector2WithRemovedValueOnAxis(this Vector3 vector3, Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return new Vector2(vector3.z, vector3.y);
            case Axis.Y:
                return new Vector3(vector3.x, vector3.z);
            case Axis.Z:
                return new Vector3(vector3.x, vector3.y);
            default: throw new System.Exception("Parameter axis cannot be None");
        }
    }

    public static Bounds GetBounds(this Rect rect, Axis extendAxis, float positionOffsetValue = 0, float sizeExpandValue = 0)
    {
        return new Bounds(
            rect.center.GetVector3WithValueOnAxis(extendAxis, positionOffsetValue), 
            rect.size.GetVector3WithValueOnAxis(extendAxis, sizeExpandValue));
    }

    public static Rect GetRect(this Bounds bounds, Axis flatAxis)
    {
        return new Rect(
            bounds.min.GetVector2WithRemovedValueOnAxis(flatAxis),
            bounds.size.GetVector2WithRemovedValueOnAxis(flatAxis));
    }

    public static Vector3 WithValueOnAxis(this Vector3 vector3, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.X:
                return new Vector3(value, vector3.y, vector3.z);
            case Axis.Y:
                return new Vector3(vector3.x, value, vector3.z);
            case Axis.Z:
                return new Vector3(vector3.x, vector3.y, value);
            default: throw new System.Exception("Parameter axis cannot be None");
        }
    }
}
