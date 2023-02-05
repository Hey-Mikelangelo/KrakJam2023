using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvalidColorsSetterBehaviour : MonoBehaviour
{
    [SerializeField] private List<ColorSO> validColorsSO = new();
    [SerializeField] private List<ColorSO> invalidColorsSO = new();
    [SerializeField, Required] private ColorPickerBehaviour colorPicker;

    [Button]
    public void SetInvalidColors()
    {
        List<Color> validColors = validColorsSO.Select(x => x.Value).ToList();
        List<Color> notTakenColors = colorPicker.Colors.Where((color) => validColors.Contains(color) == false).ToList();
        for (int i = 0; i < invalidColorsSO.Count; i++)
        {
            int colorIndex = i;
            if(i >= notTakenColors.Count)
            {
                colorIndex = notTakenColors.Count - 1;
            }
            invalidColorsSO[i].Value = notTakenColors[colorIndex];
        }
    }
}
