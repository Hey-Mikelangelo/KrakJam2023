using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

[InputControlLayout(stateType = (typeof(VirtualButtonsDeviceState)))]
public class VirtualButtonsDevice : InputDevice
{
    public const int ButtonsCount = 11;
    public const string ButtonBaseName = "button";
    [InputControl]
    public ButtonControl button0{ get; private set; }
    [InputControl]
    public ButtonControl button1 { get; private set; }
    [InputControl]
    public ButtonControl button2 { get; private set; }
    [InputControl]
    public ButtonControl button3 { get; private set; }
    [InputControl]
    public ButtonControl button4 { get; private set; }
    [InputControl]
    public ButtonControl button5 { get; private set; }
    [InputControl]
    public ButtonControl button6 { get; private set; }
    [InputControl]
    public ButtonControl button7 { get; private set; }
    [InputControl]
    public ButtonControl button8 { get; private set; }
    [InputControl]
    public ButtonControl button9 { get; private set; }
    [InputControl]
    public ButtonControl button10 { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        button0 = GetChildControl<ButtonControl>(nameof(button0));
        button1 = GetChildControl<ButtonControl>(nameof(button1));
        button2 = GetChildControl<ButtonControl>(nameof(button2));
        button3 = GetChildControl<ButtonControl>(nameof(button3));
        button4 = GetChildControl<ButtonControl>(nameof(button1));
        button5 = GetChildControl<ButtonControl>(nameof(button5));
        button6 = GetChildControl<ButtonControl>(nameof(button6));
        button7 = GetChildControl<ButtonControl>(nameof(button7));
        button8 = GetChildControl<ButtonControl>(nameof(button8));
        button9 = GetChildControl<ButtonControl>(nameof(button9));
        button10 = GetChildControl<ButtonControl>(nameof(button10));
    }

    [UnityEngine.RuntimeInitializeOnLoadMethod]
    private static void RegisterLayout()
    {
        InputSystem.RegisterLayout<VirtualButtonsDevice>();

    }
}
