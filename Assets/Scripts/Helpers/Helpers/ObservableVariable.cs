using System;

public abstract class ObservableVariable : IReadOnlyObservableVariable
{
    public abstract object ValueObj { get; }

    public abstract event Action OnValueChanged;
}
public class ObservableVariable<T> : ObservableVariable, IReadOnlyObservableVariable<T> where T: IEquatable<T>
{
    public T Value
    {
        get => value;
        set
        {
            if(this.value == null || this.value.Equals(value) == false)
            {
                this.value = value;
                OnValueChanged?.Invoke();
            }
        }
    }

    public override object ValueObj => Value;

    public override event Action OnValueChanged;
    private T value;

    public static implicit operator T(ObservableVariable<T> observableVariable)
    {
        return observableVariable.Value;
    }

}
