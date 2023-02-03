using UnityEngine;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
namespace EditorHelpers
{
#if UNITY_EDITOR
    public static class EditorSceneManagerExtended
    {
        public static bool IsAllScenesLoaded()
        {
            int scenesCount = UnityEditor.SceneManagement.EditorSceneManager.sceneCount;
            for (int i = 0; i < scenesCount; i++)
            {
                var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);
                bool isSceneLoaded = scene.isLoaded;
                if (isSceneLoaded == false && EditorSceneManager.IsReloading(scene) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static void OpenScenes(IEnumerable<string> scenesPaths)
        {
            foreach (var scenePath in scenesPaths)
            {
                var scene = EditorSceneManager.GetSceneByPath(scenePath);
                if (scene.IsValid() && scene.isLoaded)
                {
                    continue;
                }

                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }

    public static class AssetDatabaseExtended
    {

        public static T[] GetAllScriptableObjects<T>() where T : ScriptableObject
        {
            var assets = GetAssets("t:" + typeof(T).Name);
            int assetsCount = assets.Count;
            T[] scriptableObjects = new T[assetsCount];
            for (int i = 0; i < assetsCount; i++)
            {
                scriptableObjects[i] = assets[i] as T;
            }

            return scriptableObjects;

        }

        public static T GetScriptableObject<T>() where T : ScriptableObject
        {
            var s = GetAllScriptableObjects<T>();
            return s.Length == 0 ? null : s[0];
        }

        public static List<Object> GetAssets(string searchString, bool includeSubAssets = true)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets(searchString);
            List<Object> objects = new List<Object>(guids.Length);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                if (includeSubAssets)
                {
                    objects.AddRange(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path));
                }
                else
                {
                    objects.Add(UnityEditor.AssetDatabase.LoadMainAssetAtPath(path));
                }
            }
            return objects;
        }

        public static T GetAssetFromGUID<T>(string guid) where T : Object
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            return obj;
        }


        public static Object GetAsset(string searchString)
        {
            var assets = GetAssets(searchString);
            if (assets.Count == 0)
            {
                return null;
            }
            return assets[0];
        }

        public static bool IsPrefabOnDisk(UnityEngine.Object unityObject)
        {
            if (unityObject is ScriptableObject)
            {
                return false;
            }
            string scenePath = unityObject is GameObject ? ((GameObject)unityObject).scene.path : ((Component)unityObject).gameObject.scene.path;
            bool isPrefabScene = string.IsNullOrEmpty(scenePath) || scenePath.EndsWith(".prefab");
            return (isPrefabScene || PrefabUtility.IsPartOfPrefabAsset(unityObject) || IsEditingInPrefabMode(unityObject));
        }

        public static bool IsEditingInPrefabMode(UnityEngine.Object unityObject)
        {
            if (EditorUtility.IsPersistent(unityObject) && (unityObject is ScriptableObject) == false)
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }
            else if (unityObject is Component component)
            {
                // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle(component.gameObject);
                if (currentStage != mainStage)
                {
                    var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(component.gameObject);
                    if (prefabStage != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable GetAllScenesAssetsDropdown()
        {
            var root = "Assets/Scenes/";

            return UnityEditor.AssetDatabase.GetAllAssetPaths()
                .Where(x => x.StartsWith(root))
                .Select(x => x.Substring(root.Length))
                .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(root + x)));

        }
        public static T CreateScriptableAtPath<T>(string name, string path, bool refreshAssetDatabase = true) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            if (string.IsNullOrEmpty(path))
            {
                path = Application.dataPath;
            }

            T scriptable = ScriptableObject.CreateInstance<T>();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fullPath = path + name + ".asset";
            fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(scriptable, fullPath);
            AssetDatabase.SaveAssets();
            if (refreshAssetDatabase)
            {
                AssetDatabase.Refresh();
            }
            return scriptable;
        }

        public static string FullToLocalPath(string fullPath)
        {
            return "Assets\\" + fullPath.Remove(0, Application.dataPath.Length);
        }

        public static string LocalToFullPath(string fullPath)
        {
            var applicationDataPath = Application.dataPath;
            var systemPathLength = applicationDataPath.Length - "Assets".Length;
            return applicationDataPath.Remove(0, systemPathLength);
        }
    }
#endif

}
