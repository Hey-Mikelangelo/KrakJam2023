using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

[CreateAssetMenu]
public class InputActionsLocalizedNames : ScriptableSingleton<InputActionsLocalizedNames>
{
    [SerializeField] private List<InputActionReferenceLocalizedName> LocalizedNames = new();

    public bool TryGetLocalizedName(InputAction action, out LocalizedString localizedString)
    {
        var keyValuePairs = LocalizedNames;
        var keyValuePairsCount = keyValuePairs.Count;
        for (int i = 0; i < keyValuePairsCount; i++)
        {
            InputActionReferenceLocalizedName inputActionReferenceLocalizedName = keyValuePairs[i];
            if (inputActionReferenceLocalizedName.InputActionRef.IsNullWithErrorLog())
            {
                continue;
            }
            if (inputActionReferenceLocalizedName.InputActionRef.action.id == action.id)
            {
                localizedString = inputActionReferenceLocalizedName.LocalizedName;
                return true;
            }
        }
        localizedString = new LocalizedString();
        return false;
    }

    [Button]
    private void RemoveDuplicates()
    {
        int count = LocalizedNames.Count;
        List<InputActionReference> addedInputActionReferences = new(count);
        for (int i = count - 1; i >= 0; i--)
        {
            var value = LocalizedNames[i];
            if (value.IsNullWithErrorLog())
            {
                LocalizedNames.RemoveAt(i);
                continue;
            }
            var inputActionRef = value.InputActionRef;
            if (inputActionRef.IsNullWithErrorLog())
            {
                Debug.LogError($"Null input action ref on index {i}");
                continue;
            }
            if (addedInputActionReferences.Contains(inputActionRef))
            {
                LocalizedNames.RemoveAt(i);
                Debug.Log($"Removed duplicate {inputActionRef.name} at index {i}");
                continue;
            }
            addedInputActionReferences.Add(inputActionRef);
        }
    }

    [System.Serializable]
    private class InputActionReferenceLocalizedName
    {
        [SerializeField, Required] public InputActionReference InputActionRef;
        public LocalizedString LocalizedName;
    }
}
