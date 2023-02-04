using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

public class MeshRendererColorViewBehaviour : MonoBehaviour
{
    [SerializeField, Required] private MeshRenderer meshRenderer;
    [SerializeField, Min(0)] private int materialIndex;
    [SerializeField, Required] private ColorSO colorSO;
    private Material[] materialsClone;
    private void Awake()
    {
        colorSO.OnValueChanged += ColorSO_OnValueChanged;
            materialsClone = meshRenderer.materials.Select(x => new Material(x)).ToArray();
       
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
        int index = materialIndex;
        if (index >= meshRenderer.materials.Length)
        {
            index = 0;
        }
        materialsClone[index].color = colorSO.Value;
        meshRenderer.materials = materialsClone;
    }
}
