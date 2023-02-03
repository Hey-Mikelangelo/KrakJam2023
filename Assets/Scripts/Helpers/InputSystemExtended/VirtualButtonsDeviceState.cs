using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public struct VirtualButtonsDeviceState : IInputStateTypeInfo
{
    public FourCC format => new FourCC("VRTB");
    [InputControl(name = nameof(VirtualButtonsDevice.button0), layout = "Button")]
    public float button0Value;

    [InputControl(name = nameof(VirtualButtonsDevice.button1), layout = "Button")]
    public float button1Value;

    [InputControl(name = nameof(VirtualButtonsDevice.button2), layout = "Button")]
    public float button2Value;

    [InputControl(name = nameof(VirtualButtonsDevice.button3), layout = "Button")]
    public float button3Value;

    [InputControl(name = nameof(VirtualButtonsDevice.button4), layout = "Button")]
    public float button4Value;
    
    [InputControl(name = nameof(VirtualButtonsDevice.button5), layout = "Button")]
    public float button5Value;
    
    [InputControl(name = nameof(VirtualButtonsDevice.button6), layout = "Button")]
    public float button6Value;
    
    [InputControl(name = nameof(VirtualButtonsDevice.button7), layout = "Button")]
    public float button7Value;
    
    [InputControl(name = nameof(VirtualButtonsDevice.button8), layout = "Button")]
    public float button8Value;

    [InputControl(name = nameof(VirtualButtonsDevice.button9), layout = "Button")]
    public float button9Value;

    [InputControl(name = nameof(VirtualButtonsDevice.button10), layout = "Button")]
    public float button10Value;
}
