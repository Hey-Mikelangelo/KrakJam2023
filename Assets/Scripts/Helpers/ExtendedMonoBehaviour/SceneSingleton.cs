using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorHelpers
{
    public abstract class SceneSingleton<T> : AdvancedMonoBehaviour<T> where T : SceneSingleton<T>
    {
        public static bool IsReady => FindObjectOfType<T>() != null;
        public static T FirstInstance
        {
            get
            {
                var instances = Instances;
                if(instances.Count == 0)
                {
                    return null;
                }
                return instances[0];
            }
        }
        public static IReadOnlyList<T> Instances
        {
            get
            {
                return GetAllInstances();
            }
        }

        private static Dictionary<Scene, T> sceneInstances = new Dictionary<Scene, T>();

        private static readonly object lockObj = new object();
        private static List<T> instances = new List<T>(3);
        private static T GetInstance(Scene scene)
        {
            if(scene.IsValid() == false)
            {
                return null;
            }
            lock (lockObj)
            {
                if (sceneInstances.TryGetValue(scene, out T instance))
                {
                    return instance;
                }
                if(scene.isLoaded == false || scene.IsValid() == false)
                {
                    return null;
                }
                var instances = scene.FindObjectsOfType<T>();
                T firstInstance = null;
                if (instances.Length == 0)
                {
                    Debug.LogError($"<{typeof(T).Name}> An instance is needed in the scene {scene.name} and no existing instances were found. New Instance will be created");
                    var newGameObject = new GameObject($"{typeof(T).Name}");
                    firstInstance = newGameObject.AddComponent<T>();
                    SceneManager.MoveGameObjectToScene(newGameObject, scene);
                }
                else if (instances.Length == 1)
                {
                    firstInstance = instances[0];
                }
                else if (instances.Length > 1)
                {
                    firstInstance = instances[0];
                    Debug.LogError($"<{typeof(T).Name}> There should never be more than one {typeof(T).Name} in the scene. The first instance found will be used, and all others will be destroyed.");
                    for (int i = 1; i < instances.Length; i++)
                    {
                        if (Application.isEditor)
                        {
                            DestroyImmediate(instances[i]);
                        }
                        else
                        {
                            Destroy(instances[i]);
                        }
                    }
                }
                sceneInstances.Add(scene, firstInstance);
                return firstInstance;
            }
        }


        public static IReadOnlyList<T> GetAllInstances()
        {
            instances.Clear();

            if (Application.isPlaying == false)
            {
#if UNITY_EDITOR
                int scenesCount = UnityEditor.SceneManagement.EditorSceneManager.sceneCount;
                for (int i = 0; i < scenesCount; i++)
                {
                    var instance = GetInstance(UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i));
                    if(instance != null)
                    {
                        instances.Add(instance);
                    }
                }
#endif
            }
            else
            {
                int scenesCount = SceneManager.sceneCount;
                for (int i = 0; i < scenesCount; i++)
                {
                    var instance = GetInstance(SceneManager.GetSceneAt(i));
                    if (instance != null)
                    {
                        instances.Add(instance);
                    }
                }
            }
            return instances;
        }

    }
}

