using System;
using System.Collections.Generic;
using UnityEngine;

public struct RotationMatrix2x2Int
{
    public int a, b, c, d;

    public RotationMatrix2x2Int(Vector2Int row0, Vector2Int row1)
    {
        (a, b) = row0;
        (c, d) = row1;
    }

    public RotationMatrix2x2Int(int a, int b, int c, int d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }
    public static RotationMatrix2x2Int Identity => new RotationMatrix2x2Int(new Vector2Int(1, 0), new Vector2Int(0, 1));
    public RotationMatrix2x2Int Inverse => new RotationMatrix2x2Int(a, c, b, d);

    public Vector2Int MuliplyPoint(Vector2Int point)
    {
        int x = (point.x * a) + (point.y * b);
        int y = (point.x * c) + (point.y * d);
        return new Vector2Int(x, y);
    }
}
public struct Box
{
    public Vector3 Center;
    public Vector3 Size;
    public Quaternion Rotation;
    public Matrix4x4 RotationMatrix => Matrix4x4.Rotate(Rotation);
    public Matrix4x4 WorldToLocalMatrix => LocalToWorldMatrix.inverse;
    public Matrix4x4 LocalToWorldMatrix => Matrix4x4.TRS(Center, Rotation, Vector3.one);
    public Vector3 ExtentsLocal => Size * 0.5f;

    public Vector3 ExtentsWorld => LocalToWorldMatrix.MultiplyVector(ExtentsLocal);
    public Vector3 MinLocal => -ExtentsLocal;
    public Vector3 MaxLocal => ExtentsLocal;
    public Vector3 Min => LocalToWorldMatrix.MultiplyPoint3x4(MinLocal);
    public Vector3 Max => LocalToWorldMatrix.MultiplyPoint3x4(MaxLocal);
         

    public const int CornersCount = 8;
    private static Vector3[] cornersTemp = new Vector3[CornersCount];
    private static Vector2[] cornersMinMax2dTemp = new Vector2[CornersCount];
    private static Vector3[] cornersMinMaxTemp = new Vector3[CornersCount];

    public Box(Vector3 center, Vector3 size, Quaternion rotation)
    {
        Center = center;
        Size = size;
        Rotation = rotation;
    }

    public IReadOnlyList<Vector3> GetCornersWorldPositions()
    {
        return GetCornersWorldPositionsArray();
    }

    private Vector3[] GetCornersWorldPositionsArray()
    {
        Vector3 extents = ExtentsLocal;
        var localToWorldMatrix = LocalToWorldMatrix;
        cornersTemp[0] = localToWorldMatrix.MultiplyPoint3x4(extents);
        cornersTemp[1] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));
        cornersTemp[2] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
        cornersTemp[3] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
        cornersTemp[4] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
        cornersTemp[5] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));
        cornersTemp[6] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
        cornersTemp[7] = localToWorldMatrix.MultiplyPoint3x4(-extents);
        return cornersTemp;
    }

    public (Vector3 min, Vector3 max) GetMinMaxLocalInOther(Matrix4x4 worldToLocalMatrix)
    {
        var corners = GetCornersWorldPositions();

        for (int i = 0; i < CornersCount; i++)
        {
            cornersMinMaxTemp[i] = worldToLocalMatrix.MultiplyPoint(corners[i]);
        }
        var (min, max) = MathUtils.GetMinMax(cornersMinMaxTemp);
        return (min, max);
    }

    public (Vector2 min, Vector2 max) GetMinMax2dLocalInOther(Matrix4x4 worldToLocalMatrix, Axis flatAxis)
    {
        var corners = GetCornersWorldPositions();

        for (int i = 0; i < CornersCount; i++)
        {
            cornersMinMax2dTemp[i] = worldToLocalMatrix.MultiplyPoint(corners[i]).GetVector2WithRemovedValueOnAxis(flatAxis);
        }
        var (min, max) = MathUtils.GetMinMax(cornersMinMax2dTemp);
        return (min, max);
    }

    public Rect ToRect(Matrix4x4 worldToLocalMatrix, Axis flatAxis)
    {
        var (min, max) = GetMinMax2dLocalInOther(worldToLocalMatrix, flatAxis);

        Vector2 boxSizeInGrid = max - min;
        return new Rect(min, boxSizeInGrid);
    }


    public (Vector3 p1, Vector3 p2, float radius) GetFittingCapsule(Axis layAxis = Axis.None)
    {
        float radius = Size.MinComponent() * 0.5f;
        if(layAxis == Axis.None)
        {
            layAxis = Size.GetMaxValueAxis();
        }
        var extentsWithRadius = ExtentsLocal.WithValueOnAxis(layAxis, radius);
        Vector3 side1Center = MinLocal + extentsWithRadius;
        Vector3 side2Center = MaxLocal - extentsWithRadius;
        var localToWorlMatrix = LocalToWorldMatrix;
        return (localToWorlMatrix.MultiplyPoint(side1Center), localToWorlMatrix.MultiplyPoint(side2Center), radius);
    }


    public void Encapsulate(Box otherCube)
    {
        if (otherCube.Size == Vector3.zero)
        {
            return;
        }
        Vector3 minLocal = MinLocal;
        Vector3 maxLocal = MaxLocal;
        var worldToLocalMatrix = this.WorldToLocalMatrix;
        var (otherMinLocal, otherMaxLocal) = otherCube.GetMinMaxLocalInOther(worldToLocalMatrix);

        if (otherMinLocal.x < minLocal.x)
        {
            minLocal.x = otherMinLocal.x;
        }
        if (otherMinLocal.y < minLocal.y)
        {
            minLocal.y = otherMinLocal.y;
        }
        if (otherMinLocal.z < minLocal.z)
        {
            minLocal.z = otherMinLocal.z;
        }
        if (otherMaxLocal.x > maxLocal.x)
        {
            maxLocal.x = otherMaxLocal.x;
        }
        if (otherMaxLocal.y > maxLocal.y)
        {
            maxLocal.y = otherMaxLocal.y;
        }
        if (otherMaxLocal.z > maxLocal.z)
        {
            maxLocal.z = otherMaxLocal.z;
        }
        Size = maxLocal - minLocal;
        var centerLocal = minLocal + (Size * 0.5f);
        Center = this.LocalToWorldMatrix.MultiplyPoint(centerLocal);
    }
}