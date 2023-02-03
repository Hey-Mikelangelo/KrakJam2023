using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), ExecuteAlways]
public class CollisionEventsListener : MonoBehaviour
{
    public Rigidbody Rigidbody => rigidbody ??= GetComponent<Rigidbody>();

    public event Action<Collision> OnCollisionEnterEvent;
    public Action<Collision> OnCollisionStayEvent;
    public Action<Collision> OnCollisionExitEvent;
    private new Rigidbody rigidbody;

    private void OnCollisionEnter(Collision other)
    {
        OnCollisionEnterEvent?.Invoke(other);
    }

    private void OnCollisionStay(Collision other)
    {
        OnCollisionStayEvent?.Invoke(other);
    }

    private void OnCollisionExit(Collision other)
    {
        OnCollisionExitEvent?.Invoke(other);
    }

}
