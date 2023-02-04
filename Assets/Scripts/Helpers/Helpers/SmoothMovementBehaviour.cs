using Sirenix.OdinInspector;
using UnityEngine;

public class SmoothMovementBehaviour : MonoBehaviour
{
    [SerializeField, OnValueChanged(nameof(UpdateSmoothPositionSmoothingTime))] private float movementSmoothingTime = 0.1f;
    [SerializeField, OnValueChanged(nameof(UpdateSmoothRotationSmoothingTime))] private float rotationSmoothingTime = 0.1f;
    [SerializeField, Required] private Transform movedTransform;
    public Transform MovedTransform => movedTransform;
    private float movementSmoothingTimeOverride = -1;
    private float rotationSmoothingTimeOverride = -1;

    private SmoothVector3 smoothPosition;
    private SmoothQuaternion smoothRotation;
    public Vector3 TargetPosition
    {
        get => smoothPosition.TargetValue;
        set => SetTargetPosition(value);
    }
    public Quaternion TargetRotation
    {
        get => smoothRotation.TargetValue;
        set => SetTargetRotation(value);
    }

    private void Reset()
    {
        movedTransform = this.transform;
    }

    public float MovementSmoothingTime
    {
        get => movementSmoothingTimeOverride == -1 ? movementSmoothingTime : movementSmoothingTimeOverride;
        set
        {
            movementSmoothingTimeOverride = Mathf.Clamp(value, 0, float.MaxValue);
            UpdateSmoothPositionSmoothingTime();
        }
    }
    public float RotationSmoothingTime
    {
        get => rotationSmoothingTimeOverride == -1 ? rotationSmoothingTime : rotationSmoothingTimeOverride;
        set
        {
            rotationSmoothingTimeOverride = Mathf.Clamp(value, 0, float.MaxValue);
            UpdateSmoothRotationSmoothingTime();
        }
    }

    public void ResetMovementSmoothingToDefault()
    {
        movementSmoothingTimeOverride = -1;
    }

    public void ResetRotationSmoothingToDefault()
    {
        rotationSmoothingTimeOverride = -1;
    }


    private void UpdateSmoothPositionSmoothingTime()
    {
        smoothPosition.SmoothingTime = movementSmoothingTimeOverride == -1 ? movementSmoothingTime : movementSmoothingTimeOverride;
    }
    private void UpdateSmoothRotationSmoothingTime()
    {
        smoothRotation.SmoothingTime = rotationSmoothingTimeOverride == -1 ? rotationSmoothingTime : rotationSmoothingTimeOverride;
    }

    private void OnEnable()
    {
        smoothPosition = new SmoothVector3(movementSmoothingTime, movedTransform.position, GetPosition);
        smoothRotation = new SmoothQuaternion(rotationSmoothingTime, movedTransform.rotation);
    }

    private Vector3 GetPosition() => movedTransform.position;

    private void Update()
    {
        bool updatedRotation = smoothRotation.Update();
        bool updatedPosition = smoothPosition.Update();

        if (updatedRotation)
        {
            movedTransform.rotation = smoothRotation.Value;
        }
        if (updatedPosition)
        {
            movedTransform.position = smoothPosition.Value;
        }
    }

    public void SetTargetPositionImmediate(Vector3 position)
    {
        smoothPosition.SetValueImmediate(position);
        movedTransform.position = position;
    }

    public void SetTargetRotationImmediate(Quaternion rotation)
    {
        smoothRotation.SetValueImmediate(rotation);
        movedTransform.rotation = rotation;
    }
    public void SetTargetPosition(Vector3 position)
    {
        if (enabled == false)
        {
            SetTargetPositionImmediate(position);
            return;
        }
        smoothPosition.TargetValue = position;
    }

    public void SetTargetRotation(Quaternion rotation)
    {
        if (enabled == false)
        {
            SetTargetRotationImmediate(rotation);
            return;
        }
        smoothRotation.TargetValue = rotation;
    }
}

