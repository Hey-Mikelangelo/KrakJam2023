using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FloatingAboveTargetHeightBehaviour : MonoBehaviour
{
    [SerializeField] private float targetHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private Transform raycastTransform;
    [SerializeField, Min(0)] private float offsetFromElevation;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField, Min(0)] private float rayHeightOffset = 2;
    [SerializeField, Min(0)] private float rayDownLength = 0.2f;
    [SerializeField, Min(0)] private float checkSphereRadius = 0.1f;
    [SerializeField, Min(0)] private float moveUpSmoothingTime = 0.1f;
    [SerializeField, Min(0)] private float moveDownSmoothingTime = 0.1f;

    [SerializeField] private bool draw = true;
    [SerializeField] private QueryTriggerInteraction queryTriggerInteraction;
    private SmoothFloat smoothHeight;
    private float offset;
    private RaycastHit hit;

    private void Reset()
    {
        SetTargetHeightToCurrent();
    }
    [Button]
    public void SetTargetHeightToCurrent()
    {
        targetHeight = transform.position.y;
    }
    private void OnEnable()
    {
        smoothHeight = new SmoothFloat(moveUpSmoothingTime, transform.position.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (draw)
        {
            GizmosExtend.DrawSphere(transform.position.WithY(targetHeight), 0.05f, Color.green);
            GizmosExtend.DrawSphere(transform.position.WithY(maxHeight), 0.05f, Color.red);

        }
        var hitPoint = hit.collider == null ? transform.position : hit.point;
        var hitNormal = hit.collider == null ? Vector3.up : hit.normal;
        var sphereCenter = hitPoint + (hitNormal * checkSphereRadius);
        GizmosExtend.DrawSphere(sphereCenter, checkSphereRadius);
    }
    private void Update()
    {
        float maxOffset = 0f;
        float transformRayOffset;
        if (TryGetHit(out var hit))
        {
            transformRayOffset = hit.point.y - targetHeight;
            if (transformRayOffset < 0)
            {
                transformRayOffset = 0;
            }

        }
        else
        {
            transformRayOffset = 0;
        }
        if (transformRayOffset > maxOffset)
        {
            maxOffset = transformRayOffset;
        }
        offset = maxOffset;
        var newHeight = targetHeight + offset;
        newHeight = Mathf.Clamp(newHeight, targetHeight, maxHeight);
        smoothHeight.SmoothingTime = newHeight < raycastTransform.position.y ? moveDownSmoothingTime : moveUpSmoothingTime;
        smoothHeight.TargetValue = targetHeight + offset;
        smoothHeight.Update();
        var newSmoothedHeight = Mathf.Clamp(smoothHeight.Value, targetHeight, maxHeight);
        transform.position = transform.position.WithY(newSmoothedHeight);
    }

    private bool TryGetHit(out RaycastHit hit)
    {
        if (raycastTransform.IsNullWithErrorLog())
        {
            hit = default;
            return false;
        }
        var position = new Vector3(raycastTransform.position.x, targetHeight, raycastTransform.position.z);
        var rayDown = new Ray(position + new Vector3(0, rayHeightOffset, 0), Vector3.down);;
        return Physics.SphereCast(rayDown, checkSphereRadius, out hit, rayHeightOffset + rayDownLength, groundLayers.value, queryTriggerInteraction);
    }
}
