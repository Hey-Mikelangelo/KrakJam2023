using InteractionSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
public class ObjectsMovingBehahaviour : MonoBehaviour
{
    [SerializeField] private float pickingRange = 3;
    [SerializeField] private float smoothingTime = 0.1f;
    [SerializeField, Required] private CameraGuidReference cameraRef;
    [SerializeField, Required] private PlayerMovementController playerController;
    [SerializeField, Required] private MovementModeSO movementModeSO;
    [SerializeField, Required] private InputActionReference startMovingObjectActionRef;
    [SerializeField, Required] private InputActionReference stopMovingObjectActionRef;
    private bool move;
    private InputActionWrapper startMovingAction, stopMovingAction;
    private SmoothVector3 smoothPosition;
    private Interactable prevMovedInteractable;
    private RaycastHit[] hits = new RaycastHit[5];
    private void OnEnable()
    {
        smoothPosition = new SmoothVector3(smoothingTime, Vector3.zero);
    }
    private void Start()
    {
        startMovingAction = new InputActionWrapper(startMovingObjectActionRef.action, OnStartMoving);
        stopMovingAction = new InputActionWrapper(stopMovingObjectActionRef.action, OnStopMoving);
        startMovingAction.Enable();
        stopMovingAction.Enable();
    }
    private void OnDestroy()
    {
        startMovingAction.Unsubscribe();
        stopMovingAction.Unsubscribe();
        startMovingAction.Disable();
        stopMovingAction.Disable();
    }
    private void Update()
    {
        smoothPosition.Update();
        var selectedInteractable = InteractableSelection.SelectedInteractable;
        if (prevMovedInteractable != selectedInteractable)
        {
            prevMovedInteractable = selectedInteractable;
            if(selectedInteractable != null)
            {
                smoothPosition.SetValueImmediate(selectedInteractable.transform.position);
            }
        }
        if (selectedInteractable != null && move)
        {
            
            var pointerPosition = GetPointerPosition();
            var toPointerVector = pointerPosition - playerController.transform.position;
            if (toPointerVector.magnitude > pickingRange)
            {
                if (playerController.IsMoving == false)
                {
                    Vector2 toPointerVector2d = movementModeSO.Value == MovementMode.Side2d
                        ? toPointerVector.GetVector2WithRemovedValueOnAxis(Axis.Z)
                        : toPointerVector.GetVector2WithRemovedValueOnAxis(Axis.Y);
                    playerController.Move(toPointerVector2d);
                }
                toPointerVector = toPointerVector.normalized * pickingRange;
            }
            var pickedObjectPosition = playerController.transform.position + toPointerVector;
            smoothPosition.TargetValue = pickedObjectPosition;
            selectedInteractable.transform.position = smoothPosition.Value;
        }
    }

    private Vector3 GetPointerPosition()
    {
        var ray = RaycastUtils.GetMousePositionRay(cameraRef.Component);
        int hitsCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        Vector3 position;
        if(hitsCount > 0 && TryGetSnapPosition(hits, hitsCount, out var snapPosition))
        {
            position = snapPosition;
        }
        else
        {
            position = ray.origin;
        }
        return movementModeSO.Value == MovementMode.Side2d
                ? position.GetVector2WithRemovedValueOnAxis(Axis.Z)
                : position.WithValueOnAxis(Axis.Y, 0);
    }

    private bool TryGetSnapPosition(RaycastHit[] hits, int count, out Vector3 snapPosition)
    {
        for (int i = 0; i < count; i++)
        {
            var hit = hits[i];
            if(hit.collider.TryGetComponent(out SnapPositionCollider snapPositionCollider))
            {
                snapPosition = snapPositionCollider.SnapPosition;
                return true;
            }
        }
        snapPosition = default;
        return false;
    }
    private void OnStartMoving()
    {
        move = true;
    }
    private void OnStopMoving()
    {
        move = false;
    }
}
