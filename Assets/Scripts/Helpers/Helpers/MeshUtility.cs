using System;
using UnityEngine;

public static class MeshUtility
{
    public static Mesh GetPrimitive(PrimitiveType primitiveType)
    {
        switch (primitiveType)
        {
            case PrimitiveType.Sphere:
                return GetCachedPrimitiveMesh(ref unitySphereMesh, primitiveType);
            case PrimitiveType.Capsule:
                return GetCachedPrimitiveMesh(ref unityCapsuleMesh, primitiveType);
            case PrimitiveType.Cylinder:
                return GetCachedPrimitiveMesh(ref unityCylinderMesh, primitiveType);
            case PrimitiveType.Cube:
                return GetCachedPrimitiveMesh(ref unityCubeMesh, primitiveType);
            case PrimitiveType.Plane:
                return GetCachedPrimitiveMesh(ref unityPlaneMesh, primitiveType);
            case PrimitiveType.Quad:
                return GetCachedPrimitiveMesh(ref unityQuadMesh, primitiveType);
            default:
                throw new ArgumentOutOfRangeException(nameof(primitiveType), primitiveType, null);
        }
    }

    private static Mesh GetCachedPrimitiveMesh(ref Mesh primMesh, PrimitiveType primitiveType)
    {
        if (primMesh == null)
        {
            Debug.Log("Getting Unity Primitive Mesh: " + primitiveType);
            primMesh = Resources.GetBuiltinResource<Mesh>(GetPrimitiveMeshPath(primitiveType));

            if (primMesh == null)
            {
                Debug.LogError("Couldn't load Unity Primitive Mesh: " + primitiveType);
            }
        }

        return primMesh;
    }

    private static string GetPrimitiveMeshPath(PrimitiveType primitiveType)
    {
        switch (primitiveType)
        {
            case PrimitiveType.Sphere:
                return "New-Sphere.fbx";
            case PrimitiveType.Capsule:
                return "New-Capsule.fbx";
            case PrimitiveType.Cylinder:
                return "New-Cylinder.fbx";
            case PrimitiveType.Cube:
                return "Cube.fbx";
            case PrimitiveType.Plane:
                return "New-Plane.fbx";
            case PrimitiveType.Quad:
                return "Quad.fbx";
            default:
                throw new ArgumentOutOfRangeException(nameof(primitiveType), primitiveType, null);
        }
    }

    private static Mesh unityCapsuleMesh = null;
    private static Mesh unityCubeMesh = null;
    private static Mesh unityCylinderMesh = null;
    private static Mesh unityPlaneMesh = null;
    private static Mesh unitySphereMesh = null;
    private static Mesh unityQuadMesh = null;
}