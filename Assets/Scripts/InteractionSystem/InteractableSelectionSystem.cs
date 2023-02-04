using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace InteractionSystem
{
    public sealed class InteractableSelectionSystem : Singleton<InteractableSelectionSystem>
    {
        [SerializeField, Required] private CameraGuidReference cameraRef;
        [SerializeField, Required] private InputActionReference lockSelectionInputActionRef;
        [SerializeField, Required] private InputActionReference cancelLockSelectionInputActionRef;

        private RaycastHit[] hits = new RaycastHit[10];
        [ShowInInspector, ReadOnly] private GameObject hitInteractableGameObject; //for debugging in editor
        [ShowInInspector, ReadOnly] private GameObject hitGameObject; //for debugging in editor

        private List<InteractableSelectionData> allHitInteractables = new List<InteractableSelectionData>();
        private InputActionWrapper lockSelectionInputAction;
        private InputActionWrapper cancelLockSelectionInputAction;

        private Interactable lockedInteractable;
        private bool pressedLockInteractableSelection;

        private void Start()
        {
            lockSelectionInputAction = new InputActionWrapper(lockSelectionInputActionRef.action, LockSelection);
            cancelLockSelectionInputAction = new InputActionWrapper(cancelLockSelectionInputActionRef.action, CancelLockSelection);
            if (lockSelectionInputAction != null)
            {
                lockSelectionInputAction.Enable();
            }
            if (cancelLockSelectionInputAction != null)
            {
                cancelLockSelectionInputAction.Enable();
            }
        }

        private void OnDestroy()
        {
            lockSelectionInputAction.Unsubscribe();
            cancelLockSelectionInputAction.Unsubscribe();
        }

        private void Update()
        {
            UpdateSelectedInteractable(RaycastUtils.GetMousePositionRay(cameraRef.Component), uiBlocksRay: true);
        }
        private void OnEnable()
        {
            if(lockSelectionInputAction != null)
            {
                lockSelectionInputAction.Enable();
            }
            if(cancelLockSelectionInputAction != null)
            {
                cancelLockSelectionInputAction.Enable();
            }
        }
        private void OnDisable()
        {
            lockSelectionInputAction.Disable();
            cancelLockSelectionInputAction.Disable();
            var currentSelectedInteractable = InteractableSelection.SelectedInteractable;
            var currentSelectedBlockedInteractable = InteractableSelection.SelectedIgnoreBlockedInteractable;
            var currentHighlightedInteractable = InteractableSelection.HightlightedInteractable;
            if (currentSelectedInteractable != null)
            {
                currentSelectedInteractable.Deselect();
            }
            if (currentSelectedBlockedInteractable != null)
            {
                currentSelectedBlockedInteractable.Deselect();
            }
            if (currentHighlightedInteractable != null)
            {
                currentHighlightedInteractable.Deselect();
            }
            InteractableSelection.SelectedInteractable = null;
            InteractableSelection.SelectedIgnoreBlockedInteractable = null;
            InteractableSelection.HightlightedInteractable = null;
            allHitInteractables.Clear();
            InteractableSelection.AllHitInteractables = allHitInteractables;
        }
        public void SimulateSelectionRayCast(bool uiBlocksRay)
        {
            UpdateSelectedInteractable(RaycastUtils.GetMousePositionRay(cameraRef.Component), uiBlocksRay);
        }
        public void SimulateSelectionRayCast(Vector2 viewportPoint, bool uiBlocksRay)
        {
            UpdateSelectedInteractable(cameraRef.Component.ViewportPointToRay(viewportPoint), uiBlocksRay);
        }

        public void LockSelection()
        {
            pressedLockInteractableSelection = true;
            UpdateSelectedInteractable(RaycastUtils.GetMousePositionRay(cameraRef.Component), uiBlocksRay: true);
        }

        public void CancelLockSelection()
        {
            lockedInteractable = null;
            UpdateSelectedInteractable(RaycastUtils.GetMousePositionRay(cameraRef.Component), uiBlocksRay: true);
        }

        public void ForceSelect(Interactable interactable)
        {
            ForceSelect(interactable, interactable.transform.position);
        }

        public void ForceSelect(Interactable interactable, Vector3 selectionPoint)
        {
            pressedLockInteractableSelection = true;
            UpdateSelection(interactable, true, selectionPoint);
        }

        private void UpdateSelection(Interactable pointedByCursorInteractable, bool canInteract, Vector3 selectionPoint)
        {
            if (enabled == false)
            {
                return;
            }
            var prevHighlihgtedInteractable = InteractableSelection.HightlightedInteractable;
            var prevSelectedInteractable = InteractableSelection.SelectedIgnoreBlockedInteractable;

            var highlightedInteractable = GetHightlightedInteractable(pointedByCursorInteractable);
            var selectedInteractable = GetSelectedInteractable(highlightedInteractable);

            InteractableSelection.HightlightedInteractable = highlightedInteractable;
            InteractableSelection.SelectedIgnoreBlockedInteractable = selectedInteractable;
            InteractableSelection.SelectedInteractable = canInteract || lockedInteractable == selectedInteractable ? selectedInteractable : null;
            InteractableSelection.AllHitInteractables = allHitInteractables;

            if (selectedInteractable != null && selectedInteractable.NeedsLockingToSelect && canInteract)
            {
                lockedInteractable = selectedInteractable;
            }
            if (pressedLockInteractableSelection && highlightedInteractable == null && lockedInteractable != null &&
                (EventSystem.current == null || EventSystem.current.IsPointerOverGameObject() == false))
            {
                lockedInteractable.Deselect();
                lockedInteractable = null;
            }
            if (prevHighlihgtedInteractable != null && prevHighlihgtedInteractable != highlightedInteractable
                && prevHighlihgtedInteractable != lockedInteractable)
            {
                prevHighlihgtedInteractable.Deselect();
            }
            if (highlightedInteractable != null)
            {
                if (canInteract)
                {
                    highlightedInteractable.Select(Interactable.State.Highlighted, selectionPoint);
                }
                else if (highlightedInteractable == pointedByCursorInteractable)
                {
                    highlightedInteractable.Select(Interactable.State.HightlightedBlocked, selectionPoint);
                }
            }
            if (prevSelectedInteractable != null && prevSelectedInteractable != selectedInteractable
                && prevSelectedInteractable != lockedInteractable)
            {
                prevSelectedInteractable.Deselect();
            }
            if (selectedInteractable != null)
            {
                if (canInteract)
                {
                    selectedInteractable.Select(Interactable.State.Selected, selectionPoint);
                }
                else if (selectedInteractable == pointedByCursorInteractable)
                {
                    selectedInteractable.Select(Interactable.State.HightlightedBlocked, selectionPoint);
                }
            }
            pressedLockInteractableSelection = false;

        }
        private void UpdateSelectedInteractable(Ray ray, bool uiBlocksRay)
        {
            var (pointedByCursorInteractable, canInteract, selectionPoint) = GetPointedByCursorInteractable(ray, uiBlocksRay);
#if UNITY_EDITOR
            if (pointedByCursorInteractable == null)
            {
                hitInteractableGameObject = null;
            }
            else
            {
                hitInteractableGameObject = pointedByCursorInteractable.gameObject;
            }
#endif
            UpdateSelection(pointedByCursorInteractable, canInteract, selectionPoint);


        }

        private Interactable GetHightlightedInteractable(Interactable pointedByCursorInteractable)
        {
#if UNITY_EDITOR
            if (pointedByCursorInteractable == null)
            {
                hitInteractableGameObject = null;
            }
            else
            {
                hitInteractableGameObject = pointedByCursorInteractable.gameObject;
            }
#endif
            if (pointedByCursorInteractable == null || pointedByCursorInteractable.enabled == false
                || pointedByCursorInteractable.gameObject.activeInHierarchy == false)
            {
                return null;
            }
            return pointedByCursorInteractable;

        }

        private Interactable GetSelectedInteractable(Interactable hightlightedInteractable)
        {
            if (hightlightedInteractable == lockedInteractable)
            {
                return hightlightedInteractable;
            }
            bool isValidLockedInteractable = lockedInteractable != null && lockedInteractable.enabled && lockedInteractable.gameObject.activeInHierarchy;
            if (hightlightedInteractable == null)
            {
                if (isValidLockedInteractable)
                {
                    return lockedInteractable;
                }
                else
                {
                    return null;
                }
            }
            if (isValidLockedInteractable)
            {
                if (pressedLockInteractableSelection)
                {
                    return hightlightedInteractable;
                }
                else
                {
                    return lockedInteractable;
                }
            }
            if (hightlightedInteractable.NeedsLockingToSelect)
            {
                if (pressedLockInteractableSelection)
                {
                    return hightlightedInteractable;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return hightlightedInteractable;
            }
        }

        private InteractableSelectionData GetPointedByCursorInteractable(Ray ray, bool uiBlocksRay)
        {
            if (uiBlocksRay && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return default;
            }
            Debug.DrawRay(ray.origin, ray.direction, Color.blue);
            bool isHit = Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, LayersDatabase.SolidLayerMask.value, QueryTriggerInteraction.Ignore);
            allHitInteractables.Clear();
            if (isHit)
            {
                int allHitsCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, LayersDatabase.SolidLayerMask.value, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < allHitsCount; i++)
                {
                    var hit = hits[i];
                    var interactableSelectionData = GetInteractableSelectionDataFromHit(hit);
                    if (interactableSelectionData.Interactable != null)
                    {
                        allHitInteractables.Add(interactableSelectionData);
                    }
                }
                hitGameObject = raycastHit.collider.gameObject;
                return GetInteractableSelectionDataFromHit(raycastHit);
            }
            else
            {
                hitGameObject = null;
            }
            return default;
        }

        private InteractableSelectionData GetInteractableSelectionDataFromHit(RaycastHit raycastHit)
        {
            var isInteractable = raycastHit.collider.TryGetComponent(out InteractableCollider interactableCollider);
            if (isInteractable == false)
            {
                return default;
            }
            var interactable = interactableCollider.Interactable;
            if (interactable == null)
            {
                Debug.LogError($"Interactable on {interactableCollider.gameObject.GetHierarchyPath()} is not assigned");
                return default;
            }
            if (interactable.enabled == false)
            {
                return default;
            }
            if (interactable.IsBlocked())
            {
                return new InteractableSelectionData(interactable, false, raycastHit.point);
            }
            return new InteractableSelectionData(interactable, true, raycastHit.point);
        }
    }
}