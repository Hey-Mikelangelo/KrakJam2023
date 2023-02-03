
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using EditorHelpers;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class EditorGuidsGenerator
{
    private static EditorSavableVariable<Dictionary<Guid, UnityEngine.Object>> guidsToObjects
       = new EditorSavableVariable<Dictionary<Guid, UnityEngine.Object>>("allGuidsWithComponents", new Dictionary<Guid, UnityEngine.Object>());
    static EditorGuidsGenerator()
    {
        Selection.selectionChanged -= SelectionChanged;
        Selection.selectionChanged += SelectionChanged;
        ClearGuidsMemory();
    }

    private static void SelectionChanged()
    {
        var gameObjects = Selection.gameObjects;
        List<ICrossSceneComponent> allCrossSceneComponents = new List<ICrossSceneComponent>(2);
        foreach (var gameObject in gameObjects)
        {
            if (AssetDatabaseExtended.IsPrefabOnDisk(gameObject))
            {
                continue;
            }
            var crossSceneComponents = gameObject.GetComponentsInChildren<ICrossSceneComponent>(true);
            allCrossSceneComponents.AddDistinct(crossSceneComponents);
        }
        foreach (var crossSceneComponent in allCrossSceneComponents)
        {
                if (AssetDatabaseExtended.IsPrefabOnDisk(crossSceneComponent.Component.gameObject))
            {
                continue;
            }
            RegisterGuid(crossSceneComponent.Guid, crossSceneComponent.Component);
        }
    }

    private static void AssemblyReloadEvents_beforeAssemblyReload()
    {
        ClearGuidsMemory();
    }
    [MenuItem("Tools/Editor Tools/Clear Editor Guids")]
    public static void ClearGuidsMemory()
    {
        guidsToObjects.Value = new Dictionary<Guid, UnityEngine.Object>();
    }
    public static Guid EnsureValidGuid(Guid currentGuid, UnityEngine.Object unityObject)
    {
        if (unityObject.IsNullWithErrorLog())
        {
            return Guid.Empty;
        }
        bool isGuidValid = currentGuid != Guid.Empty;
        if (isGuidValid == false)
        {
            return GetNewGuidAndRegister(unityObject);
        }
        else
        {
            //if copied component with guid, ensure to remove guids duplication
            if (EditorGuidsGenerator.IsGuidTaken(currentGuid, unityObject))
            {
                return GetNewGuidAndRegister(unityObject);
            }
            else
            {
                //ensure that registered. Does not make duplicate registration
                EditorGuidsGenerator.RegisterGuid(currentGuid, unityObject);
                return currentGuid;
            }
        }
    }

    public static Guid GetNewGuidAndRegister(UnityEngine.Object unityObject)
    {
        if (unityObject.IsNullWithErrorLog())
        {
            return Guid.Empty;
        }
        var newGuid = EditorGuidsGenerator.GetGuid(unityObject);
        if (newGuid == Guid.Empty)
        {
            throw new Exception("Generated guid is empty. Something wrong");
        }
        EditorGuidsGenerator.RegisterGuid(newGuid, unityObject);
        // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
        // force a save of the modified prefab instance properties
        if (PrefabUtility.IsPartOfNonAssetPrefabInstance(unityObject))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(unityObject);
        }
        return newGuid;
    }

    public static Guid GetGuid(UnityEngine.Object unityObject)
    {
        if (unityObject.IsNullWithErrorLog())
        {
            return Guid.Empty;
        }
        /*// if in editor, make sure we aren't a prefab of some kind
        if (AssetDatabaseExtended.IsPrefabOnDisk(unityObject))
        {
            return Guid.Empty;
        }*/
        // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
        // force a save of the modified prefab instance properties
        if (PrefabUtility.IsPartOfNonAssetPrefabInstance(unityObject))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(unityObject);
        }
        Guid guid;
        do
        {
            guid = Guid.NewGuid();
        }
        while (guidsToObjects.Value.ContainsKey(guid));
        return guid;
    }

    public static void RegisterGuid(Guid guid, UnityEngine.Object unityObject)
    {
        if (unityObject.IsNullWithErrorLog())
        {
            return;
        }
        if (guid == Guid.Empty)
        {
            throw new Exception("Cannot add Empty guid");
        }
        if (IsGuidTaken(guid, unityObject))
        {
            throw new Exception("Cannot add register taken guid");
        }
        if (guidsToObjects.Value.ContainsKey(guid) == false)
        {
            guidsToObjects.Value.Add(guid, unityObject);
        }
    }

    public static bool IsGuidTaken(Guid guid, UnityEngine.Object unityObject)
    {
        if (unityObject.IsNullWithErrorLog())
        {
            return true;
        }
        if (guidsToObjects.Value.TryGetValue(guid, out UnityEngine.Object foundObject))
        {
            if(foundObject == null)
            {
                return false;
            }
            return foundObject != unityObject;
        }
        return false;
    }

    
}
#endif