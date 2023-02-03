using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using EditorHelpers;
using Sirenix.OdinInspector;

public static class ChangeMaterialInstancesToMaterials
{
    private const string instanceString = " (Instance)";
    private const int instanceStringLength = 11;

    [MenuItem("Tools/Editor Tools/Change material instances to materials")]
    private static void FixMaterials()
    {
        int scenesCount = EditorSceneManager.sceneCount;
        for (int i = 0; i < 1; i++)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            var renderers = scene.GetRootGameObjects().GetComponentsInChildren<Renderer>(true);
            FixRenders(renderers);
        }
    }


    private static void FixRenders(List<Renderer> renderers)
    {
        int count = renderers.Count;
        for (int i = 0; i < count; i++)
        {
            var renderer = renderers[i];

            FixRenderer(renderer);
        }
    }

    [Button]
    private static void FixRenderer(Renderer renderer)
    {
        if (renderer == null || renderer.sharedMaterials == null || renderer.sharedMaterials.Count() == 0)
        {
            return;
        }
        int materialsCount = renderer.sharedMaterials.Count();
        Material[] newMaterials = new Material[materialsCount];
        System.Array.Copy(renderer.sharedMaterials, newMaterials, materialsCount);
        int index = -1;
        foreach (var material in renderer.sharedMaterials)
        {
            index++;
            if (material == null)
            {
                continue;
            }
            int indexOfInstanceInName = material.name.IndexOf(instanceString);
            if (indexOfInstanceInName != -1)
            {
                var prevMaterial = material;
                string materialName = material.name.Remove(indexOfInstanceInName, instanceStringLength);
                var searchString = materialName + " t:material";
                var materialAssets = AssetDatabaseExtended.GetAssets(searchString);
                var matchingMaterialAssets = materialAssets.Where(x => x.name == materialName && x is Material).Select(x => x as Material).ToList();
                Material originalMaterial;
                if (matchingMaterialAssets.Count() == 0)
                {
                    Debug.LogError($"GameObject {renderer.gameObject}. No material asset with name {materialName}. SearchString = {searchString}");
                    return;
                }
                else if (matchingMaterialAssets.Count() > 1)
                {
                    var matchingMeshes = matchingMaterialAssets.Where(x => x.Equals(prevMaterial));
                    var matchingByParent = GetMatchingByParent(renderer, matchingMaterialAssets, searchString);
                    if (matchingMeshes.Count() > 0)
                    {
                        originalMaterial = matchingMeshes.First();
                    }
                    else if (matchingByParent != null)
                    {
                        originalMaterial = matchingByParent;
                        return;
                    }
                    else
                    {
                        Debug.LogError($"GameObject {renderer.gameObject}. More than one asset with name {materialName}. SearchString = {searchString}. Manualy resolve");
                        return;
                    }
                }
                else
                {
                    originalMaterial = matchingMaterialAssets.First();
                }

                if (originalMaterial != null)
                {
                    newMaterials[index] = originalMaterial;
                    EditorUtility.SetDirty(renderer);
                }
            }
        }
        renderer.sharedMaterials = newMaterials;
    }

    private static Material GetMatchingByParent(Renderer renderer, List<Material> matchingMaterialAssets, string searchString)
    {
        var parent = renderer.transform.parent;
        if (parent == null)
        {
            return null;
        }
        var parentGameObjectName = parent.name;
        var matchingmaMaterialParentObjects = AssetDatabaseExtended.GetAssets(searchString, includeSubAssets: false).Where(x => x.name == parentGameObjectName);
        if (matchingmaMaterialParentObjects.Count() == 1)
        {
            var matchingParentObject = matchingmaMaterialParentObjects.First();
            Object[] allSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(matchingParentObject));
            foreach (var meshAsset in matchingMaterialAssets)
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
