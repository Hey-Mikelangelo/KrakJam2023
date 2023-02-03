using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextMeshProInputFieldSubmitInvoker : MonoBehaviour
{
    [SerializeField, Required] private TMPro.TMP_InputField inputField;
    private BaseEventData eventData;

    public void InvokeSubmit()
    {
        if (eventData == null)
        {
            eventData = new BaseEventData(EventSystem.current);
        }
        eventData.selectedObject = inputField.gameObject;
        inputField.OnSubmit(eventData);
    }
}
