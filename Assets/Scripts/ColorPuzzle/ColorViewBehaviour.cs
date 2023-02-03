using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ColorViewBehaviour : MonoBehaviour
{
    [SerializeField, Required] private Image image;
    [SerializeField, Required] private ColorSO colorSO;

    private void Awake()
    {
        colorSO.OnValueChanged += ColorSO_OnValueChanged;
    }
    private void Start()
    {
        ColorSO_OnValueChanged();
    }
    private void OnDestroy()
    {
        colorSO.OnValueChanged -= ColorSO_OnValueChanged;
    }
    private void ColorSO_OnValueChanged()
    {
        image.color = colorSO.Value;
    }
}
