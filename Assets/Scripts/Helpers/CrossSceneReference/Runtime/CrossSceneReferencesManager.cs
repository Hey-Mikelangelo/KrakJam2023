using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using UnityEngine;
using EditorHelpers;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
#endif
public class CrossSceneReferencesManager : SceneSingleton<CrossSceneReferencesManager>
{
    [SerializeField, ReadOnly] private Component[] referencedComponents = new Component[0];
    private bool loadedReferences;
#if UNITY_EDITOR
    static CrossSceneReferencesManager()
    {
        EditorSceneManager.sceneSaving -= BeforeSceneSave;
        EditorSceneManager.sceneSaving += BeforeSceneSave;
        ComponentsGuidManager.OnCreatedInstance -= ComponentsGuidManager_OnCreatedInstance;
        ComponentsGuidManager.OnCreatedInstance += ComponentsGuidManager_OnCreatedInstance;
        EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
    }

    private static void EditorApplication_playModeStateChanged(PlayModeStateChange playModeStateChange)
    {
        var instances = GetAllInstances();
        if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
        {
            foreach (var instance in instances)
            {
                instance.loadedReferences = false;
            }
        }
        
    }

    private static void ComponentsGuidManager_OnCreatedInstance()
    {
        if (Application.isPlaying)
        {
            return;
        }
        var instances = new List<CrossSceneReferencesManager>(GetAllInstances());
        foreach (var instance in instances)
        {
            instance.loadedReferences = false;
            instance.LoadSceneReferences();

        }
    }

    private static void BeforeSceneSave(Scene scene, string path)
    {
        if (Application.isPlaying)
        {
            return;
        }
        var instances = GetAllInstances();
        foreach (var item in instances)
        {
            if (item != null && item.gameObject.scene == scene)
            {
                item.RefreshCrossSceneObjectsArray();
                return;
            }
        }
    }
#endif
    public override void OnInit()
    {
        base.OnInit();
        LoadSceneReferences();
    }

    private void LoadSceneReferences(bool forceLoad = false)
    {
        if (loadedReferences && forceLoad == false)
        {
            return;
        }
        loadedReferences = true;
        int count = referencedComponents.Length;
        for (int i = 0; i < count; i++)
        {
            Component crossSceneObjectComponent = referencedComponents[i];
            AddCrossSceneObject(crossSceneObjectComponent);

            //Debug.Log($"Added {crossSceneObjectComponent.gameObject} {crossSceneObjectComponent.GetType()}");
        }
    }

    public override void OnBeforeUnloaded()
    {
        base.OnBeforeUnloaded();
        RemoveThisSceneObjects();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        RemoveThisSceneObjects();
    }
    private void RemoveThisSceneObjects()
    {
        int count = referencedComponents.Length;
        for (int i = 0; i < count; i++)
        {
            Component crossSceneObjectComponent = referencedComponents[i];
            if (crossSceneObjectComponent == null)
            {
                continue;
            }
            ICrossSceneComponent crossSceneObject = crossSceneObjectComponent as ICrossSceneComponent;
            RemoveCrossSceneObject(crossSceneObject);
            //Debug.Log($"Removed {crossSceneObjectComponent.gameObject} {crossSceneObjectComponent.GetType()}");
        }
    }

    private void AddCrossSceneObject(Component crossSceneObjectComponent)
    {
        ICrossSceneComponent crossSceneObject = crossSceneObjectComponent as ICrossSceneComponent;
        if(crossSceneObject == null)
        {
            return;
        }
        ComponentsGuidManager.Add(crossSceneObject.Guid, crossSceneObjectComponent);
        crossSceneObject.OnDestroyed += RemoveCrossSceneObject;
    }

    private void RemoveCrossSceneObject(ICrossSceneComponent crossSceneObject)
    {
        crossSceneObject.OnDestroyed -= RemoveCrossSceneObject;
        ComponentsGuidManager.Remove(crossSceneObject.Guid);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(CoroutineUtils.InvokeActionWaitAllScenesLoadedEditor(RefreshCrossSceneObjectsArray));
    }

    [Button]
    private void Refresh()
    {
        referencedComponents = (gameObject.scene.GetRootGameObjects().GetComponentsInChildren<ICrossSceneComponent>(true)
            .Select(x => x as Component).ToArray());
        LoadSceneReferences(true);
    }
    
    public void RefreshCrossSceneObjectsArray()
    {
        try
        {
            if (loadedReferences || gameObject == null)
            {
                return;
            }
            Refresh();
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
    }

#endif
}
