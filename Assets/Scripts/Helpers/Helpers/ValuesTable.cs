using System.Collections.Generic;
using UnityEngine;

public abstract class ValuesTable<T,TableT> : ScriptableSingleton<TableT> where TableT : ValuesTable<T, TableT>
{
    [SerializeField] private List<T> values = new List<T>();

    public IReadOnlyList<T> Values => values;
    public bool HasValue(T value)
    {
        return values.Contains(value);
    }
    public bool RemoveValue(T value)
    {
        return values.Remove(value);
    }
    public bool AddValue(T value)
    {
        if (HasValue(value))
        {
            return false;
        }
        values.Add(value);
        return true;
    }

}
