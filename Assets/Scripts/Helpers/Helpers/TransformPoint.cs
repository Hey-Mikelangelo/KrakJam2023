using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class TransformPointRef : GuidReference<TransformPoint> { }

[ExecuteInEditMode]
public class TransformPoint : MonoBehaviour
{
    [SerializeField] private Vector3 pointLocalPosition;
    [SerializeField, ShowIf(nameof(enableRotation))] public Vector3 forwardDirection = Vector3.forward;
    [SerializeField, ShowIf(nameof(enableRotation))] public Vector3 upDirection = Vector3.up;
    [SerializeField] private bool enableRotation = true;

    public bool EnableRotation => enableRotation;
    [ShowInInspector, ReadOnly]
    public Vector3 position
    {
        get => transform.parent == null ? pointLocalPosition : transform.parent.TransformPoint(pointLocalPosition);
        set
        {
            if(transform.parent == null)
            {
                pointLocalPosition = value;
            }
            else
            {
                pointLocalPosition = transform.parent.InverseTransformPoint(value);
            }
        }
    }
}
