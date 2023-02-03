using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> on this <paramref name="gameObject"/> and all its children and sets their enabled state.
    /// </summary>
    /// <typeparam name="T">Components type.</typeparam>
    /// <param name="gameObject">Target object.</param>
    /// <param name="enabled">Enabled state to set.</param>
    public static void SetComponentsEnabledInChildren<T>(this GameObject gameObject, bool enabled) where T : MonoBehaviour
    {
        var components = gameObject.GetComponentsInChildren<T>();

        foreach (var component in components)
            component.enabled = enabled;
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> on this <paramref name="gameObject"/> and all its parents and sets their enabled state.
    /// </summary>
    /// <typeparam name="T">Components type.</typeparam>
    /// <param name="gameObject">Target object.</param>
    /// <param name="enabled">Enabled state to set.</param>
    public static void SetComponentsEnabledInParents<T>(this GameObject gameObject, bool enabled) where T : MonoBehaviour
    {
        var components = gameObject.GetComponentsInParent<T>();

        foreach (var component in components)
            component.enabled = enabled;
    }
    public static List<T> GetComponentsInChildren<T>(this IReadOnlyList<GameObject> rootGameObjects, bool includeInactive = false)
    {
        int rootObjectsCount = rootGameObjects.Count;
        List<T[]> rootObjectsComponents = new List<T[]>(rootObjectsCount);
        int allComponentsCount = 0;
        for (int i = 0; i < rootObjectsCount; i++)
        {
            var rootObject = rootGameObjects[i];
            var components = rootObject.GetComponentsInChildren<T>(includeInactive);

            rootObjectsComponents.Add(components);
            allComponentsCount += components.Length;
        }
        List<T> allComponents = new List<T>(allComponentsCount);
        for (int i = 0; i < rootObjectsComponents.Count; i++)
        {
            var components = rootObjectsComponents[i];
            allComponents.AddRange(components);
        }
        return allComponents;
    }

    public static T GetComponentInChildren<T>(this IReadOnlyList<GameObject> rootGameObjects, bool includeInactive = false)
    {
        int rootObjectsCount = rootGameObjects.Count;
        for (int i = 0; i < rootObjectsCount; i++)
        {
            var rootObject = rootGameObjects[i];
            var component = rootObject.GetComponentInChildren<T>(includeInactive);
            if(component != null)
            {
                return component;
            }
        }
        return default;
    }
    public static void ResetToNew(this GameObject gameObject, Transform parent)
    {
        var components = gameObject.GetComponents<Component>();
        if(gameObject.transform.parent != parent) 
        {
            gameObject.transform.SetParent(parent);
        }
        gameObject.transform.ResetComponent();
        if (gameObject.TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.ResetComponent();
        }
        int count = components.Length;
        for (int i = count - 1; i >= 0; i--)
        {
            var component = components[i];
            if (component is Transform || component is RectTransform)
            {
                continue;
            }
            if (Application.isPlaying)
            {
                GameObject.DestroyImmediate(component);
            }
            else
            {
                GameObject.DestroyImmediate(component);
            }
        }
        
    }

    public static T EnsureAddedComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.TryGetComponent<T>(out T com))
        {
            return com;
        }
        return gameObject.AddComponent<T>();
    }
}