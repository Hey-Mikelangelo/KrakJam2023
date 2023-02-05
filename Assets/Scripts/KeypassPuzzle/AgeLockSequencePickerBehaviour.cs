using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class AgeLockSequencePickerBehaviour : MonoBehaviour
{
    [SerializeField, MinMaxSlider(0, 120, showFields: true)] private Vector2Int minMaxAge;
    [SerializeField] private int initialAge;
    [SerializeField, Required] private TextMeshProUGUI ageTextBox;
    [SerializeField] private IntSequenceSO ageLockCombinationSO;
    [SerializeField] private IntValueSO currentYearSO;
    [SerializeField] private IntValueSO ageValueSO;
    [ShowInInspector, ReadOnly] private int age;
    public UltEvent OnSubmit = new();
    private void Awake()
    {
        SetAge(initialAge);
        Submit();
    }
    public void DecreaseAge()
    {
        SetAge(this.age - 1);
    }
    public void IncreaseAge()
    {
        SetAge(this.age + 1);
    }

    private void SetAge(int newAge)
    {
        newAge = Mathf.Clamp(newAge, minMaxAge.x, minMaxAge.y);
        this.age = newAge;
        ageTextBox.text = this.age.ToString();
    }

    public void Submit()
    {
        var birthYear =2023 - age;
        ageLockCombinationSO.Value = birthYear.ToDigitsList();
        ageValueSO.Value = age;
    }
}
