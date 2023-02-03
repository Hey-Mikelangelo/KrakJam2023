using UnityEngine;

public static class PolarCoords
{
    public static Vector2 ToCartesianDeg(float radius, float angleDeg)
    {
        return ToCartesian(radius, (Mathf.Deg2Rad * angleDeg));
    }
    public static Vector2 ToCartesian(float radius, float angleRad)
    {
        return new Vector2(radius * Mathf.Cos(angleRad), radius * Mathf.Sin(angleRad));
    }
    public static (float radius, float angleDeg) FromCartesianDeg(Vector2 position)
    {
        var (radius, angleRad) = FromCartesian(position);
        return (radius, Mathf.Rad2Deg * angleRad);
    }
    public static (float radius, float angleRad) FromCartesian(Vector2 position)
    {
        float radius = Mathf.Sqrt((position.x * position.x) + (position.y * position.y));
        float angleRad = Mathf.Atan(position.y / position.x);
        return (radius, angleRad);
    }
}
