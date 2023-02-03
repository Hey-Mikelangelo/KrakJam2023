using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void NotifyComponentAdded<T>(this T component) where T : MonoBehaviour
    {
        var listeners = component.GetComponents<IComponentAddedListener<T>>();
        for (int i = 0; i < listeners.Length; i++)
        {
            var listener = listeners[i];
            listener.OnComponentAdded(component);
        }
    }

    public static void NotifyComponentRemoved<T>(this T component) where T : MonoBehaviour
    {
        var listeners = component.GetComponents<IComponentAddedListener<T>>();
        for (int i = 0; i < listeners.Length; i++)
        {
            var listener = listeners[i];
            listener.OnComponentRemoved(component);
        }
    }
}

public interface IComponentAddedListener<T> where T : MonoBehaviour
{
    public void OnComponentAdded(T component);
    public void OnComponentRemoved(T component);
}