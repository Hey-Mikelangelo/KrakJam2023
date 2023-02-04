using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VirtualButtonActionAuthoring : MonoBehaviour
{
    [SerializeField, Required] private InputActionReference inputActionReference;
    [SerializeField, OdinSerialize] private UltEvent<bool> OnActionEnabledStatusChanged = new();
    private VirtualButtonAction virtualButtonAction;
    private InputAction action;
    private bool actionEnabledStatus;
    private bool isDeactivatedForOnePress;
    private void Start()
    {
        action = inputActionReference.action.GetDefaultPlayerAction();
        actionEnabledStatus = action.enabled;
        OnActionEnabledStatusChanged.Invoke(actionEnabledStatus);
        virtualButtonAction = new VirtualButtonAction(action);
    }
    private void OnDestroy()
    {
        if (virtualButtonAction != null)
        {
            virtualButtonAction.Destroy();
        }
    }
    
    private void Update()
    {
        if(action.enabled != actionEnabledStatus)
        {
            actionEnabledStatus = action.enabled;
            OnActionEnabledStatusChanged.Invoke(actionEnabledStatus);
        }
    }

    public void DeactivateForOnePress()
    {
        isDeactivatedForOnePress = true;
    }

    [Button]
    public void PressAndRelease()
    {
        if (isDeactivatedForOnePress)
        {
            isDeactivatedForOnePress = false;
            return;
        }
        virtualButtonAction.PressAndRelease(CoroutineRunner.Instance);
    }

   
}


