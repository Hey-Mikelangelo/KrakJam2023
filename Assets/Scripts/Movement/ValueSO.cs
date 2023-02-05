using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class ValueSO<T> : ScriptableObject
{
    [SerializeField] private SerializableGuid guid;
    private static Dictionary<Guid, ValueSO<T>> guidToInstance = new();
    [ShowInInspector, ReadOnly]
    public T Value
    {
        get => GetInstance().value;
        set
        {
            var instance = GetInstance();
            if (instance.value != null && instance.value.Equals(value))
            {
                return;
            }
            instance.value = value;
            instance.OnValueChanged?.Invoke();
        }
    }

    public ValueSO<T> GetInstance()
    {
        if(guidToInstance.TryGetValue(guid, out var valueSO))
        {
            return valueSO;
        }
        guidToInstance.Add(guid, this);
        return this;
    }
    private T value;
    public event System.Action OnValueChanged;
}
