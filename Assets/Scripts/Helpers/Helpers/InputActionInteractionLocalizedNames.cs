using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;


public static class BindingsBetterReadableNames
{
    private static Dictionary<string, string> bindingPathToNames = new()
    {
        { "<Mouse>/delta", "Drag Mouse" }
    };
    public static bool TryGetBetterName(InputBinding binding, out string bindingName)
    {
        return bindingPathToNames.TryGetValue(binding.path, out bindingName);
    }
}

[CreateAssetMenu]
public class InputActionInteractionLocalizedNames : ScriptableSingleton<InputActionInteractionLocalizedNames>
{
    [SerializeField] private SerializableDictionary<string, LocalizedString> interactionsNames = new SerializableDictionary<string, LocalizedString>();
    [SerializeField] private LocalizedString defaultInteractionName;
    [SerializeField] private LocalizedString defaultKeyboardInteractionName;
    [SerializeField] private LocalizedString defaultMouseInteractionName;

    [SerializeField, Tooltip("Word representing english \"and\" word. Used to connect more than one interaction names")] 
    private LocalizedString andWordName;
    public string DefaultInteractionName => defaultInteractionName.GetLocalizedString();
    public string DefaultKeyboardInteractionName => defaultKeyboardInteractionName.GetLocalizedString();
    public string DefaultMouseInteractionName => defaultMouseInteractionName.GetLocalizedString();

    public string AndWordName => andWordName.GetLocalizedString();

    public string GetLocalizedName(string interactionName)
    {
        bool foundName = interactionsNames.TryGetValue(interactionName, out var localizedStringName);
        if (foundName)
        {
            return localizedStringName.GetLocalizedString();
        }
        return string.Empty;
    }
}
