using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ValueComponentData<T, TData> : MonoComponentData<T, TData>, ICrossSceneComponent<T> where T : ValueComponentData<T, TData>
{
    [SerializeField] private SerializableGuid guid;
    [ShowInInspector, ReadOnly] public TData Value { get; set; }
    protected virtual void Reset()
    {
        guid.SetNewIfNotSet();
    }
    public System.Guid Guid => guid;

    public event System.Action<ICrossSceneComponent> OnDestroyed;

    public override TData GetData()
    {
        return Value;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnDestroyed?.Invoke((ICrossSceneComponent<T>)this);
    }

#if UNITY_EDITOR
    [Button]
    private void SetValueFromEditor(TData value)
    {
        Value = value;
    }
#endif
}


