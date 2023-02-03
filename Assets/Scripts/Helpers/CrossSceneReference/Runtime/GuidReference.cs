using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

#endif

[System.Serializable]
public abstract class GuidReference
{
    [SerializeField] private SerializableGuid serializableGuid;
    [SerializeField] protected bool useSimpleReference = true;
#if UNITY_EDITOR
    // decorate with some extra info in Editor so we can inform a user of what that GUID means
    [SerializeField] private string cachedName;
    [SerializeField] private SceneAsset cachedScene;

#endif
    public Guid Guid => serializableGuid;
    private bool isCacheSet;
    public abstract void ClearCache();

}

public abstract class GuidReference<T> : GuidReference where T : Component
{
    [SerializeField] protected T componentReference;
    private T cachedReference;

    private T cachedComponent;

    public void SetSimpleReferenceComponent(T component)
    {
        componentReference = component;
        useSimpleReference = true;
    }
    public T Component
    {
        get
        {
            if(cachedComponent != null /*&& Guid != Guid.Empty*/)
            {   
                return cachedComponent;
            }
            if (useSimpleReference)
            {
                cachedComponent = componentReference;
            }
            else
            {
                var resolvedComponent = ComponentsGuidManager.ResolveGuid(this.Guid);
                if(resolvedComponent == null)
                {
                    return null;
                }
                if (typeof(ICrossSceneComponent).IsAssignableFrom(typeof(T)) && resolvedComponent is T component)
                {
                    cachedComponent = component;
                }
                else
                {
                    cachedComponent = resolvedComponent.GetComponent<T>();
                }
            }
            return cachedComponent;
        }
    }

    public sealed override void ClearCache()
    {
        cachedComponent = null;
    }

    public static implicit operator T (GuidReference<T> guidReference)
    {
        return guidReference.Component;
    }
}


public interface ICrossSceneComponent 
{
    public System.Guid Guid { get; }
    public event System.Action<ICrossSceneComponent> OnDestroyed;

    public Component Component => ((Component)this);
}
public interface ICrossSceneComponent<T> : ICrossSceneComponent where T : Component
{
    
}

