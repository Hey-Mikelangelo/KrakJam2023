using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

public class ObjectColorTag : MonoBehaviour
{
    [SerializeField, Required] private ColorSO colorSO;
    [SerializeField] private bool is2d;
    [SerializeField, Required, ShowIf(nameof(is2d))] private SpriteRenderer spriteRenderer;
    [SerializeField, Required, HideIf(nameof(is2d))] private MeshRenderer meshRenderer;
    [SerializeField, HideIf(nameof(is2d))] private int colorMaterialIndex;

    private Material[] materialsCopy;
    public Color Color => colorSO.Value;
    private void Awake()
    {
        if(is2d == false)
        {
            materialsCopy = meshRenderer.materials.Select(x => new Material(x)).ToArray();
        }
    }
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
        if (is2d)
        {
            spriteRenderer.color = colorSO.Value;
        }
        else
        {
            materialsCopy[colorMaterialIndex].color = colorSO.Value;
            meshRenderer.materials = materialsCopy;
        }
    }
}
