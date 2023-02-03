using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public static class ComponentsExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Not_NullWithErrorLog<T>(this T obj, object hintObject = null) where T : class
    {
#if DEVELOPMENT_BUILD
        return obj != null;
#else
        return obj.IsNullWithErrorLog<T>(hintObject) == false;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullWithErrorLog<T>(this T obj, object hintObject = null) where T : class
    {
#if DEVELOPMENT_BUILD
        return obj == null;
#else
        var isNull = obj.IsNull();
        if (isNull)
        {
            obj.LogNullError<T>(hintObject);
        }
        return isNull;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogErrorIfNull<T>(this T obj, object hintObject = null) where T : class
    {
        if (obj.IsNull())
        {
            obj.LogNullError<T>(hintObject);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogNullError<T>(this T obj, object hintObject = null) where T : class
    {
        if (hintObject != null)
        {
            Debug.LogError($"{typeof(T).Name} is Null: {hintObject}");
        }
        else
        {
            Debug.LogError($"{typeof(T).Name} is Null");
        }
    }

    public static bool IsNull<T>(this T obj)
    {
        bool isNull;
        if (obj is UnityEngine.Object unityObject)
        {
            isNull = unityObject == null;
        }
        else
        {
            isNull = obj == null;
        }
        return isNull;
    }

    public static string GetHierarchyPath(this GameObject gameObject)
    {
        if (gameObject.IsNullWithErrorLog())
        {
            return string.Empty;
        }
        StringBuilder sb = new StringBuilder();
        var hierarchyEnumerator = gameObject.transform.GetHierarchyTopToBottomEnumeratorFromChild();
        bool  isFirst = true;
        Transform lastTransform = null;
        foreach (var transform in hierarchyEnumerator)
        {
            if (isFirst)
            {
                sb.Append(transform.gameObject.scene.name).Append('/');
                isFirst = false;
            }
            sb.Append(transform.gameObject.name).Append('/');
            lastTransform = transform;
        }
        //remove last '/'
        sb.Remove(sb.Length - 1, 1);
        if(lastTransform != null && lastTransform.parent != null)
        {
            var childs = lastTransform.parent.GetChilds();
            var indexOfChild = childs.IndexOf(lastTransform);
            sb.Append(". Child index: ");
            sb.Append(indexOfChild);
        }
        return sb.ToString();
    }
    public static void ResetComponent(this Transform transform)
    {
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }

    public static void ResetComponent(this RectTransform transform)
    {
        transform.anchoredPosition = new Vector2(0, 0);
        transform.anchorMin = new Vector2(0.5f, 0.5f);
        transform.anchorMax = new Vector2(0.5f, 0.5f);
        transform.sizeDelta = new Vector2(100, 100);
        transform.pivot = new Vector2(0.5f, 0.5f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasComponent<T>(this Component c) where T : Component
    {
        return c.GetComponent<T>() != null;
    }

    public static bool HasComponentWithErrorLog<T>(this Transform transform) where T : Component
    {
        return HasComponentWithErrorLog<T>(transform.gameObject);
    }
    public static bool HasComponentWithErrorLog<T>(this GameObject obj) where T : Component
    {
        bool hasComponent = obj.HasComponent<T>();
        if (hasComponent == false)
        {
            Debug.LogError($"{obj.name} should have {nameof(T)} component");
        }
        return hasComponent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.TryGetComponent(out T _);
    }


}
