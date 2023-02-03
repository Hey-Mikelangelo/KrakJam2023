using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public abstract class Singleton<T> : AdvancedMonoBehaviour<T> where T : Singleton<T>
{
    public static bool Quitting { get; private set; }
    public static bool IsReady => _instance != null || FindObjectOfType<T>() != null;
    public static T Instance => GetInstance();

    protected static T _instance;

    private static readonly object _lock = new object();

    private static T GetInstance()
    {
        if (Quitting)
        {
            Debug.LogWarning($"[{typeof(T).Name}] Instance will not be returned because the application is quitting.");
            return null;
        }
        lock (_lock)
        {
            if (_instance != null)
            {
                return _instance;
            }
            var instances = FindObjectsOfType<T>();
            if (instances.Length > 0)
            {
                if (instances.Length > 1)
                {
                    Debug.LogError($"<{typeof(T).Name}>. There should never be more than one {typeof(T).Name} in the scene {SceneManager.GetActiveScene().name}, but {instances.Length} were found. The first instance found will be used, and all others will be destroyed.");
                    for (var i = 0; i < instances.Length; i++)
                    {
                        Debug.LogError(instances[i].gameObject.GetHierarchyPath());
                        //Destroy(instances[i]);
                    }
                }

                return _instance = instances[0];
            }
            else
            {
                Debug.LogError($"[{typeof(T).Name}] An instance is needed in the scene and no existing instances were found");
                return null;
               // return _instance = new GameObject($"({nameof(Singleton<T>)}").AddComponent<T>();
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if(_instance == null)
        {
            _instance = this as T;
        }
        else if(_instance != this)
        {
            Debug.LogError($"{typeof(T).Name} is already set.");
        }
    }

    private void OnApplicationQuit()
    {
        Quitting = true;
    }
}


