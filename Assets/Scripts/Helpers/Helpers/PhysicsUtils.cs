using UnityEngine;

public static class PhysicsUtils
{
    public static int OverlapColliderNonAlloc(Collider collider, Collider[] results, int mask, QueryTriggerInteraction queryTriggerInteraction, Vector3 positionOffset,
        Quaternion rotationOffset,
            out Vector3 boxCenter, out Vector3 extents)
    {
        boxCenter = Vector3.zero;
        extents = Vector3.zero;
        if (collider is BoxCollider boxCollider)
        {
            (Vector3 position, Vector3 halfExtents, Quaternion rotation) = GetOverlapDataForBox(boxCollider);
            boxCenter = position + positionOffset;
            rotation *= rotationOffset;
            extents = halfExtents;
            return Physics.OverlapBoxNonAlloc(position + positionOffset, halfExtents, results, rotation, mask, queryTriggerInteraction);
        }
        else if (collider is CapsuleCollider capsuleCollider)
        {
            (Vector3 p1, Vector3 p2, float radius) = GetOverlapDataForCapsule(capsuleCollider, rotationOffset);
            return Physics.OverlapCapsuleNonAlloc(p1 + positionOffset, p2 + positionOffset, radius, results, mask, queryTriggerInteraction);
        }
        else if (collider is SphereCollider sphereCollider)
        {
            (Vector3 center, float radius) = GetOverlapDataForSphere(sphereCollider);
            return Physics.OverlapSphereNonAlloc(center + positionOffset, radius, results, mask, queryTriggerInteraction);
        }
        else
        {
            var colliderBounds = collider.bounds;
            return Physics.OverlapBoxNonAlloc(colliderBounds.center + positionOffset, colliderBounds.size * 0.5f, results, Quaternion.identity, mask, queryTriggerInteraction);
        }
    }

    public static (Vector3 center, float radius) GetOverlapDataForSphere(SphereCollider sphereCollider)
    {
        var colliderTransform = sphereCollider.transform;
        Vector3 center = colliderTransform.TransformPoint(sphereCollider.center);
        float radius = sphereCollider.radius *
            Mathf.Max(
                Mathf.Max(colliderTransform.lossyScale.x, colliderTransform.lossyScale.z),
                colliderTransform.lossyScale.y);
        return (center, radius);
    }
    public static (Vector3 position, Vector3 halfExtents, Quaternion rotation) GetOverlapDataForBox(BoxCollider boxCollider)
    {
        var boxTransform = boxCollider.transform;
        Vector3 position = boxTransform.TransformPoint(boxCollider.center);
        Vector3 size = boxCollider.size.GetScaled(boxTransform.lossyScale);
        Quaternion rotation = boxTransform.rotation;
        return (position, size * 0.5f, rotation);
    }

    public static (Vector3 p1, Vector3 p2, float radius) GetOverlapDataForCapsule(CapsuleCollider capsuleCollider, 
        Quaternion rotationOffset)
    {
        Vector3 point1, point2;
        float scale;
        Vector3 scale3d = capsuleCollider.transform.lossyScale;

        // Get the center of the capsule collider in world space
        Vector3 center = capsuleCollider.transform.TransformPoint(capsuleCollider.center);

        // Get the orientation of the capsule collider in world space
        Quaternion rotation = capsuleCollider.transform.rotation * rotationOffset;

        // Calculate the point1 and point2 of the capsule collider
        if (capsuleCollider.direction == 0)
        {

            var halfHeightScaled = capsuleCollider.height * 0.5f * scale3d.x;
            // The capsule is oriented along the x-axis
            point1 = center + (rotation * (Vector3.left * halfHeightScaled));
            point2 = center + (rotation * (Vector3.right * halfHeightScaled));
            scale = Mathf.Max(scale3d.y, scale3d.z);
        }
        else if (capsuleCollider.direction == 1)
        {
            // The capsule is oriented along the y-axis
            var halfHeightScaled = capsuleCollider.height * 0.5f * scale3d.y;
            point1 = center + (rotation * (Vector3.down * halfHeightScaled));
            point2 = center + (rotation * (Vector3.up * halfHeightScaled));
            scale = Mathf.Max(scale3d.x, scale3d.z);
        }
        else
        {
            // The capsule is oriented along the z-axis
            var halfHeightScaled = capsuleCollider.height * 0.5f * scale3d.z;
            point1 = center + (rotation * (Vector3.back * halfHeightScaled));
            point2 = center + (rotation * (Vector3.forward * halfHeightScaled));
            scale = Mathf.Max(scale3d.x, scale3d.y);
        }

        // Calculate the correct radius based on the scale
        float radius = (capsuleCollider.radius * scale);
        return (point1, point2, radius);
    }

}
