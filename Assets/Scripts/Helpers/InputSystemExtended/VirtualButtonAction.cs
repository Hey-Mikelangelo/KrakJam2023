using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public class VirtualButtonAction
{
    private InputAction action;
    private InputControl control;
    private InputDevice device;
    private InputBinding binding;
    private int buttonIndex;
    public InputBinding Binding => binding;
    public InputDevice Device => device;
    private static Dictionary<InputActionMap, (InputDevice virtualDevice, List<int> takenButtonsIndexes)> actionMapToVirtualDeviceData = new();
    public VirtualButtonAction(InputAction inputAction)
    {
        action = inputAction;
        var actionMap = action.actionMap;
        InputDevice virtualDevice;
        if (actionMapToVirtualDeviceData.TryGetValue(actionMap, out var deviceData))
        {
            virtualDevice = deviceData.virtualDevice;
            buttonIndex = GetSmallestNotTakenIndex(deviceData.takenButtonsIndexes);
            deviceData.takenButtonsIndexes.Add(buttonIndex);
        }
        else
        {
            virtualDevice = InputSystem.AddDevice<VirtualButtonsDevice>();
            buttonIndex = 0;
            actionMapToVirtualDeviceData.Add(actionMap, (virtualDevice, new List<int>() { 0 }));
        }
        if (buttonIndex >= VirtualButtonsDevice.ButtonsCount)
        {
            throw new System.Exception($"Tried to add virtual button {buttonIndex} on {actionMap.name} Input Action Map, " +
                $"but {nameof(VirtualButtonsDevice)} has only {VirtualButtonsDevice.ButtonsCount} buttons. " +
                $"Split actions to different action maps or increase virtual buttons count in {nameof(VirtualButtonsDevice)} class");
        }

        ReadOnlyArray<InputDevice> newDevicesArray;
        if (actionMap.devices.HasValue == false)
        {
            newDevicesArray = new ReadOnlyArray<InputDevice>(new InputDevice[1] { virtualDevice });
        }
        else
        {
            var initialDevices = actionMap.devices.Value;
            int initialDevicesCount = initialDevices.Count;
            var newDevices = new InputDevice[initialDevicesCount + 1];
            for (int i = 0; i < initialDevicesCount; i++)
            {
                newDevices[i] = initialDevices[i];
            }
            newDevices[initialDevicesCount] = virtualDevice;
            newDevicesArray = new ReadOnlyArray<InputDevice>(newDevices);
        }
        actionMap.devices = newDevicesArray;

        string controlPath = $"/{VirtualButtonsDevice.ButtonBaseName}{buttonIndex}";

        control = virtualDevice[controlPath];
        device = virtualDevice;

        binding = new InputBinding($"<{nameof(VirtualButtonsDevice)}>{controlPath}");
        action.AddBinding(binding);
        //Debug.Log($"Action {action.name} additional binding {binding.path}");
    }


    public void Destroy()
    {
        if (actionMapToVirtualDeviceData.TryGetValue(action.actionMap, out var virtualDeviceData))
        {
            virtualDeviceData.takenButtonsIndexes.Remove(this.buttonIndex);
            if(virtualDeviceData.takenButtonsIndexes.Count == 0)
            {
                actionMapToVirtualDeviceData.Remove(action.actionMap);
            }
        }

        action.ChangeBinding(binding).Erase();
        if (InputSystem.devices.Contains(device))
        {
            InputSystem.RemoveDevice(device);
        }
    }

    public void PressAndRelease(MonoBehaviour coroutineRunner)
    {
        Set(true);
        coroutineRunner.StartCoroutine(SetFalseAfterTwoFrames());
    }

    private IEnumerator SetFalseAfterTwoFrames()
    {
        yield return null;
        yield return null;
        Set(false);
    }

    public void Press()
    {
        Set(true);
    }

    public void Release()
    {
        Set(false);
    }

    public void Set(bool state, float time = -1)
    {
        using (StateEvent.From(device, out var eventPtr))
        {
            float value = state ? 1 : 0;
            control.WriteValueIntoEvent(value, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }
    }
    private int GetSmallestNotTakenIndex(List<int> indexes)
    {
        int count = indexes.Count;
        indexes.Sort();
        if (count == 0)
        {
            return 0;
        }
        int triedIndex = 0;
        for (int i = 0; i < count; i++)
        {
            if (indexes[i] == triedIndex)
            {
                triedIndex++;
            }
            else
            {
                return triedIndex;
            }
        }
        return triedIndex;
    }
}
