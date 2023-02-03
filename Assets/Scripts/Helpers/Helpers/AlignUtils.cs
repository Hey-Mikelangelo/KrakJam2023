using UnityEngine;
public enum LocalAlignPosition
{
    None = 0,
    TopLeft = 2,
    TopCenter = 3,
    TopRight = 4,
    MiddleLeft = 5,
    Center = 6,
    MiddleRight = 7,
    BottomLeft = 8,
    BottomCenter = 9,
    BottomRight = 10
}
public static class AlignUtils
{

    /// <summary>
    /// Calculates offset assuming that <paramref name="innerBoxSize"/> is in bottom left corner of <paramref name="outerBoxSize"/>
    /// </summary>
    /// <param name="alignPosition"></param>
    /// <param name="outerBoxSize"></param>
    /// <param name="innerBoxSize"></param>
    /// <returns></returns>
    public static Vector2 GetAlignOffset(LocalAlignPosition alignPosition, Vector2 outerBoxSize, Vector2 innerBoxSize)
    {
        switch (alignPosition)
        {
            case LocalAlignPosition.Center:
                return (outerBoxSize - innerBoxSize) * 0.5f;
            case LocalAlignPosition.TopLeft:
                return new Vector2(0, outerBoxSize.y - innerBoxSize.y);
            case LocalAlignPosition.TopCenter:
                return new Vector2((outerBoxSize.x - innerBoxSize.x) * 0.5f, outerBoxSize.y - innerBoxSize.y);
            case LocalAlignPosition.TopRight:
                return new Vector2(outerBoxSize.x - innerBoxSize.x, outerBoxSize.y - innerBoxSize.y);
            case LocalAlignPosition.MiddleLeft:
                return new Vector2(0, (outerBoxSize.y - innerBoxSize.y) * 0.5f);
            case LocalAlignPosition.MiddleRight:
                return new Vector2(outerBoxSize.x - innerBoxSize.x, (outerBoxSize.y - innerBoxSize.y) * 0.5f);
            case LocalAlignPosition.BottomLeft:
                return new Vector2(0, 0);
            case LocalAlignPosition.BottomCenter:
                return new Vector2((outerBoxSize.x - innerBoxSize.x) * 0.5f, 0);
            case LocalAlignPosition.BottomRight:
                return new Vector2(outerBoxSize.x - innerBoxSize.x, 0);
            default:
                throw new System.NotImplementedException($"Not implemented case for position {alignPosition}");
        }
    }

}
