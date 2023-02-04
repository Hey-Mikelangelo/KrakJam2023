using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectColorTag : MonoBehaviour
{
    [SerializeField, Required] private ColorSO colorSO;
    [SerializeField, Required] private SpriteRenderer spriteRenderer;
    public Color Color => colorSO.Value;

    private void OnEnable()
    {
        colorSO.OnValueChanged += ColorSO_OnValueChanged;
        ColorSO_OnValueChanged();
    }


    private void OnDisable()
    {
        colorSO.OnValueChanged -= ColorSO_OnValueChanged;
    }
    private void ColorSO_OnValueChanged()
    {
        spriteRenderer.color = colorSO.Value;
    }
}
