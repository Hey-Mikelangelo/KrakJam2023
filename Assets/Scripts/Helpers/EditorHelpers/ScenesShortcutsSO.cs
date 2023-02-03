using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class ScenesShortcutsSO : ScriptableSingleton<ScenesShortcutsSO>
{
#if UNITY_EDITOR
    [SerializeField] private List<ShortcutScenes> shortcuts = new List<ShortcutScenes>();
    public List<(string shortcutName, IEnumerable<string> scenesPaths)> ShortcutsScenePaths
    {
        get
        {
            var shortcutsScenePaths = new List<(string shortcutName, IEnumerable<string> scenesPaths)>(shortcuts.Count);
            foreach (var shortcut in shortcuts)
            {
                if (shortcut != null)
                {
                    shortcutsScenePaths.Add((shortcut.ShortcutName, shortcut.ScenesPaths));
                }
            }
            return shortcutsScenePaths;
        }
    }

    

    [System.Serializable]
    private class ShortcutScenes
    {
        public string ShortcutName;
        [ValueDropdown(nameof(GetAllScenesAssetsDropdown), IsUniqueList = true, DropdownTitle = "Select Scene", DrawDropdownForListElements = false, ExcludeExistingValuesInList = true)]
        public UnityEditor.SceneAsset[] ScenesToLoad = new UnityEditor.SceneAsset[0];

        public IEnumerable<string> ScenesPaths => ScenesToLoad.Select(x => AssetDatabase.GetAssetPath(x));

        private IEnumerable GetAllScenesAssetsDropdown()
        {
            return EditorHelpers.AssetDatabaseExtended.GetAllScenesAssetsDropdown();
        }
    }
#endif
}
