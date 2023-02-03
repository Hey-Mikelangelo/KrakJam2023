using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Localization;
[CreateAssetMenu()]
public class InputBindingsIcons : ScriptableSingleton<InputBindingsIcons>
{
    [OnValueChanged(nameof(VerifyInputcontrols))]
    [SerializeField] private SerializableDictionary<InputAction, SpriteAssetWithIndex> inputBindingsToSpriteAssets = new SerializableDictionary<InputAction, SpriteAssetWithIndex>();

    [SerializeField, ReadOnly] private SerializableDictionary<string, SpriteAssetWithIndex> inputControlsPathsToSpriteAssets = new SerializableDictionary<string, SpriteAssetWithIndex>();
    public bool TryGetSpriteAssetForInputControl(string fullControlPath, out TMP_SpriteAsset spriteAsset, out int spriteIndex)
    {
        bool isFound = inputControlsPathsToSpriteAssets.TryGetValue(fullControlPath, out var spriteAssetWithIndex);
        spriteAsset = spriteAssetWithIndex.spriteAsset;
        spriteIndex = spriteAssetWithIndex.index;
        return isFound;
    }

    [System.Serializable]
    private struct SpriteAssetWithIndex
    {
        public TMP_SpriteAsset spriteAsset;
        public int index;
    }

    [Button]
    private void Save()
    {
        inputControlsPathsToSpriteAssets.Clear();
        foreach (var inputActionToSpriteAsset in inputBindingsToSpriteAssets)
        {
            var inputAction = inputActionToSpriteAsset.Key;
            if (inputAction.controls.Count == 0)
            {
                continue;
            }
            var spriteAsset = inputActionToSpriteAsset.Value;
            var controlPath = inputAction.controls[0].path;
            inputControlsPathsToSpriteAssets.Add(controlPath, spriteAsset);
        }
    }
    private void VerifyInputcontrols()
    {
        foreach (var inputAction in inputBindingsToSpriteAssets.Keys)
        {
            if (inputAction.controls.Count > 1)
            {
                inputAction.ChangeBinding(1).Erase();
            }
        }

    }
}