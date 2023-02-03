using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface IInputComponentData 
{
    public void EnableInput(bool enable);
}
public abstract class SingletonInputComponentData<TypeT, DataT> : SingletonComponentData<TypeT, DataT> , IInputComponentData
    where TypeT : SingletonInputComponentData<TypeT, DataT>
{
    public void EnableInput(bool enable = true)
    {
        var inputActions = GetInputActions();
        for (int i = 0; i < inputActions.Count; i++)
        {
            var inputAction = inputActions[i];
            if (inputAction.IsNullWithErrorLog())
            {
                continue;
            }
            if (enable)
            {
                inputAction.Enable();
            }
            else
            {
                inputAction.Disable();
            }
        }
    }
    protected abstract IReadOnlyList<InputAction> GetInputActions();
}
