using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionWrapper
{
    public InputAction InputAction { get; }
    private Action onPerformedAction;
    private Action onStartedAction;
    private Action onCancelledAction;

    public InputActionWrapper(InputAction inputAction,
        Action onPerformedAction,
        Action onStartedAction = null,
        Action onCancelledAction = null)
    {
        InputAction = InputSystemExtensions.GetDefaultPlayerInput().actions[inputAction.id.ToString()];
        this.onPerformedAction = onPerformedAction;
        this.onStartedAction = onStartedAction;
        this.onCancelledAction = onCancelledAction;
        Subscribe();
    }

    public void Subscribe()
    {
        if (onPerformedAction.Is_Not_NullWithErrorLog())
        {
            InputAction.performed += OnPerformedAction;
        }
        if (onStartedAction != null)
        {
            InputAction.started += OnStartedAction;
        }
        if (onCancelledAction != null)
        {
            InputAction.canceled += OnCancelledAction;
        }
    }
    public void Unsubscribe()
    {
        if (onPerformedAction.Is_Not_NullWithErrorLog())
        {
            InputAction.performed -= OnPerformedAction;
        }
        if (onStartedAction != null)
        {
            InputAction.started -= OnStartedAction;
        }
        if (onCancelledAction != null)
        {
            InputAction.canceled -= OnCancelledAction;
        }
    }
    public void Enable() => InputAction.Enable();
    public void Disable() => InputAction.Disable();

    public T ReadValue<T>() where T : struct
    {
        return this.InputAction.ReadValue<T>();
    }

    private void OnPerformedAction(InputAction.CallbackContext callbackContext)
    {
        this.onPerformedAction?.Invoke();
    }

    private void OnStartedAction(InputAction.CallbackContext callbackContext)
    {
        this.onStartedAction.Invoke();
    }
    private void OnCancelledAction(InputAction.CallbackContext callbackContext)
    {
        this.onCancelledAction.Invoke();
    }
}
