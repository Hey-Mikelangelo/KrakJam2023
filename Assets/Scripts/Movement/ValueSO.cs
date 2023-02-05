using Sirenix.OdinInspector;
using UnityEngine;
public abstract class ValueSO<T> : ScriptableObject
{
    [ShowInInspector, ReadOnly] public T Value
    {
        get => value;
        set
        {
            if (this.value == null || this.value.Equals(value))
            {
                return;
            }
            this.value = value;
            OnValueChanged?.Invoke();
        }
    }
   private T value;
    public event System.Action OnValueChanged;
}
