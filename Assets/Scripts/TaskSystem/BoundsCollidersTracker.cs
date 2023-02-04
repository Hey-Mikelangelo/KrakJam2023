using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class BoundsCollidersTracker : MonoBehaviour
{
    [SerializeField, Required] private BoxCollider boundsCollider;
    [SerializeField] private int maxColliders = 20;
    private Collider[] colliders;
    public ReadOnlyArray<Collider> CollidersInsideBounds => GetUpdatedColliders();
    private void OnValidate()
    {
        if(boundsCollider != null)
        {
            this.boundsCollider.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        colliders = new Collider[maxColliders];
        this.boundsCollider.isTrigger = true;
    }
    private ReadOnlyArray<Collider> GetUpdatedColliders()
    {
        var bounds = boundsCollider.bounds;
        int count = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, colliders);
        return new ReadOnlyArray<Collider>(colliders, 0, count);
    }
}
