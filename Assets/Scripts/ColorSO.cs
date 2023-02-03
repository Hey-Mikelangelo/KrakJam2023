using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class ColorSO : ScriptableObject
{
    [ShowInInspector] public Color Value
    {
        get => value;
        set
        {
            if(this.value == value)
            {
                return;
            }
            this.value = value;
            OnValueChanged?.Invoke();
        }
    }

    private Color value;
    public event System.Action OnValueChanged;

}
