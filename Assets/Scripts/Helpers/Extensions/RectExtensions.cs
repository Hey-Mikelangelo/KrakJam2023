using System;
using UnityEngine;
[System.Flags]
public enum Directions
{
    None = 0,
    Top = 1,
    Bottom = 1 << 1,
    Left = 1 << 2,
    Right = 1 <<3
}
public static class RectIntExtensions
{
    public static RectInt CropToFitRect(this RectInt rect, RectInt otherRect)
    {
        int xMin = rect.min.x;
        int yMin = rect.min.y;
        int xMax = rect.max.x;
        int yMax = rect.max.y;
        if (rect.min.x < otherRect.min.x)
        {
            xMin = otherRect.min.x;
        }
        if (rect.min.y < otherRect.min.y)
        {
            yMin = otherRect.min.y;
        }
        if (rect.max.x > otherRect.max.x)
        {
            xMax = otherRect.max.x;
        }
        if (rect.max.y > otherRect.max.y)
        {
            yMax = otherRect.max.y;
        }
        return new RectInt(xMin, yMin, (xMax - xMin), (yMax - yMin));
    }

    public static (Vector2Int pointPositionInsideRect, Directions pointOffsetDirections) MovePointToInside(this RectInt rectInt, Vector2Int point)
    {
        Directions directions = Directions.None;
        if (point.x < rectInt.min.x)
        {
            point.x = rectInt.min.x;
            directions |= Directions.Left;
        }
        else if(point.x > rectInt.max.x - 1)
        {
            point.x = rectInt.max.x - 1;
            directions |= Directions.Right;
        }
        if (point.y < rectInt.min.y)
        {
            point.y = rectInt.min.y;
            directions |= Directions.Bottom;
        }
        else if (point.y > rectInt.max.y - 1)
        {
            point.y = rectInt.max.y - 1;
            directions |= Directions.Top;

        }
        return (point, directions);
    }

    public static void Deconstruct(this RectInt rectInt, out Vector2Int min, out Vector2Int size)
    {
        min = rectInt.min;
        size = rectInt.size;
    }

    public static bool Contains(this RectInt largerRect, RectInt smallerRect)
    {
        if (smallerRect.min.x < largerRect.min.x 
            || smallerRect.min.y < largerRect.min.y 
            || smallerRect.max.x > largerRect.max.x 
            || smallerRect.max.y > largerRect.max.y)
        {
            return false;
        }
        return true;
    }

    public static RectInt GetCenterSizeOneRect(this RectInt rectInt)
    {
        Vector2Int halfSize = new Vector2Int(
            Mathf.FloorToInt(((float)rectInt.size.x * 0.5f)), 
            Mathf.FloorToInt(((float)rectInt.size.y * 0.5f)));
        return new RectInt(rectInt.min + halfSize, Vector2Int.one);
    }
    public static RectInt GetExpanded(this RectInt rectInt, int expandAmount)
    {
        Vector2Int expandSize = new Vector2Int(expandAmount, expandAmount);
        Vector2Int newMin = rectInt.min - expandSize;
        Vector2Int newMax = rectInt.max + expandSize;
        return new RectInt(newMin, newMax - newMin);
    }

    public static RectInt WithOffset(this RectInt rectInt, Vector2Int offset)
    {
        return new RectInt(rectInt.min + offset, rectInt.size);
    }

