using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class PlayerController : MonoBehaviour
{
    [SerializeField, OdinSerialize] private UltEvent<bool> onActivateStateChanged = new UltEvent<bool>();
    [SerializeField] public UltEvent onActivated = new UltEvent();
    [SerializeField] public UltEvent onDeactivated = new UltEvent();
    [SerializeField] private bool disableOnInit; 

    private static List<PlayerController> playerControllers = new List<PlayerController>();
    public static PlayerController ActiveController { get; private set; }
    public static event System.Action OnActiveControllerChanged;
    public bool IsInputActive { get; private set; } = true;
    //protected abstract IInputComponentData InputComponentData { get; }

    protected virtual void Awake()
    {
        playerControllers.Add(this);
        if (disableOnInit)
        {
            enabled = false;
        }
        else 
        {
            this.ActivateThis();
        }
    }
    
    private void Start()
    {
        InvokeActivatedEvents(this);
    }

    private void OnDestroy()
    {
        playerControllers.Remove(this);

    }

    protected virtual void OnActiveStateChanged() { }
    [Button]
    public void ActivateThis()
    {
        Debug.Log($"Activate {this.name}");
        Activate(this);
    } 

    public void DeactivateThis()
    {
        ActivateController(this, false);
    }

    public abstract void SetActiveCamera();

    public void SetInputActive(bool activate)
    {
        IsInputActive = activate;
    }
    public static void Activate(PlayerController playerController)
    {
        if (playerController.IsNullWithErrorLog())
        {
            return;
        }
        var prevActiveController = ActiveController;
        if(prevActiveController != null && prevActiveController != playerController && prevActiveController.enabled)
        {
            Debug.Log($"Active 2 player controllers: {prevActiveController.name} and {playerController.name}. Disabling {prevActiveController.name}");
            ActivateController(prevActiveController, false);
        }
        ActiveController = playerController;
        ActivateController(playerController, true);
        OnActiveControllerChanged?.Invoke();
    }
    public static void ActivateController(PlayerController controller, bool activate)
    {
        if(controller.IsNullWithErrorLog())
        {
            return;
        }
        if(controller.enabled == activate)
        {
            return;
        }
        if(activate == false && ActiveController == controller)
        {
            ActiveController = null;
        }
        Debug.Log($"Activated controller {controller.name}: {activate}");
        controller.enabled = activate;
        
    }

    protected virtual void OnEnable()
    {
        SetInputActive(true);
        InvokeActivatedEvents(this);
    }

    protected virtual void OnDisable()
    {
        SetInputActive(false);
        InvokeActivatedEvents(this);
    }

    private static void InvokeActivatedEvents(PlayerController controller)
    {
        if (controller.enabled)
        {
            controller.onActivated?.Invoke();
        }
        else
        {
            controller.onDeactivated?.Invoke();
        }
        controller.onActivateStateChanged?.Invoke(controller.enabled);
        controller.OnActiveStateChanged();
    }
}
