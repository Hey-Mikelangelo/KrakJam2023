using Sirenix.OdinInspector;
using UnityEngine;

public class InitableMonoBehaviour : MonoBehaviour
{
    public bool IsInited { get; private set; }

    public bool IsEnabledInHierarchy => this.enabled && this.gameObject.activeInHierarchy;

    protected virtual void Awake()
    {
        EnsureInited();
    }

    public void EnsureInited()
    {
        if (IsInited)
        {
            return;
        }
        OnInit();
    }
    public virtual void OnInit()
    {
        IsInited = true;
    }

    /*public void Enable(bool enable = true)
    {
        *//*if(this.enabled == enable)
        {
            return;
        }
        if(enable && this.gameObject.activeInHierarchy == false)
        {
            OnDisable();
        }
        else if(enable == false && this.gameObject.activeInHierarchy == false)
        {
            Disable();
        }
        if(this.enabled != enable && enable == true && this.gameObject.activeInHierarchy == )*//*
    }*/
   /* public void Disable()
    {

    }*/
    
    protected virtual void OnDestroy()
    {

    }
}
