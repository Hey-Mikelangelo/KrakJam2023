using UnityEngine;

public class LoadableMonoBehaviour : InitableMonoBehaviour
{
    public bool IsLoaded { get; private set; }
    private bool? initialEnabledStatus = null;
    private bool isUnloaded;
    public static System.Action<LoadableMonoBehaviour> OnAwake;
    public event System.Action OnLoadedEvent;
    public event System.Action OnBeforeLoadedEvent;
    private bool isAwaked;

    protected override void Awake()
    {
        base.Awake();
        if(isAwaked == false)
        {
            isAwaked = true;
            OnAwake?.Invoke(this);
        }
    }

    public void EnsureLoaded()
    {
        EnsureInited();
        if (isAwaked == false)
        {
            isAwaked = true;
            OnAwake?.Invoke(this);
        }
        if (IsLoaded)
        {
            return;
        }
        OnBeforeLoaded();
        OnLoaded();
    }

    public void OnAfterInited()
    {
        if (initialEnabledStatus.HasValue)
        {
            return;
        }
        initialEnabledStatus = enabled;
        //needed to prevent running update before all scripts are Loaded
        enabled = false;
    }
    public void OnBeforeLoaded()
    {
        if (initialEnabledStatus.HasValue)
        {
            enabled = initialEnabledStatus.Value;
            OnBeforeLoadedEvent?.Invoke();
            initialEnabledStatus = null;
        }
       
    }
    public virtual void OnLoaded()
    {
        IsLoaded = true;
        isUnloaded = false;
        OnLoadedEvent?.Invoke();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(isUnloaded == false)
        {
            OnBeforeUnloaded();
        }
    }
    public virtual void OnBeforeUnloaded()
    {
        isUnloaded = true;
        IsLoaded = false;
        enabled = false;
    }
}