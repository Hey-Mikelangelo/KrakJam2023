using System;
using UnityEngine;

[RequireComponent(typeof(Collider)), ExecuteAlways]
public class TriggerEventsListener : MonoBehaviour
{
    public Collider Collider => collider ??= GetComponent<Collider>();
    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerStayEvent;
    public event Action<Collider> OnTriggerExitEvent;
    private new Collider collider;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }
}
