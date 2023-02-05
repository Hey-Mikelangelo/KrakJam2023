using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class AssignStringValueSOFromTextInputBehaviour : MonoBehaviour
{
    [SerializeField, Required] private TMP_InputField inputField;
    [SerializeField, Required] private StringValueSO stringValueSO; 
    public void Assign()
    {
        stringValueSO.Value = inputField.text;
    }
}