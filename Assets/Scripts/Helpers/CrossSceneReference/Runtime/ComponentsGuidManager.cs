using System.Collections.Generic;
using UnityEngine;
using System;
// Class to handle registering and accessing objects by GUID
public class ComponentsGuidManager
{
    // for each GUID we need to know the Game Object it references
    // and an event to store all the callbacks that need to know when it is destroyed
    private struct GuidInfo
    {
        public Component component;

        public event Action<Component> OnAdd;
        public event Action OnRemove;
        public GuidInfo(Component comp)
        {
            component = comp;
            OnRemove = null;
            OnAdd = null;
        }

        public void HandleAddCallback()
        {
            if (OnAdd != null)
            {
                OnAdd(component);
            }
        }

        public void HandleRemoveCallback()
        {
            if (OnRemove != null)
            {
                OnRemove();
            }
        }
    }
    public static event Action OnCreatedInstance;

    // Singleton interface
    private static ComponentsGuidManager Instance;

    // All the public API is static so you need not worry about creating an instance
    public static bool Add(Guid guid, Component component)
    {
        EnsureInstanceCreated();

        return Instance.InternalAdd(guid, component);
    }

    public static void Remove(System.Guid guid)
    {
        EnsureInstanceCreated();

        Instance.InternalRemove(guid);
    }

    public static Component ResolveGuid(System.Guid guid, Action<Component> onAddCallback, Action onRemoveCallback)
    {
        EnsureInstanceCreated();

        return Instance.ResolveGuidInternal(guid, onAddCallback, onRemoveCallback);
    }

    public static bool ContainsGuid(Guid guid)
    {
        return Instance.guidToObjectMap.ContainsKey(guid);
    }

    public static Component ResolveGuid(System.Guid guid, Action onDestroyCallback)
    {
        EnsureInstanceCreated();

        return Instance.ResolveGuidInternal(guid, null, onDestroyCallback);
    }

    public static Component ResolveGuid(System.Guid guid)
    {
        EnsureInstanceCreated();
        return Instance.ResolveGuidInternal(guid, null, null);
    }

    // instance data
    private Dictionary<System.Guid, GuidInfo> guidToObjectMap;

    private ComponentsGuidManager()
    {
        guidToObjectMap = new Dictionary<System.Guid, GuidInfo>();
    }

    private bool InternalAdd(Guid guid, Component component)
    {
        GuidInfo info = new GuidInfo(component);
        if(component == null)
        {
            Debug.LogError($"Cannot add null component");
            return false;
        }
        else if (guid == Guid.Empty)
        {
            Debug.LogError($"Cannot add empty guid for component {component.GetType()} on GameObject {component.gameObject}");
            return false;
        }

        if (!guidToObjectMap.ContainsKey(guid))
        {
            guidToObjectMap.Add(guid, info);
            return true;
        }

        GuidInfo existingInfo = guidToObjectMap[guid];
        if (existingInfo.component != null && existingInfo.component != component)
        {
            // normally, a duplicate GUID is a big problem, means you won't necessarily be referencing what you expect
            if (Application.isPlaying)
            {
                Debug.AssertFormat(false, component.gameObject, "Guid Collision Detected between {0} and {1}.\nAssigning new Guid. Consider tracking runtime instances using a direct reference or other method.", (guidToObjectMap[guid].component != null ? guidToObjectMap[guid].component.gameObject.name : "NULL"), (component != null ? component.gameObject.name : "NULL"));
            }
            else
            {
                // however, at editor time, copying an object with a GUID will duplicate the GUID resulting in a collision and repair.
                // we warn about this just for pedantry reasons, and so you can detect if you are unexpectedly copying these components
                Debug.LogWarningFormat(component.gameObject, "Guid Collision Detected while creating {0}.\nAssigning new Guid.", (component != null ? component.gameObject.name : "NULL"));
            }
            return false;
        }
        // if we already tried to find this GUID, but haven't set the game object to anything specific, copy any OnAdd callbacks then call them
        existingInfo.component = info.component;
        existingInfo.HandleAddCallback();
        guidToObjectMap[guid] = existingInfo;
        return true;
    }

    private void InternalRemove(System.Guid guid)
    {
        GuidInfo info;
        if (guidToObjectMap.TryGetValue(guid, out info))
        {
            // trigger all the destroy delegates that have registered
            info.HandleRemoveCallback();
        }

        guidToObjectMap.Remove(guid);
    }

    // nice easy api to find a GUID, and if it works, register an on destroy callback
    // this should be used to register functions to cleanup any data you cache on finding
    // your target. Otherwise, you might keep components in memory by referencing them
    private Component ResolveGuidInternal(System.Guid guid, Action<Component> onAddCallback, Action onRemoveCallback)
    {
        GuidInfo info = default;
        if (guid != Guid.Empty && guidToObjectMap.TryGetValue(guid, out info))
        {
            if (onAddCallback != null)
            {
                info.OnAdd += onAddCallback;
            }

            if (onRemoveCallback != null)
            {
                info.OnRemove += onRemoveCallback;
            }
            guidToObjectMap[guid] = info;
            return info.component;
        }

        if (onAddCallback != null)
        {
            info.OnAdd += onAddCallback;
        }

        if (onRemoveCallback != null)
        {
            info.OnRemove += onRemoveCallback;
        }

        if(guid != Guid.Empty)
        {
            guidToObjectMap.Add(guid, info);
        }

        return null;
    }

    private static void EnsureInstanceCreated()
    {
        if (Instance == null)
        {
            Instance = new ComponentsGuidManager();
            OnCreatedInstance?.Invoke();
        }
#if UNITY_EDITOR
        if(Application.isPlaying == false)
        {
            var instances = CrossSceneReferencesManager.GetAllInstances();
            for (int i = 0; i < instances.Count; i++)
            {
                if(instances[i] != null)
                {
                    instances[i].RefreshCrossSceneObjectsArray();
                }
            }
        }
#endif   
    }
}
