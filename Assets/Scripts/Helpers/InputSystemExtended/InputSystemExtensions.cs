using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
public static class InputSystemExtensions
{
    public static InputBinding Merge(this InputBinding thisBinding, InputBinding otherBinding)
    {
        string unionActions = thisBinding.action.UnionList(otherBinding.action, InputBinding.Separator);
        string unionGroups = thisBinding.groups.UnionList(otherBinding.groups, InputBinding.Separator);
        string unionPaths = thisBinding.path.UnionList(otherBinding.path, InputBinding.Separator);
        var unionBinding = new InputBinding
        {
            action = unionActions,
            groups = unionGroups,
            path = unionPaths
        };
        return unionBinding;
    }

    public static string[] GetInteractionsNames(this InputAction inputAction)
    {
        var sb = new StringBuilder(inputAction.interactions);
        sb.RemoveSpansFromTo('(', ')', removeSeparators: true);
        return sb.ToString().Split(',', System.StringSplitOptions.RemoveEmptyEntries);
    }

    public static StringBuilder GetHintText(this InputAction inputAction, Color color = default)
    {
        if (inputAction.IsNullWithErrorLog())
        {
            return new StringBuilder("Null input action");
        }
        StringBuilder stringBuilder = GetActionHintPrefix(inputAction).Space();
        string deviceLayout = string.Empty;
        string controlPath = string.Empty;
        string bindingString = string.Empty;
        try
        {
            var bindingMask = new InputBinding()
            {
                groups = PlayerInput.GetPlayerByIndex(0).currentControlScheme
            };
            var activeBindingIndex = inputAction.GetBindingIndex(bindingMask);
            var binding = inputAction.bindings[activeBindingIndex];
            if (binding.isPartOfComposite &&
                string.Equals(binding.name, "modifier1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(binding.name, "modifier2", StringComparison.OrdinalIgnoreCase)
                || string.Equals(binding.name, "modifier", StringComparison.OrdinalIgnoreCase))
            {
                activeBindingIndex = inputAction.bindings.IndexOf(x =>
                    string.Equals(x.name, "binding", StringComparison.OrdinalIgnoreCase) && x.action == inputAction.name);
            }
            string bindingName = null;
            bindingString = inputAction.GetBindingDisplayString(activeBindingIndex, out deviceLayout, out controlPath,
                InputBinding.DisplayStringOptions.DontIncludeInteractions);
            if (BindingsBetterReadableNames.TryGetBetterName(inputAction.bindings[activeBindingIndex], out bindingName))
            {
                bindingString = bindingName;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return new StringBuilder($"Exception while getting binding for input action {inputAction.name}");
        }

        var inputBindingsIcons = InputBindingsIcons.Instance;
        if (inputBindingsIcons != null)
        {
            var control = inputAction.controls.FirstOrDefault(x => x.path == $"/{deviceLayout}/{controlPath}");
            if (control != null && inputBindingsIcons.TryGetSpriteAssetForInputControl(control.path, out var spriteAsset, out int index))
            {
                stringBuilder.Append("  ");
                stringBuilder.Append(spriteAsset.GetTMPSpriteString(index));
                stringBuilder.Append("  ");
                return stringBuilder;
            }

        }

        if (color != default)
        {
            stringBuilder.Append($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>");

        }
        stringBuilder.Append(bindingString);
        if (color != default)
        {
            stringBuilder.Append("</color>");
        }

        return stringBuilder;
    }

    public static string GetDevicePath(this InputBinding inputBinding)
    {
        int indexOfDeviceNameStart = inputBinding.path.IndexOf('<');
        int indexOfDeviceNameEnd = inputBinding.path.IndexOf('>');

        return inputBinding.path.Substring(indexOfDeviceNameStart + 1, indexOfDeviceNameEnd - indexOfDeviceNameStart - 1);
    }

    public static string GetBindingPathUnified(string bindingPath, StringBuilder sb = null)
    {
        int indexOfDeviceNameStart = bindingPath.IndexOf('<');
        if (indexOfDeviceNameStart == -1)
        {
            return bindingPath;
        }
        int indexOfDeviceNameEnd = bindingPath.IndexOf('>');
        if (indexOfDeviceNameEnd == -1)
        {
            Debug.LogError($"Not valid biniding path {bindingPath}");
            return bindingPath;
        }
        int pathLength = bindingPath.Length;
        if (sb == null)
        {
            sb = new StringBuilder(pathLength - 1);
        }
        sb.Length = 0;
        for (int i = 0; i < pathLength; i++)
        {
            if (i == indexOfDeviceNameStart)
            {
                sb.Append('/');
            }
            else if(i != indexOfDeviceNameEnd)
            {
                sb.Append(bindingPath[i]);
            }
        }
        return sb.ToString();
    }

    public static bool IsCompositeModifier(this InputBinding binding)
    {
        return (binding.isPartOfComposite &&
                string.Equals(binding.name, "modifier1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(binding.name, "modifier2", StringComparison.OrdinalIgnoreCase)
                || string.Equals(binding.name, "modifier", StringComparison.OrdinalIgnoreCase)
                || string.Equals(binding.name, "binding", StringComparison.OrdinalIgnoreCase));
    }

    public static string GetBindingComponentsConcatPath(this InputAction action, int compositeBindingIndex)
    {
        if (compositeBindingIndex < 0 || compositeBindingIndex >= action.bindings.Count)
        {
            return string.Empty;
        }
        var binding = action.bindings[compositeBindingIndex];
        if (binding.isComposite)
        {
            StringBuilder stringBuilder = new();
            int componentBindingIndex = compositeBindingIndex + 1;
            while (componentBindingIndex < action.bindings.Count && action.bindings[componentBindingIndex].isPartOfComposite)
            {
                var componentBinding = action.bindings[componentBindingIndex];
                stringBuilder.Append(componentBinding.effectivePath).Append('+');
                componentBindingIndex++;
            }
            return stringBuilder.ToString();
        }
        return string.Empty;
    }
    public static StringBuilder GetActionHintPrefix(InputAction inputAction)
    {
        string[] inputActionInteractionsNames = inputAction.GetInteractionsNames();
        InputActionInteractionLocalizedNames inputActionInteractionLocalizedNames = InputActionInteractionLocalizedNames.Instance;
        StringBuilder hintPrefixBuilder = new StringBuilder();
        int inputActionInteractionsCount = inputActionInteractionsNames.Length;
        if (inputActionInteractionsCount == 0)
        {
            var bindingMask = new InputBinding()
            {
                groups = PlayerInput.GetPlayerByIndex(0).currentControlScheme
            };
            var activeBindingIndex = inputAction.GetBindingIndex(bindingMask);
            var binding = inputAction.bindings[activeBindingIndex];
            var devicePath = binding.GetDevicePath();
            Span<int> inputModifiersBindingsIndexes = stackalloc int[2];
            int modifiersCount = 0;
            if (binding.IsCompositeModifier())
            {
                var modifierBindingIndex = inputAction.bindings.IndexOf(x =>
                    string.Equals(x.name, "modifier", StringComparison.OrdinalIgnoreCase) && x.action == inputAction.name);
                if (modifierBindingIndex != -1)
                {
                    inputModifiersBindingsIndexes[0] = (modifierBindingIndex);
                    modifiersCount = 1;
                }
                else
                {
                    var modifier1BindingIndex = inputAction.bindings.IndexOf(x =>
                        string.Equals(x.name, "modifier1", StringComparison.OrdinalIgnoreCase) && x.action == inputAction.name);
                    if (modifier1BindingIndex != -1)
                    {
                        inputModifiersBindingsIndexes[0] = (modifier1BindingIndex);
                        modifiersCount = 1;
                    }
                    var modifier2BindingIndex = inputAction.bindings.IndexOf(x =>
                        string.Equals(x.name, "modifier2", StringComparison.OrdinalIgnoreCase) && x.action == inputAction.name);
                    if (modifier2BindingIndex != -1)
                    {
                        inputModifiersBindingsIndexes[1] = (modifier2BindingIndex);
                        modifiersCount = 2;
                    }
                }

            }
            InputDevice device = binding.isComposite ? null : InputSystem.GetDevice(devicePath);

            if (device is Mouse)
            {
                hintPrefixBuilder.Append(inputActionInteractionLocalizedNames.DefaultMouseInteractionName);
            }
            else if (device is Keyboard)
            {
                hintPrefixBuilder.Append(inputActionInteractionLocalizedNames.DefaultKeyboardInteractionName);
            }
            else
            {
                hintPrefixBuilder.Append(inputActionInteractionLocalizedNames.DefaultInteractionName);
            }
            if (inputModifiersBindingsIndexes != null)
            {
                for (int i = 0; i < modifiersCount; i++)
                {
                    string bindingName = string.Empty;
                    if (BindingsBetterReadableNames.TryGetBetterName(inputAction.bindings[inputModifiersBindingsIndexes[i]], out bindingName) == false)
                    {
                        bindingName = inputAction.GetBindingDisplayString(inputModifiersBindingsIndexes[i],
                            InputBinding.DisplayStringOptions.DontIncludeInteractions); ;
                    }
                    hintPrefixBuilder.Space().Append(bindingName).Append(" +");
                }
            }
            return hintPrefixBuilder;
        }
        for (int i = 0; i < inputActionInteractionsCount; i++)
        {
            var localizedName = inputActionInteractionLocalizedNames.GetLocalizedName(inputActionInteractionsNames[i]);
            hintPrefixBuilder.Append(localizedName);
            if (i != inputActionInteractionsCount - 1)
            {
                hintPrefixBuilder.Space();
                hintPrefixBuilder.Append(inputActionInteractionLocalizedNames.AndWordName);
                hintPrefixBuilder.Space();
            }
        }
        return hintPrefixBuilder;
    }

    public static void Enable(this InputAction inputAction, bool enable)
    {
        if (enable)
        {
            inputAction.Enable();
        }
        else
        {
            inputAction.Disable();
        }
    }

    public static float ReadValueFloatSmoothIfMouse(this InputAction action, float maxInputValue, float multiplier)
    {
        float inputValue = action.ReadValue<float>();

        bool isInputFromMouse = action.activeControl != null && action.activeControl.device == Mouse.current;
        if (isInputFromMouse)
        {
            inputValue = Mathf.Clamp(inputValue, 0, maxInputValue);
            inputValue *= multiplier;
            return inputValue;
        }
        return inputValue;
    }

    public static List<(InputActionMap actionMap, bool enabledStatus)> GetActionMapsEnabledStatuses()
    {
        var playerInput = GetDefaultPlayerInput();
        if (playerInput.IsNullWithErrorLog())
        {
            return new List<(InputActionMap actionMap, bool enabledStatus)>();
        }
        var actionMaps = playerInput.actions.actionMaps;
        int count = actionMaps.Count;
        List<(InputActionMap actionMap, bool enabledStatus)> actionMapsEnabledStatuses = new(count);
        for (int i = 0; i < count; i++)
        {
            var actionMap = actionMaps[i];
            actionMapsEnabledStatuses.Add((actionMap, actionMap.enabled));
        }
        return actionMapsEnabledStatuses;
    }

    public static void DisableAllActionMaps()
    {
        var playerInput = GetDefaultPlayerInput();
        if (playerInput.IsNullWithErrorLog())
        {
            return;
        }
        var actionMaps = playerInput.actions.actionMaps;
        int count = actionMaps.Count;
        List<(InputActionMap actionMap, bool enabledStatus)> actionMapsEnabledStatuses = new(count);
        for (int i = 0; i < count; i++)
        {
            var actionMap = actionMaps[i];
            actionMap.Disable();
        }
    }

    public static void ResetActionMapsEnabledStatuses(IReadOnlyList<(InputActionMap actionMap, bool enabledStatus)> actionMapsEnabledStatuses)
    {
        var playerInput = GetDefaultPlayerInput();
        if (playerInput.IsNullWithErrorLog())
        {
            return;
        }
        var actionMaps = playerInput.actions.actionMaps;
        int count = actionMaps.Count;
        for (int i = 0; i < count; i++)
        {
            var actionMap = actionMaps[i];
            var indexOfActionMap = actionMapsEnabledStatuses.IndexOf((x) => x.actionMap == actionMap);
            if (indexOfActionMap == -1)
            {
                Debug.LogError($"Action map {actionMap.name} not found");
                continue;
            }
            bool wasEnabled = actionMapsEnabledStatuses[indexOfActionMap].enabledStatus;
            if (wasEnabled)
            {
                actionMap.Enable();
            }
            else
            {
                actionMap.Disable();
            }
        }
    }
    public static PlayerInput GetDefaultPlayerInput() => PlayerInput.GetPlayerByIndex(0);

    public static void EnableActionMaps(IReadOnlyList<string> actionMapsNames)
    {
        var player = GetDefaultPlayerInput();
        var actionMaps = player.actions.actionMaps;
        foreach (var actionMap in actionMaps)
        {
            if (actionMapsNames.Contains(actionMap.name))
            {
                actionMap.Enable();
            }
            else
            {
                actionMap.Disable();
            }
        }
    }

    /// <summary>
    /// Use to get the same action object when using <see cref="InputActionReference"/> objects. 
    /// <see cref="InputAction"/>s are duplicated when some <see cref="InputActionReference"/> are set in Adressable Scriptable Objects
    /// and some other are set in non Adressables scenes.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static InputAction GetDefaultPlayerAction(this InputAction action)
    {
        var defaultPlayer = GetDefaultPlayerInput();
        if (defaultPlayer.IsNullWithErrorLog())
        {
            return null;
        }
        return defaultPlayer.actions[action.id.ToString()];
    }
}
