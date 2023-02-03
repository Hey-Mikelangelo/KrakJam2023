using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransformPoint)), CanEditMultipleObjects]
public class TransformPointDrawer : Editor
{
    protected virtual void OnSceneGUI()
    {
        TransformPoint transformPoint = (TransformPoint)target;
        EditorGUI.BeginChangeCheck();
        var forwardDirection = transformPoint.forwardDirection;
        var upDirection = transformPoint.upDirection;

        if(forwardDirection == upDirection && upDirection == Vector3.zero)
        {
            forwardDirection = Vector3.forward;
            transformPoint.forwardDirection = forwardDirection;
            upDirection = Vector2.up;
            transformPoint.upDirection = upDirection;
        }
        else
        {
            Vector3 rightDirection = Vector3.Cross(forwardDirection, upDirection);
            upDirection = Vector3.Cross(rightDirection, forwardDirection);
            transformPoint.upDirection = upDirection;
        }
        
        Quaternion rotation = Quaternion.LookRotation(forwardDirection, upDirection);
        Quaternion newRotation = rotation;
        Vector3 newPosition = Handles.PositionHandle(transformPoint.position, rotation);
        if (transformPoint.EnableRotation)
        {
            newRotation = Handles.RotationHandle(rotation, newPosition);
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(transformPoint, "Changed Transform Point");
            transformPoint.position = newPosition;
            transformPoint.forwardDirection = newRotation * Vector3.forward;
            transformPoint.upDirection = newRotation * Vector3.up;
        }
    }
}
