using UnityEngine;

public static class BoundsExtensions
{
    public static Bounds GetExpanded(this Bounds bounds, Bounds otherCube)
    {
        if(bounds.size == Vector3.zero)
        {
            return otherCube;
        }
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        Vector3 otherMin = otherCube.min;
        Vector3 otherMax = otherCube.max;
        if (otherMin.x < min.x)
        {
            min.x = otherMin.x;
        }
        if (otherMin.y < min.y)
        {
            min.y = otherMin.y;
        }
        if (otherMax.x > max.x)
        {
            max.x = otherMax.x;
        }
        if (otherMax.y > max.y)
        {
            max.y = otherMax.y;
        }
        var size = max - min;
        var center = min + (size * 0.5f);
        return new Bounds(center, size);
    }
}
