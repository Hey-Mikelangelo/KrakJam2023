using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GuidComponent : MonoBehaviour, ICrossSceneComponent<GuidComponent>
{
    [SerializeField] private SerializableGuid serializableGuid;
    public Guid Guid => serializableGuid;

    public event Action<ICrossSceneComponent> OnDestroyed;

    private void OnValidate()
    {
    }

    protected virtual void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }
}


public class GuidComponent<T> : GuidComponent
{
    public T Value => this.GetComponent<T>();
}

