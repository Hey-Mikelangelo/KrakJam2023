using System;

public interface IReadOnlyObservableVariable<T> : IReadOnlyObservableVariable
{
    public T Value { get; }
}
public interface IReadOnlyObservableVariable
{
    public event Action OnValueChanged;
    public object ValueObj { get; }
}
