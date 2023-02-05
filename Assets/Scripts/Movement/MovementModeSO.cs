using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class MovementModeSO : ScriptableSingleton<MovementModeSO>
{
    /*[ShowInInspector]
    public MovementMode Value
    {
        get => value;
        set
        {
            if (this.value == value)
            {
                return;
            }
            this.value = value;
            OnValueChanged?.Invoke();
        }
    }*/
    private MovementMode value = MovementMode.Side2d;
    public event System.Action OnValueChanged;
}
