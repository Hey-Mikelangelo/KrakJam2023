using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;



public class PlayerMovementController : PlayerController
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField, Required] private InputActionReference moveActionRef;
    [SerializeField, Required] private CameraGuidReference cameraRef;
    [SerializeField] private MovementMode movementMode = MovementMode.TopDown2d;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float noMovementDistance = 0.01f;
    [SerializeField] private float rotationSmoothing = 0.1f;
    private SmoothQuaternion smoothRotation;
    private InputAction moveAction;
    [ShowInInspector, ReadOnly] public bool IsMoving { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        moveAction = moveActionRef.action;
        moveAction.Enable();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (movementMode == MovementMode.Side2d)
        {
            rotationSmoothing = 0;
        }
        smoothRotation = new SmoothQuaternion(rotationSmoothing, Quaternion.identity);
    }
    private void FixedUpdate()
    {
        if (IsInputActive == false)
        {
            return;
        }
        if (smoothRotation.Update())
        {
            this.transform.rotation = smoothRotation.Value;
        }
        IsMoving = false;
        /*var moveDirection2d = moveAction.ReadValue<Vector2>();
        moveDirection2d.Normalize();
        IsMoving = moveDirection2d.magnitude != 0;*/
        var move = moveAction.ReadValue<float>();
        if (move == 0)
        {
            return;
        }
        var ray = RaycastUtils.GetMousePositionRay(cameraRef.Component);
        Vector3 position = ray.origin;

        var moveDirection = position - this.transform.position;

        var moveDirection2d = moveDirection.GetVector2WithRemovedValueOnAxis(Axis.Z);

        IsMoving = moveDirection2d.magnitude > noMovementDistance;
        if (IsMoving == false)
        {
            return;
        }
        Move(moveDirection2d);
    }
    private void LateUpdate()
    {
        Move(Vector2.zero);
    }
    public void Move(Vector2 moveDirection2d)
    {
        if (IsInputActive == false)
        {
            return;
        }
        moveDirection2d.Normalize();
        if (movementMode == MovementMode.Side2d)
        {
            //allow movement only left - right
            moveDirection2d.y = 0;
        }
        var moveVector2d = moveDirection2d * moveSpeed;
        rigidbody.velocity = (Vector3)moveVector2d;
        AlignToDirection(moveVector2d);
    }

    private void MovementModeSO_OnValueChanged()
    {
        AlignToDirection(Vector3.right);
    }

    private void AlignToDirection(Vector2 direction2d)
    {
        if (direction2d.magnitude == 0)
        {
            return;
        }
        if (movementMode == MovementMode.TopDown2d)
        {
            Vector3 upVector = Vector3.Cross(Vector3.forward, (Vector3)direction2d);
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, upVector);
            smoothRotation.TargetValue = rotation;
        }
        else
        {
            if (direction2d.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = -transform.localScale;
            }
            else if (direction2d.x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = -transform.localScale;
            }
        }

    }

    public override void SetActiveCamera()
    {
        Debug.Log("Set active cam");
    }
}
