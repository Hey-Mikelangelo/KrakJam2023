using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SnapPositionCollider : MonoBehaviour
{
    [SerializeField, Required] private Transform snapPositionTransform;
    public Vector3 SnapPosition => snapPositionTransform.position;

    private void Reset()
    {
        snapPositionTransform = this.transform;
    }
}