    public static bool IsAlignedTo(this RectInt rect, RectInt target)
    {
        if(rect.min.x == target.min.x && rect.max.x == target.max.x)
        {
            return true;
        }
        if(rect.min.y == target.min.y && rect.max.y == target.max.y)
        {
            return true;
        }
        return false;
    }
    public static bool IsAlignedAndTouching(this RectInt rect, RectInt target)
    {
        if (rect.min.x == target.min.x && rect.max.x == target.max.x)
        {
            if(target.max.y == rect.min.y || target.min.y == rect.max.y)
            {
                return true;
            }
        }
        if (rect.min.y == target.min.y && rect.max.y == target.max.y)
        {
            if (target.max.x == rect.min.x || target.min.x == rect.max.x)
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Returns position side of <paramref name="other"/> rect relative to <paramref name="rect"/>. So if returned value = (Top, Right) it means
    /// that <paramref name="other"/> is in top right corner of <paramref name="rect"/>
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static RectSide GetRectSide(this RectInt rect, RectInt other)
    {
        RectSide rectSide = RectSide.None;
        if(other.min.y >= rect.max.y)
        {
            rectSide |= RectSide.Top;
        }
        else if(other.max.y <= rect.min.y)
        {
            rectSide |= RectSide.Bottom;
        }
        if(other.min.x >= rect.max.x)
        {
            rectSide |= RectSide.Right;
        }
        else if(other.max.x <= rect.min.x)
        {
            rectSide |= RectSide.Left;
        }
        return rectSide;
    }
    public static void ExpandToFit(this ref RectInt rectInt, RectInt other)
    {
        rectInt = rectInt.GetExpandedToFit(other);
    }
    public static RectInt GetExpandedToFit(this RectInt rectInt, RectInt other)
    {
        int minX = rectInt.min.x;
        int minY = rectInt.min.y;
        int maxX = rectInt.max.x;
        int maxY = rectInt.max.y;
        if (other.min.x < minX)
        {
            minX = other.min.x;
        }
        if(other.min.y < minY)
        {
            minY = other.min.y;
        }
        if(other.max.x > maxX)
        {
            maxX = other.max.x;
        }
        if(other.max.y > maxY)
        {
            maxY = other.max.y;
        }
        return new RectInt(minX, minY, maxX - minX, maxY - minY);
    }

    public static (int startX, int startY, int endX, int endY) GetIterationRange(this RectInt areaRect)
    {
        var startX = areaRect.min.x;
        var startY = areaRect.min.y;
        var endX = startX + areaRect.size.x;
        var endY = startY + areaRect.size.y;
        return (startX, startY, endX, endY);
    }
}

[Flags]
public enum RectSide
{
    None = 0,
    Top = 1,
    Bottom = 1 << 1,
    Left = 1 << 2, 
    Right = 1 << 3
}
public enum Rotation90
{
    Zero = 0,
    Rotation90,
    Rotation180,
    Rotation270
}

public static class RectExtensions
{

    /// <summary>
    /// Sets rect center.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="center">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithCenter(this Rect rect, Vector2 center)
    {
        rect.center = center;
        return rect;
    }

    /// <summary>
    /// Sets rect position.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="position">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithPosition(this Rect rect, Vector2 position)
    {
        rect.position = position;
        return rect;
    }

    /// <summary>
    /// Sets rect height.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="height">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithHeight(this Rect rect, float height)
    {
        rect.height = height;
        return rect;
    }

    /// <summary>
    /// Sets rect width.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="width">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithWidth(this Rect rect, float width)
    {
        rect.width = width;
        return rect;
    }

    /// <summary>
    /// Sets rect max point.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="max">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithMax(this Rect rect, Vector2 max)
    {
        rect.max = max;
        return rect;
    }

    /// <summary>
    /// Sets rect min point.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="min">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithMin(this Rect rect, Vector2 min)
    {
        rect.min = min;
        return rect;
    }

    /// <summary>
    /// Sets rect size.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="size">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithSize(this Rect rect, Vector2 size)
    {
        rect.size = size;
        return rect;
    }

    /// <summary>
    /// Sets rect x position.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="x">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithX(this Rect rect, float x)
    {
        rect.x = x;
        return rect;
    }

    /// <summary>
    /// Sets rect x position.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="y">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithY(this Rect rect, float y)
    {
        rect.y = y;
        return rect;
    }

    /// <summary>
    /// Sets rect x max position.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="xMax">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithXMax(this Rect rect, float xMax)
    {
        rect.xMax = xMax;
        return rect;
    }

    /// <summary>
    /// Sets rect x min position.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="xMin">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithXMin(this Rect rect, float xMin)
    {
        rect.xMin = xMin;
        return rect;
    }

    /// <summary>
    /// Sets rect y max position.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="yMax">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithYMax(this Rect rect, float yMax)
    {
        rect.yMax = yMax;
        return rect;
    }

    /// <summary>
    /// Sets rect y min position.
    /// </summary>
    /// <param name="rect">Target rect.</param>
    /// <param name="yMin">Value to set.</param>
    /// <returns>Changed copy ot the <paramref name="rect"/></returns>
    public static Rect WithYMin(this Rect rect, float yMin)
    {
        rect.yMin = yMin;
        return rect;
    }

    public static Vector3 GetOffsetToMoveInsideOtherRect(this Rect insideRect, Rect outsideRect, Vector2 offsetFromBorder)
    {
        float xOffset = 0, yOffset = 0;
        if(insideRect.min.x < outsideRect.min.x)
        {
            xOffset = (outsideRect.min.x - insideRect.min.x) + offsetFromBorder.x;
        }
        else if (insideRect.max.x > outsideRect.max.x)
        {
            xOffset = outsideRect.max.x - insideRect.max.x - offsetFromBorder.x;
        }
        if (insideRect.min.y < outsideRect.min.y)
        {
            yOffset = outsideRect.min.y - insideRect.min.y + offsetFromBorder.y;
        }
        else if (insideRect.max.y > outsideRect.max.y)
        {
            yOffset = outsideRect.max.y - insideRect.max.y - offsetFromBorder.y;
        }
        return new Vector2(xOffset, yOffset);
    }
    /// <summary>
    /// returns offset to snap <paramref name="insideRect"/> to the <paramref name="outsideRect"/> by the nearest corner
    /// </summary>
    /// <param name="outsideRect"></param>
    /// <param name="insideRect"></param>
    /// <returns></returns>
    public static Vector2 GetOffsetToSnapToNearestCorner(this Rect outsideRect, Rect insideRect)
    {
        float minCornersDistance = Vector2.Distance(outsideRect.min, insideRect.min);

        float maxCornersDistance = Vector2.Distance(outsideRect.max, insideRect.max);

        Vector2 outsideRectMinXMaxY = new Vector2(outsideRect.min.x, outsideRect.max.y);
        Vector2 insideRectMinXMaxY = new Vector2(insideRect.min.x, insideRect.max.y);
        float minXMaxYCornersDistance = Vector2.Distance(outsideRectMinXMaxY, insideRectMinXMaxY);

        Vector2 outsideRectMaxXMinY = new Vector2(outsideRect.max.x, outsideRect.min.y);
        Vector2 insideRectMaxXMinY = new Vector2(insideRect.max.x, insideRect.min.y);
        float maxXminYCornersDistance = Vector2.Distance(outsideRectMaxXMinY, insideRectMaxXMinY);

        System.Span<float> distancesToCorners = stackalloc float[4];
        distancesToCorners[0] = minCornersDistance;
        distancesToCorners[1] = maxCornersDistance;
        distancesToCorners[2] = minXMaxYCornersDistance;
        distancesToCorners[3] = maxXminYCornersDistance;

        int minDistanceIndex = -1;
        float minDistance = float.MaxValue;
        for (int i = 0; i < 4; i++)
        {
            if (distancesToCorners[i] < minDistance)
            {
                minDistance = distancesToCorners[i];
                minDistanceIndex = i;
            }
        }
        if (minDistanceIndex == 0)
        {
            return outsideRect.min - insideRect.min;
        }
        else if (minDistanceIndex == 1)
        {
            return outsideRect.max - insideRect.max;
        }
        else if (minDistanceIndex == 2)
        {
            return outsideRectMinXMaxY - insideRectMinXMaxY;
        }
        else
        {
            return outsideRectMaxXMinY - insideRectMaxXMinY;
        }
    }

    public static Vector2 GetOffsetToSnapToFarCorner(this Rect outsideRect, Rect insideRect)
    {
        float minCornersDistance = Vector2.Distance(outsideRect.min, insideRect.min);

        float maxCornersDistance = Vector2.Distance(outsideRect.max, insideRect.max);

        Vector2 outsideRectMinXMaxY = new Vector2(outsideRect.min.x, outsideRect.max.y);
        Vector2 insideRectMinXMaxY = new Vector2(insideRect.min.x, insideRect.max.y);
        float minXMaxYCornersDistance = Vector2.Distance(outsideRectMinXMaxY, insideRectMinXMaxY);

        Vector2 outsideRectMaxXMinY = new Vector2(outsideRect.max.x, outsideRect.min.y);
        Vector2 insideRectMaxXMinY = new Vector2(insideRect.max.x, insideRect.min.y);
        float maxXminYCornersDistance = Vector2.Distance(outsideRectMaxXMinY, insideRectMaxXMinY);

        System.Span<float> distancesToCorners = stackalloc float[4];
        distancesToCorners[0] = minCornersDistance;
        distancesToCorners[1] = maxCornersDistance;
        distancesToCorners[2] = minXMaxYCornersDistance;
        distancesToCorners[3] = maxXminYCornersDistance;

        int minDistanceIndex = -1;
        float maxDistance = float.MinValue;
        for (int i = 0; i < 4; i++)
        {
            if (distancesToCorners[i] > maxDistance)
            {
                maxDistance = distancesToCorners[i];
                minDistanceIndex = i;
            }
        }
        if (minDistanceIndex == 0)
        {
            return outsideRect.min - insideRect.min;
        }
        else if (minDistanceIndex == 1)
        {
            return outsideRect.max - insideRect.max;
        }
        else if (minDistanceIndex == 2)
        {
            return outsideRectMinXMaxY - insideRectMinXMaxY;
        }
        else
        {
            return outsideRectMaxXMinY - insideRectMaxXMinY;
        }
    }

    public static bool CanFitInsideOtherRect(this Rect rect, Rect other)
    {
        return rect.size.x <= other.size.x && rect.size.y <= other.size.y;
    }

    public static bool IsFullyInsideOtherRect(this Rect rect, Rect other)
    {
        return rect.min.x >= other.min.x && rect.max.x <= other.max.x && rect.min.y >= other.min.y && rect.max.y <= other.max.y;
    }
   /* public static Rect ToRect(this RectInt rectInt)
    {
        Vector2 cellsAreaMinCornerGrid = Grid2dUtils.GetCellPositionFromGridCellIndex(cellsAreaRectInt.min, grid.Grid2d);
        Vector2 cellsAreaMaxCornerGrid = Grid2dUtils.GetCellPositionFromGridCellIndex(cellsAreaRectInt.max - Vector2Int.one, grid.Grid2d)
            + new Vector2(cellSize, cellSize);
        Rect cellsAreaRect = new Rect(
            cellsAreaMinCornerGrid *//*- new Vector2(cellMargin, cellMargin)*//*,
            (cellsAreaMaxCornerGrid - cellsAreaMinCornerGrid) *//*+ new Vector2(cellMargin, cellMargin)*//*);
    }*/
}