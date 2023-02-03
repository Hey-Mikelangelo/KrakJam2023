using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtils
{
    private const int maxHitsCount = 15;
    private static RaycastHit[] hits = new RaycastHit[maxHitsCount];

    public static Quaternion GetLocalRotationOnLocalAxis(Quaternion initialLocalRotation, float angle, Vector3 rotationWorldAxisVector)
    {
        return initialLocalRotation * Quaternion.AngleAxis(angle, rotationWorldAxisVector);
    }

    public static Quaternion GetLocalRotationOnLocalAxis(Quaternion initialLocalRotation, float angle, Axis axis)
    {
        return GetLocalRotationOnLocalAxis(initialLocalRotation, angle, axis.GetAxisVector());
    }

    public static Quaternion GetLocalRotationRotated(Quaternion initialLocalRotation, float x, float y, float z)
    {
        return initialLocalRotation
            * Quaternion.AngleAxis(x, new Vector3(1, 0, 0))
            * Quaternion.AngleAxis(y, new Vector3(0, 1, 0))
            * Quaternion.AngleAxis(z, new Vector3(0, 0, 1));
    }

    public static Quaternion GetRotationWithForwardDirection(Vector3 forward)
    {
        return Quaternion.LookRotation(forward);
    }

    public static Quaternion GetRotationWithUpDirection(Vector3 up)
    {
        return Quaternion.LookRotation(up) * Quaternion.AngleAxis(90, Vector3.right);
    }

    public static Quaternion GetRotationWithForwardAndUpDirection(Vector3 forward, Vector3 up)
    {
        return Quaternion.LookRotation(forward, up);
    }

    public static Quaternion GetRotationWithRightDirection(Vector3 right)
    {
        return Quaternion.LookRotation(right) * Quaternion.AngleAxis(90, Vector3.up);
    }

    public static Vector3 RandomizeDirection(Vector3 direction, float radius)
    {
        Vector3 xDirection = direction;
        Vector3 yDirection = Vector3.Cross(xDirection, Vector3.up).normalized;
        Vector3 zDirection = Vector3.Cross(xDirection, yDirection).normalized;

        Vector3 yDirectionRandom = yDirection * Random.Range(-radius, radius);
        Vector3 zDirectionRandom = zDirection * Random.Range(-radius, radius);

        Vector3 randomFromDirection = (yDirectionRandom + zDirectionRandom).normalized * Random.Range(-radius, radius);
        Vector3 newDirection = (direction + randomFromDirection).normalized;
        return newDirection;
    }

    public static (float x, float y) GetXYToMatchAngle(float angleDegrees, float distance)
    {
        float angleRadians = Mathf.Deg2Rad * angleDegrees;
        var x = distance * Mathf.Sin(Mathf.PI * 2 * angleDegrees / 360);
        var y = distance * Mathf.Cos(Mathf.PI * 2 * angleDegrees / 360);
        return (x, y);
    }

    public static Bounds GetBoundsFromCorners(Vector3 corner1, Vector3 corner2)
    {
        float minX = Mathf.Min(corner1.x, corner2.x);
        float minZ = Mathf.Min(corner1.z, corner2.z);
        float maxX = Mathf.Max(corner1.x, corner2.x);
        float maxZ = Mathf.Max(corner1.z, corner2.z);
        float width = (maxX - minX);
        float length = (maxZ - minZ);
        Vector3 center = new Vector3(minX + (width * 0.5f), corner1.y, minZ + (length * 0.5f));
        Vector3 size = new Vector3(width, 0, length);

        return new Bounds(center, size);
    }
    public static Quaternion QuaternionSmoothDamp(Quaternion currentRotation, Quaternion targetRotation, ref Quaternion derivative, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return currentRotation;
        // account for double-cover
        var Dot = Quaternion.Dot(currentRotation, targetRotation);
        var Multi = Dot > 0f ? 1f : -1f;
        targetRotation.x *= Multi;
        targetRotation.y *= Multi;
        targetRotation.z *= Multi;
        targetRotation.w *= Multi;
        // smooth damp (nlerp approx)
        var Result = new Vector4(
            Mathf.SmoothDamp(currentRotation.x, targetRotation.x, ref derivative.x, time),
            Mathf.SmoothDamp(currentRotation.y, targetRotation.y, ref derivative.y, time),
            Mathf.SmoothDamp(currentRotation.z, targetRotation.z, ref derivative.z, time),
            Mathf.SmoothDamp(currentRotation.w, targetRotation.w, ref derivative.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(derivative.x, derivative.y, derivative.z, derivative.w), Result);
        derivative.x -= derivError.x;
        derivative.y -= derivError.y;
        derivative.z -= derivError.z;
        derivative.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }

    public static (Vector2 min, Vector2 max) GetMinMax(params Vector2[] positions)
    {
        return GetMinMax((IReadOnlyList<Vector2>)positions);
    }

    public static (Vector2Int min, Vector2Int max) GetMinMax(params Vector2Int[] positions)
    {
        return GetMinMax((IReadOnlyList<Vector2Int>)positions);
    }

    public static (int min, int max) GetMinMax(int val1, int val2)
    {
        return val1 < val2 ? (val1, val2) : (val2, val1);
    }

    public static (Vector2Int min, Vector2Int max) GetMinMax(IReadOnlyList<Vector2Int> positions)
    {
        int xMin = int.MaxValue, xMax = int.MinValue, yMin = int.MaxValue, yMax = int.MinValue;
        int count = positions.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2Int pos = positions[i];
            if (pos.x < xMin)
            {
                xMin = pos.x;
            }
            if (pos.x > xMax)
            {
                xMax = pos.x;
            }
            if (pos.y < yMin)
            {
                yMin = pos.y;
            }
            if (pos.y > yMax)
            {
                yMax = pos.y;
            }
        }

        return (new Vector2Int(xMin, yMin), new Vector2Int(xMax, yMax));
    }

    public static (Vector2 min, Vector2 max) GetMinMax(IReadOnlyList<Vector2> positions)
    {
        float xMin = float.MaxValue, xMax = float.MinValue, yMin = float.MaxValue, yMax = float.MinValue;
        int count = positions.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = positions[i];
            if (pos.x < xMin)
            {
                xMin = pos.x;
            }
            if (pos.x > xMax)
            {
                xMax = pos.x;
            }
            if (pos.y < yMin)
            {
                yMin = pos.y;
            }
            if (pos.y > yMax)
            {
                yMax = pos.y;
            }
        }

        return (new Vector2(xMin, yMin), new Vector2(xMax, yMax));
    }

    public static (Vector3 min, Vector3 max) GetMinMax(IReadOnlyList<Vector3> positions)
    {
        float xMin = float.MaxValue, xMax = float.MinValue, 
            yMin = float.MaxValue, yMax = float.MinValue, 
            zMin = float.MaxValue, zMax = float.MinValue;
        int count = positions.Count;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = positions[i];
            if (pos.x < xMin)
            {
                xMin = pos.x;
            }
            if (pos.x > xMax)
            {
                xMax = pos.x;
            }
            if (pos.y < yMin)
            {
                yMin = pos.y;
            }
            if (pos.y > yMax)
            {
                yMax = pos.y;
            }
            if(pos.z < zMin)
            {
                zMin = pos.z;
            }
            if(pos.z > zMax)
            {
                zMax = pos.z;
            }
        }

        return (new Vector3(xMin, yMin, zMin), new Vector3(xMax, yMax, zMax));
    }

    public static (Vector2 min, Vector2 max) GetMinMax(Vector2 pos1, Vector2 pos2)
    {
        float xMin, xMax, yMin, yMax;
        if (pos1.x < pos2.x)
        {
            xMin = pos1.x;
            xMax = pos2.x;
        }
        else
        {
            xMax = pos1.x;
            xMin = pos2.x;
        }
        if (pos1.y < pos2.y)
        {
            yMin = pos1.y;
            yMax = pos2.y;
        }
        else
        {
            yMax = pos1.y;
            yMin = pos2.y;
        }
        return (new Vector2(xMin, yMin), new Vector2(xMax, yMax));
    }

    public static float RoundTo3DP(float value)
    {
        return Mathf.Round(value * 1000) / 1000;
    }

    public static float ColorInverseLerp(Color from, Color to, Color value)
    {
        float rT;
        if (from.r == to.r && to.r == value.r)
        {
            rT = 1;
        }
        else
        {
            rT = Mathf.InverseLerp(from.r, to.r, value.r);
        }
        float gT;
        if (from.g == to.g && to.g == value.g)
        {
            gT = 1;
        }
        else
        {
            gT = Mathf.InverseLerp(from.g, to.g, value.g);
        }
        float bT;
        if (from.b == to.b && to.b == value.b)
        {
            bT = 1;
        }
        else
        {
            bT = Mathf.InverseLerp(from.b, to.b, value.b);
        }
        float aT;
        if (from.a == to.a && to.a == value.a)
        {
            aT = 1;
        }
        else
        {
            aT = Mathf.InverseLerp(from.a, to.a, value.a);
        }
        return rT * gT * bT * aT;
    }
}