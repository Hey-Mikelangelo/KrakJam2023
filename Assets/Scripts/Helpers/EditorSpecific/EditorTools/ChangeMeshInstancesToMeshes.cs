using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using EditorHelpers;
using Sirenix.OdinInspector;

public static class ChangeMeshInstancesToMeshes
{
    private const string instanceString = " Instance";
    private const int instanceStringLength = 9;

    [MenuItem("Tools/Editor Tools/Change mesh instances to meshes")]
    private static void FixMeshFilters()
    {
        int scenesCount = EditorSceneManager.sceneCount;
        for (int i = 0; i < 1; i++)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            var meshFilters = scene.GetRootGameObjects().GetComponentsInChildren<MeshFilter>(true);
            FixMeshFilters(meshFilters);
        }
    }


    private static void FixMeshFilters(List<MeshFilter> meshFilters)
    {
        int count = meshFilters.Count;
        for (int i = 0; i < count; i++)
        {
            var meshFilter = meshFilters[i];

            FixMeshFilter(meshFilter);
        }
    }

    [Button]
    private static void FixMeshFilter(MeshFilter meshFilter)
    {
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            return;
        }
        int indexOfInstanceInName = meshFilter.sharedMesh.name.IndexOf(instanceString);
        if (indexOfInstanceInName != -1)
        {
            var prevMesh = meshFilter.sharedMesh;
            string meshName = meshFilter.sharedMesh.name.Remove(indexOfInstanceInName, instanceStringLength);
            var searchString = meshName + " t:mesh";
            var meshAssets = AssetDatabaseExtended.GetAssets(searchString);
            var matchingMeshAssets = meshAssets.Where(x => x.name == meshName && x is Mesh).Select(x => x as Mesh).ToList();
            Mesh mesh;
            if (matchingMeshAssets.Count() == 0)
            {
                if (meshName.Contains("Generated") == false)
                {
                    Debug.LogError($"GameObject {meshFilter.gameObject}. No mesh asset with name {meshName}. SearchString = {searchString}");
                    AssetDatabase.CreateAsset(prevMesh, $"Assets/{meshName}.asset");
                }

                return;
            }
            else if (matchingMeshAssets.Count() > 1)
            {
                var matchingMeshes = matchingMeshAssets.Where(x => x.GetTriangles(0).IsSameSequence(prevMesh.GetTriangles(0)));
                var matchingByParent = GetMatchingByParent(meshFilter, matchingMeshAssets, searchString);
                if (matchingMeshes.Count() > 0)
                {
                    mesh = matchingMeshes.First();
                }
                else if (matchingByParent != null)
                {
                    mesh = matchingByParent;
                    return;
                }
                else
                {
                    Debug.LogError($"GameObject {meshFilter.gameObject}. More than one asset with name {meshName}. SearchString = {searchString}. Manualy resolve");
                    return;
                }
            }
            else
            {
                mesh = matchingMeshAssets.First() as Mesh;
            }

            if (mesh != null)
            {
                meshFilter.sharedMesh = mesh;
            }
            EditorUtility.SetDirty(meshFilter);
            AssetDatabase.Refresh();
        }
    }

    private static Mesh GetMatchingByParent(MeshFilter meshFilter, List<Mesh> matchingMeshAssets, string searchString)
    {
        var meshFilterParent = meshFilter.transform.parent;
        if (meshFilterParent == null)
        {
            return null;
        }
        var parentGameObjectName = meshFilterParent.name;
        var matchingMeshParentObjects = AssetDatabaseExtended.GetAssets(searchString, includeSubAssets: false).Where(x => x.name == parentGameObjectName);
        if (matchingMeshParentObjects.Count() == 1)
        {
            var matchingParentObject = matchingMeshParentObjects.First();
            Object[] allSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(matchingParentObject));
            foreach (var meshAsset in matchingMeshAssets)
            {
                if (allSubAssets.Contains(meshAsset))
                {
                    return meshAsset;
                }
            }
        }
        return null;
    }
}
