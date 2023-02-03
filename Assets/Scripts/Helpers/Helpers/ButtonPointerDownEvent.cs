using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[RequireComponent(typeof(Button), typeof(EventTrigger))]
public class ButtonPointerDownEvent : MonoBehaviour
{
    [SerializeField, ReadOnly] private Button button;
    [SerializeField, ReadOnly] private EventTrigger eventTrigger;
    [SerializeField] public UltEvent OnPointerDown = new();
    private bool isPointerDown;
    private EventTrigger.Entry pointerDownEntry, pointerUpEntry;
    private void Reset()
    {
        button = GetComponent<Button>();
        eventTrigger = GetComponent<EventTrigger>();
    }

    private void OnEnable()
    {
        pointerDownEntry = eventTrigger.AddEventTriggerCallback(EventTriggerType.PointerDown, OnPointerDownCallback);
        pointerUpEntry = eventTrigger.AddEventTriggerCallback(EventTriggerType.PointerUp, OnPointerUpCallback);
    }

    private void OnDisable()
    {
        if (pointerDownEntry != null)
        {
            eventTrigger.RemoveEventTriggerCallback(pointerDownEntry, OnPointerDownCallback);
        }
        if (pointerUpEntry != null)
        {
            eventTrigger.RemoveEventTriggerCallback(pointerUpEntry, OnPointerUpCallback);
        }
    }

    private void OnPointerDownCallback(BaseEventData eventData)
    {
        if(button.enabled == false || button.interactable == false)
        {
            return;
        }
        if (isPointerDown == false)
        {
            isPointerDown = true;
            OnPointerDown.Invoke();
        }
    }
    private void OnPointerUpCallback(BaseEventData eventData)
    {
        if (button.enabled == false || button.interactable == false)
        {
            return;
        }
        isPointerDown = false;
    }

}
