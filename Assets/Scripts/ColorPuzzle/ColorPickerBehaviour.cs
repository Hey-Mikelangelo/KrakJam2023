using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorPickerBehaviour : MonoBehaviour
{
    [SerializeField] private List<Button> colorSelectButtons = new();
    [SerializeField, Required] private ColorSO colorSO;
    private Color[] buttonsColors;
    private UnityAction[] buttonClickDelegates;
    private void Awake()
    {
        buttonsColors = colorSelectButtons.Select(x =>
        {
            if (x.TryGetComponent(out Image image))
            {
                return image.color;
            }
            else
            {
                return Color.magenta;
            }
        }).ToArray();
        for (int i = 0; i < buttonsColors.Length; i++)
        {
            if(buttonsColors[i] == null)
            {
                Debug.LogError($"button at index {i} does not have image");
            }
        }
        SubscribeToColorButtons();
        OnColorButtonClick(colorSelectButtons[0]);
    }
    private void OnDestroy()
    {
        UnsbscribeFronColorButtons();
    }

    private void SubscribeToColorButtons()
    {
        int count = colorSelectButtons.Count;
        buttonClickDelegates = new UnityAction[count];
        for (int i = 0; i < count; i++)
        {
            var button = colorSelectButtons[i];
            if (button.IsNullWithErrorLog())
            {
                continue;
            }
            UnityAction buttonClickDelegate = () => OnColorButtonClick(button);
            buttonClickDelegates[i] = buttonClickDelegate;
            button.onClick.AddListener(buttonClickDelegate);
        }
    }
    private void UnsbscribeFronColorButtons()
    {
        int count = colorSelectButtons.Count;
        for (int i = 0; i < count; i++)
        {
            var button = colorSelectButtons[i];
            if (button.IsNullWithErrorLog())
            {
                continue;
            }
            UnityAction buttonClickDelegate = buttonClickDelegates[i];
            if (buttonClickDelegate.Is_Not_NullWithErrorLog())
            {
                button.onClick.RemoveListener(buttonClickDelegate);
            }
        }
    }
    private void OnColorButtonClick(Button button)
    {
        var index = colorSelectButtons.IndexOf(button);
        if(index == -1)
        {
            Debug.LogError($"Not valid button {button.gameObject.GetHierarchyPath()}");
            return;
        }
        colorSO.Value = buttonsColors[index];
    }
}
