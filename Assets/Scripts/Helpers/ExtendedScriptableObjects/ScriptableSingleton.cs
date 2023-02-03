using EditorHelpers;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// use <see cref="ScriptableSingletonIniter"/> to initialize <see cref="ScriptableSingleton"/> used in runtime
/// </summary>
public abstract class ScriptableSingleton : SerializedScriptableObject
{

    public virtual void OnDestroy() { }
    public virtual void Awake() { }
    public abstract bool IsSet { get; }
}

public abstract class ScriptableSingleton<T> : ScriptableSingleton where T : ScriptableSingleton<T>
{
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GetInstance();
                if (instance.Is_Not_NullWithErrorLog())
                {
                    instance.Awake();
                }
            }
            return instance;
        }
        private set => instance = value;
    }
    private static T instance;
    public override bool IsSet => instance != null;
    public override void Awake()
    {
#if UNITY_EDITOR
        if (Application.isEditor && EditorHelpers.AssetDatabaseExtended.GetAllScriptableObjects<T>().Length > 1)
        {
            Debug.LogError($"Remove created sciptable singleton instance {typeof(T).Name} because there is already instance of that type created in the project");
            return;
        }
#endif
        if (IsSet)
        {
            return;
        }
        //Debug.Log($"Awake ScriptableSingleton \"{this.GetType().Name}\"");
        Instance = this as T;
    }

    public override void OnDestroy()
    {
        Instance = null;
    }

    private static T GetInstance()
    {
        T[] singletons = null;
        singletons = Resources.FindObjectsOfTypeAll<T>();

        if (singletons == null || singletons.Length == 0)
        {
            Debug.LogError($"No loaded {typeof(T).Name} found in Resources folder");
            return default;
        }
        if (singletons.Length > 1)
        {
            Debug.LogError($"Found {singletons.Length} {nameof(ScriptableSingleton)}s in Resources folder");
        }
        return singletons[0];
    }


#if UNITY_EDITOR
    public static void EditorInit()
    {
        var singletons = EditorHelpers.AssetDatabaseExtended.GetAllScriptableObjects<T>();
        if (singletons == null || singletons.Length == 0)
        {
            throw new System.Exception($"No scriptable object of type {nameof(T)} is present in project");
        }
        if (singletons.Length > 1)
        {
            throw new System.Exception($"There are more than 1 {nameof(T)} ScriptableSingletons");
        }
        T singleton = singletons[0];
        if (Instance == null)
        {
            singleton.Awake();
        }
    }
#endif
}

