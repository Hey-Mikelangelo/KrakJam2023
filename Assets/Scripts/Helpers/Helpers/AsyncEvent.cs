using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
public class AsyncEvent<TEventArgs>
{
    private readonly List<Func<TEventArgs, UniTask>> invocationList;
    private readonly object locker;
    private List<Func<TEventArgs, UniTask>> tmpInvocationList = new();
    private AsyncEvent()
    {
        invocationList = new();
        locker = new object();
    }

    public static AsyncEvent<TEventArgs> operator +(
        AsyncEvent<TEventArgs> e, Func<TEventArgs, UniTask> callback)
    {
        if (callback == null) throw new NullReferenceException("callback is null");

        //Note: Thread safety issue- if two threads register to the same event (on the first time, i.e when it is null)
        //they could get a different instance, so whoever was first will be overridden.
        //A solution for that would be to switch to a public constructor and use it, but then we'll 'lose' the similar syntax to c# events             
        if (e == null) e = new AsyncEvent<TEventArgs>();

        lock (e.locker)
        {
            e.invocationList.Add(callback);
        }
        return e;
    }

    public static AsyncEvent<TEventArgs> operator -(
        AsyncEvent<TEventArgs> e, Func<TEventArgs, UniTask> callback)
    {
        if (callback == null) throw new NullReferenceException("callback is null");
        if (e == null) return null;

        lock (e.locker)
        {
            e.invocationList.Remove(callback);
        }
        return e;
    }

    public async UniTask InvokeAsync(TEventArgs eventArgs)
    {
        lock (locker)
        {
            tmpInvocationList.Clear();
            tmpInvocationList.AddRange(invocationList);
        }
        Debug.Log("Invoke async");
        foreach (var callback in tmpInvocationList)
        {
            //Assuming we want a serial invocation, for a parallel invocation we can use Task.WhenAll instead
            await callback(eventArgs);
        }
    }
}
public class AsyncEvent
{
    private readonly List<Func<UniTask>> invocationList;
    private readonly object locker;
    private List<Func<UniTask>> tmpInvocationList = new();
    private AsyncEvent()
    {
        invocationList = new();
        locker = new object();
    }

    public static AsyncEvent operator +(
        AsyncEvent e, Func<UniTask> callback)
    {
        if (callback == null) throw new NullReferenceException("callback is null");

        //Note: Thread safety issue- if two threads register to the same event (on the first time, i.e when it is null)
        //they could get a different instance, so whoever was first will be overridden.
        //A solution for that would be to switch to a public constructor and use it, but then we'll 'lose' the similar syntax to c# events             
        if (e == null) e = new AsyncEvent();

        lock (e.locker)
        {
            e.invocationList.Add(callback);
        }
        return e;
    }

    public static AsyncEvent operator -(
        AsyncEvent e, Func<UniTask> callback)
    {
        if (callback == null) throw new NullReferenceException("callback is null");
        if (e == null) return null;

        lock (e.locker)
        {
            e.invocationList.Remove(callback);
        }
        return e;
    }

    public async UniTask InvokeAsync()
    {
        lock (locker)
        {
            tmpInvocationList.Clear();
            tmpInvocationList.AddRange(invocationList);
        }

        foreach (var callback in tmpInvocationList)
        {
            //Assuming we want a serial invocation, for a parallel invocation we can use Task.WhenAll instead
            await callback();
        }
    }
}
