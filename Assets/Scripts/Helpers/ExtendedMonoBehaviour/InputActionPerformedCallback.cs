using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionPerformedCallback : MonoBehaviour
{
    [SerializeField] private InputActionReference inputActionRef;
    public UltEvent OnPerformed;

    private InputActionWrapper inputAction;
    private void Awake()
    {
        inputAction = new(inputActionRef.action, OnActionPerformed);
        inputAction.Enable();
    }
    private void OnDestroy()
    {
        inputAction.Unsubscribe();
    }
    private void OnActionPerformed()
    {
        OnPerformed.Invoke();
    }
}
