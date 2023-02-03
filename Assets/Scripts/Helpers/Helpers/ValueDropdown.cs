using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ValueDropdown<T, TableT> where TableT : ValuesTable<T, TableT>
{
    [Sirenix.OdinInspector.ValueDropdown(nameof(GetValues))]
    [SerializeField] private T value;

    public T Value => value;
#if UNITY_EDITOR
    [Button]
    private void AddEntry(T newEntry)
    {
        var table = GetTable();
        table.AddValue(newEntry);
        UnityEditor.EditorUtility.SetDirty(table);
    }
#endif
    public ValueDropdown()
    {
    }
    private System.Collections.Generic.IEnumerable<T> GetValues()
    {
        return GetTable().Values;
    }

    protected abstract TableT GetTable();
}
