using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VirtualButtonActionAuthoring : LoadableMonoBehaviour
{
    [SerializeField, Required] private InputActionReference inputActionReference;
    [SerializeField, OdinSerialize] private UltEvent<bool> OnActionEnabledStatusChanged = new();
    private VirtualButtonAction virtualButtonAction;
    private InputAction action;
    private bool actionEnabledStatus;
    private bool isDeactivatedForOnePress;
    public override void OnLoaded()
    {
        base.OnInit();
        action = inputActionReference.action.GetDefaultPlayerAction();
        actionEnabledStatus = action.enabled;
        OnActionEnabledStatusChanged.Invoke(actionEnabledStatus);
        virtualButtonAction = new VirtualButtonAction(action);
        //Debug.Log($"Action {action.name}. binding {virtualButtonAction.Binding}, {action.GetHashCode()}");
    }
    public override void OnBeforeUnloaded()
    {
        base.OnBeforeUnloaded();
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


