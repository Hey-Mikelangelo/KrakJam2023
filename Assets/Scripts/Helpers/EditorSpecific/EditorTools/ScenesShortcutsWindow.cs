using EditorHelpers;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesShortcutsWindow : EditorWindow
{
    private ScenesShortcutsSO scenesShortcuts;
    private List<(string shortcutName, IEnumerable<string> scenesPaths)> shortcutsScenesPaths;
    [MenuItem("Tools/Scenes shortcuts")]
    private static void OpenWindow()
    {
        var window = GetWindow<ScenesShortcutsWindow>();
        window.Initialize();
        window.Show();
    }

    public void UseShortcut(int index)
    {
        var scenesPaths = shortcutsScenesPaths[index].scenesPaths;
        int openScenesCount = EditorSceneManager.sceneCount;
        List<Scene> scenesToClose = new List<Scene>();
        for (int i = 0; i < openScenesCount; i++)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            var openScenePath = scene.path;
            if (scenesPaths.Contains(openScenePath) == false)
            {
                scenesToClose.Add(scene);
            }
        }

        EditorSceneManagerExtended.OpenScenes(scenesPaths);
        foreach (var sceneToClose in scenesToClose)
        {
            EditorSceneManager.CloseScene(sceneToClose, true);
        }
    }

    private void Initialize()
    {
        scenesShortcuts = AssetDatabaseExtended.GetScriptableObject<ScenesShortcutsSO>();
        if (scenesShortcuts.IsNullWithErrorLog())
        {
            shortcutsScenesPaths = new List<(string shortcutName, IEnumerable<string> scenesPaths)>();
            return;
        }
        shortcutsScenesPaths = scenesShortcuts.ShortcutsScenePaths;
    }

    private void OnGUI()
    {
        if (shortcutsScenesPaths == null) return;

        int shortcutsCount = shortcutsScenesPaths.Count;
        for (int i = 0; i < shortcutsCount; i++)
        {
            if (DrawShortcut(shortcutsScenesPaths[i]))
            {
                UseShortcut(i);
            }

        }
    }

    private bool DrawShortcut((string name, IEnumerable<string> sceneNames) shortcut)
    {
        bool clicked = GUILayout.Button(shortcut.name);
        return clicked;
    }
}

