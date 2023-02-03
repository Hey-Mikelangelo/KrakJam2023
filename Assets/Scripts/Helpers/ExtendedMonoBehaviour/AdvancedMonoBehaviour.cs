using System;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class AdvancedMonoBehaviour<T> : AdvancedMonoBehaviour where T : AdvancedMonoBehaviour<T>
{
    public T Inited()
    {
        return base.Inited<T>();
    }
}

public abstract class AdvancedMonoBehaviour : LoadableMonoBehaviour
{
    private List<(UltEvent, System.Action)> eventsAndActions = new List<(UltEvent, System.Action)>();
    private List<(UltEvent, System.Action)> lifetimeEventsAndActions = new List<(UltEvent, System.Action)>();

    public override void OnBeforeUnloaded()
    {
        base.OnBeforeUnloaded();
        int lifetimeEventsCount = lifetimeEventsAndActions.Count;
        for (int i = 0; i < lifetimeEventsCount; i++)
        {
            var (unityEvent, unityAction) = lifetimeEventsAndActions[i];
            if (unityEvent != null)
            {
                unityEvent -= unityAction;
            }
        }
    }

    protected virtual void OnEnable()
    {
        int eventsCount = eventsAndActions.Count;

        for (int i = 0; i < eventsCount; i++)
        {
            var (unityEvent, unityAction) = eventsAndActions[i];
            if (unityEvent != null)
            {
                unityEvent += unityAction;
            }
        }
    }

    protected virtual void OnDisable()
    {
        int eventsCount = eventsAndActions.Count;
        for (int i = 0; i < eventsCount; i++)
        {
            var (unityEvent, unityAction) = eventsAndActions[i];
            if(unityEvent != null)
            {
                unityEvent -= unityAction;
            }
        }
    }
    public T Inited<T>() where T : AdvancedMonoBehaviour
    {
        if (IsInited == false)
        {
            OnInit();
        }
        if (IsLoaded == false)
        {
            OnLoaded();
        }

        return this as T;
    }

    /// <summary>
    /// Automatically subscribes action to event in OnEnable() and unsubscribes in OnDisable()
    /// </summary>
    /// <param name="ultEvent"></param>
    /// <param name="callback"></param>
    public void AddManagedAction(UltEvent ultEvent, System.Action callback)
    {
        if (ultEvent.IsNullWithErrorLog() || callback.IsNullWithErrorLog())
        {
            return;
        }
        eventsAndActions.Add((ultEvent, callback));
    }


    /// <summary>
    /// Automatically unsubscribes action from event in OnBeforeUnload()
    /// </summary>
    /// <param name="ultEvent"></param>
    /// <param name="callback"></param>
    public void AddManagedLifetimeAction(UltEvent ultEvent, System.Action callback)
    {
        if (ultEvent.IsNullWithErrorLog() || callback.IsNullWithErrorLog())
        {
            return;
        }
        lifetimeEventsAndActions.Add((ultEvent, callback));
        ultEvent += callback;
    }

}


