using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovementController : PlayerController
{
    [SerializeField, Required] private InputActionReference moveActionRef;
    [SerializeField, Required] private CameraGuidReference cameraRef;
    [SerializeField, Required] private MovementModeSO movementModeSO;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float noMovementDistance = 0.01f;
    [SerializeField, Required] private SmoothMovementBehaviour smoothMovementBehaviour;
    private InputAction moveAction;
    [ShowInInspector, ReadOnly] public bool IsMoving { get; private set; }

    private void Awake()
    {
        moveAction = moveActionRef.action;
        moveAction.Enable();
        movementModeSO.OnValueChanged += MovementModeSO_OnValueChanged;
    }
    private void Update()
    {
        IsMoving = false;
        if (IsInputActive == false)
        {
            return;
        }
        /*var moveDirection2d = moveAction.ReadValue<Vector2>();
        moveDirection2d.Normalize();
        IsMoving = moveDirection2d.magnitude != 0;*/
        var move = moveAction.ReadValue<float>();
        if(move == 0)
        {
            return;
        }
        var ray = RaycastUtils.GetMousePositionRay(cameraRef.Component);
        Vector3 position = ray.origin;

        var moveDirection = position - this.transform.position;

        var moveDirection2d = movementModeSO.Value == MovementMode.Side2d
            ? moveDirection.GetVector2WithRemovedValueOnAxis(Axis.Z)
                : moveDirection.GetVector2WithRemovedValueOnAxis(Axis.Y);

        IsMoving = moveDirection2d.magnitude > noMovementDistance;
        if(IsMoving == false)
        {
            return;
        }
        Move(moveDirection2d);
    }

    public void Move(Vector2 moveDirection2d)
    {


        moveDirection2d.Normalize();
        if (moveDirection2d.magnitude == 0)
        {
            return;
        }
        var movementMode = movementModeSO.Value;
        if (movementMode == MovementMode.Side2d)
        {
            //allow movement only left - right
            moveDirection2d.y = 0;
        }
        Vector3 moveDirection = movementMode == MovementMode.Side2d ? (Vector3)moveDirection2d : moveDirection2d.GetVector3WithValueOnAxis(Axis.Y, 0);
        var prevPosition = smoothMovementBehaviour.MovedTransform.position;
        var moveVector = moveDirection * moveSpeed * Time.deltaTime;
        smoothMovementBehaviour.TargetPosition = prevPosition + moveVector;
        AlignToDirection(moveVector);
    }

    private void OnDestroy()
    {
        if (movementModeSO != null)
        {
            movementModeSO.OnValueChanged -= MovementModeSO_OnValueChanged;
        }
    }
    private void MovementModeSO_OnValueChanged()
    {
        AlignToDirection(Vector3.right);
    }

    private void AlignToDirection(Vector3 direction)
    {
        Quaternion rotation;
        if (movementModeSO.Value == MovementMode.Side2d)
        {
            Vector3 forwardVector = Quaternion.Euler(0, 90, 0) * direction;
            rotation = Quaternion.LookRotation(forwardVector, Vector3.up);
        }
        else
        {
            rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        smoothMovementBehaviour.TargetRotation = rotation;
    }



    public override void SetActiveCamera()
    {
        Debug.Log("Set active cam");
    }
}
