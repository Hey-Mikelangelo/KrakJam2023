using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UnityUISystemExtensions
{
    public static void Refresh(this LayoutGroup layoutGroup)
    {
        layoutGroup.CalculateLayoutInputHorizontal();
        layoutGroup.CalculateLayoutInputVertical();
        layoutGroup.SetLayoutHorizontal();
        layoutGroup.SetLayoutVertical();
    }

    public static EventTrigger.Entry AddEventTriggerCallback(this EventTrigger eventTrigger, EventTriggerType eventTriggerType,
           UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = GetEventTriggerEntry(eventTriggerType);
        entry.callback.AddListener(action);
        eventTrigger.triggers.Add(entry);
        return entry;

        EventTrigger.Entry GetEventTriggerEntry(EventTriggerType eventTriggerType)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventTriggerType;
            EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
            entry.callback = triggerEvent;
            return entry;
        }
    }



    public static void RemoveEventTriggerCallback(this EventTrigger eventTrigger, EventTrigger.Entry entry,
        UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        entry.callback.RemoveListener(action);
        eventTrigger.triggers.Remove(entry);
    }
}
