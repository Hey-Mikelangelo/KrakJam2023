using UnityEngine;
using UnityEngine.InputSystem;

public static class RaycastUtils
{
    public static Ray GetMousePositionRay(Camera camera)
    {
        var mouse = Mouse.current;
        Vector2 screenPosition = mouse.position.ReadValue();
        screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);
        return camera.ScreenPointToRay(screenPosition, Camera.MonoOrStereoscopicEye.Mono);
    }

    public static bool TryGetMousePositionHit(Camera camera, float rayLength, LayerMask layerMask, out RaycastHit hit)
    {
        Ray pointerRay = GetMousePositionRay(camera);
        Debug.DrawRay(pointerRay.origin, pointerRay.direction * rayLength, Color.blue);
        bool isHit = Physics.Raycast(pointerRay, out hit, rayLength, layerMask.value, QueryTriggerInteraction.Ignore);
        return isHit;
    }
}
