using UnityEngine.InputSystem;
using UnityEngine;

public class ValueMagnitudeThreasholdInteraction : IInputInteraction
{
    public float magnitudeThreashold = 0.5f;

    private bool wasPerformed;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod()]
    static void OnEditorMethodLoad() => InputSystem.RegisterInteraction<ValueMagnitudeThreasholdInteraction>();
#else
    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
    static void OnRuntimeMethodLoad() => InputSystem.RegisterInteraction<ValueMagnitudeThreasholdInteraction>();
#endif
    public void Process(ref InputInteractionContext context)
    {
        var magnitude = context.ComputeMagnitude();
        if (magnitude > magnitudeThreashold)
        {
            context.Started();
            context.Performed();
            wasPerformed = true;
        }
        else if(wasPerformed)
        {
            wasPerformed = false;
            context.Canceled();
        }
    }

    public void Reset()
    {
        wasPerformed = false;
    }
}
