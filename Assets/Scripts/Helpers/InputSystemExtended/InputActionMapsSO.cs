using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(menuName = "Input/Input action maps")]
public class InputActionMapsSO : ScriptableObject
{
    [SerializeField, OnValueChanged(nameof(ClearActionMaps))] private List<InputActionMapName> inputActionMaps = new List<InputActionMapName>();

    private List<InputActionMap> actionMaps;
    public IReadOnlyList<InputActionMap> ActionMaps => GetInputActionMaps();
    private List<InputAction> inputActions;
    [Button]
    public void Enable() => Enable(true);
    [Button]
    public void Disable() => Enable(false);
    public void Enable(bool enable)
    {
        var actionMaps = ActionMaps;
        int count = actionMaps.Count;
        if (enable)
        {
            for (int i = 0; i < count; i++)
            {
                var map = actionMaps[i];
                map.Enable();

                /*foreach (var action in map.actions)
                {
                    var playerAction = action.GetDefaultPlayerAction();
                    playerAction.Enable();
                    //Debug.Log($"Enable action {playerAction.name} {playerAction.GetHashCode()}");
                }*/
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var map = actionMaps[i];
                map.Disable();
                /*foreach (var action in map.actions)
                {
                    var playerAction = action.GetDefaultPlayerAction();
                    playerAction.Disable();
                    //Debug.Log($"Disable action {playerAction.name} {playerAction.GetHashCode()}");
                }*/
            }
        }
    }
   

    private void ClearActionMaps()
    {
    }
    public void RefreshActionMaps()
    {
        ClearActionMaps();
        GetInputActionMaps();
    }
    private List<InputActionMap> GetInputActionMaps()
    {
        /*if (this.actionMaps != null && this.actionMaps.Count == inputActionMaps.Count)
        {
            return this.actionMaps;
        }*/
        var actionsAsset = InputSystemExtensions.GetDefaultPlayerInput().actions;
        int inputActionMapsCount = inputActionMaps.Count;
        if(actionMaps != null)
        {
            actionMaps.Clear();
        }
        else
        {
            actionMaps = new List<InputActionMap>(inputActionMapsCount);
        }
        for (int i = 0; i < inputActionMapsCount; i++)
        {
            var actionMapName = inputActionMaps[i];
            var actionMap = GetMapWithName(actionsAsset.actionMaps, actionMapName.Name);
            if (actionMap.Is_Not_NullWithErrorLog())
            {
                actionMaps.Add(actionMap);
            }
            else
            {
                Debug.LogError($"No action map with name {actionMapName.Name}");
            }
        }
        return actionMaps;
    }

    private static InputActionMap GetMapWithName(ReadOnlyArray<InputActionMap> maps, string name)
    {
        foreach (var map in maps)
        {
            if(map.name == name)
            {
                return map;
            }
        }
        return null;
    }
}
