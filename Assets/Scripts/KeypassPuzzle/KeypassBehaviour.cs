using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KeypassBehaviour : MonoBehaviour
{
    [SerializeField] private List<Button> keyButtons = new();
    [SerializeField] private int combinationNumbersCount = 4;
    [SerializeField, Required] private TextMeshProUGUI combinationTextBox;
    [SerializeField] private string separator = " ";
    public UltEvent OnSubmit = new();
    private List<UnityAction> keyPressActions = new();
    private List<int> combinationInput = new();
    public int CombinationNumbersCount => combinationNumbersCount;
    public IReadOnlyList<int> CombinationInput => combinationInput;

    private void Awake()
    {
        int count = keyButtons.Count;
        for (int i = 0; i < count; i++)
        {
            var button = keyButtons[i];
            if (button.IsNullWithErrorLog())
            {
                keyPressActions.Add(null);
                continue;
            }
            int number = i;
            UnityAction keyPressDelegate = new UnityAction(() => OnKeyPressed(number));
            keyPressActions.Add(keyPressDelegate);
            button.onClick.AddListener(keyPressDelegate);
        }
        UpdateView();
    }

    private void OnDestroy()
    {
        int count = keyButtons.Count;
        for (int i = 0; i < count; i++)
        {
            var button = keyButtons[i];
            if (button.IsNullWithErrorLog())
            {
                continue;
            }
            UnityAction keyPressDelegate = keyPressActions[i];
            button.onClick.RemoveListener(keyPressDelegate);
        }
    }
    public void Clear()
    {
        combinationInput.Clear();
        UpdateView();
    }

    public void Submit()
    {
        if (combinationInput.Count < combinationNumbersCount)
        {
            Debug.Log("Cannot submit. Not enought numbers");
            return;
        }
        OnSubmit.Invoke();
    }

    private void OnKeyPressed(int number)
    {
        if (combinationInput.Count >= combinationNumbersCount)
        {
            return;
        }
        combinationInput.Add(number);
        UpdateView();
    }



    private void UpdateView()
    {
        int count = combinationInput.Count;
        StringBuilder stringBuilder = new(count);
        for (int i = 0; i < count; i++)
        {
            stringBuilder.Append(combinationInput[i]);
            stringBuilder.Append(separator);
        }
        if (stringBuilder.Length > 0)
        {
            stringBuilder.Length -= 1;

        }
        combinationTextBox.text = stringBuilder.ToString();
    }
}
