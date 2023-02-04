using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace InteractionSystem
{
    public sealed class Interactable : MonoBehaviour
    {
        [SerializeField, ReadOnly] private InteractionAction[] interactionActionsComponents = new InteractionAction[0];
        [SerializeField, Required] private SelectionHighlightBehaviour selectionBehaviour;
        [SerializeField] private InteractableCollider interactableCollider;
        [SerializeField] private bool needsLockingToSelect;
        [SerializeField] private bool controlCollidersEnabledStatus = true;
        [SerializeField, OdinSerialize] public UltEvent OnSelected = new();
        [SerializeField, OdinSerialize] public UltEvent OnDeselected = new();
        [SerializeField, OdinSerialize] public UltEvent<bool> OnEnabledStatusChanged = new();
        public event System.Action OnHightlighted;
        public event System.Action OnDeHightlighted;
        public bool IsEnabledAndActive { get; private set; }
        [ShowInInspector, ReadOnly] private State currentState;
        private Vector3 selectionPoint;
        private InputAction[] inputActions;
        public State CurrentState => currentState;
        public Vector3 SelectionPoint => selectionPoint;
        public IReadOnlyList<InteractionAction> InteractionActions => interactionActionsComponents;
        public InteractableCollider InteractableCollider => interactableCollider;
        public bool NeedsLockingToSelect => needsLockingToSelect;

        public event System.Action OnActionEnabledStatusChanged;
        private void Awake()
        {
            RefreshInteractionActionsArray();
            FillInputActionsArray();
            SubscribeToInteractionActionsEnabledStatusChange();
        }
        private void OnDestroy()
        {
            UnsubscribeFromInteractionActionsEnabledStatusChange();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            RefreshInteractionActionsArray();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }
            AssingInteractableToCollidersGroupInEditor();
        }
        private void AssingInteractableToCollidersGroupInEditor()
        {
            var prevInteractableCollidersGroup = interactableCollider;
            if (interactableCollider != null)
            {
                interactableCollider.SetInteractable(this);
            }
            if (prevInteractableCollidersGroup != null)
            {
                prevInteractableCollidersGroup.SetInteractable(null);
            }
        }
#endif
        private void Start()
        {
            if (interactableCollider.Is_Not_NullWithErrorLog())
            {
                interactableCollider.SetInteractable(this);
                if (controlCollidersEnabledStatus)
                {
                    EnableColliders(this.IsEnabledAndActive);
                }
            }
            else
            {
                Debug.LogWarning($"No colliders grop on interactable {this.gameObject.name}");
            }
        }

        private void OnEnable()
        {
            IsEnabledAndActive = true;
            if (controlCollidersEnabledStatus)
            {
                EnableColliders(true);
            }

            OnEnabledStatusChanged.Invoke(true);

        }

        private void OnDisable()
        {
            IsEnabledAndActive = false;
            if (this.name.Contains("Custom"))
            {
                Debug.Log("Disabled custom interactable");
            }
            if (controlCollidersEnabledStatus)
            {
                EnableColliders(false);
            }

            Deselect();

            OnEnabledStatusChanged.Invoke(false);

        }

        public void Deselect()
        {
            SetSelected(State.NotSelected, default);

        }

        public void Hightlight(State state)
        {
            if (selectionBehaviour == null)
            {
                selectionBehaviour.LogNullError();
            }
            if (currentState == State.Selected)
            {
                state = State.Selected;
            }
            var prevSelectedState = selectionBehaviour.State;
            selectionBehaviour.SetSelected(state);
            if (prevSelectedState != State.NotSelected)
            {
                if (selectionBehaviour.State == State.NotSelected)
                {
                    OnDeHightlighted?.Invoke();
                }
            }
            else
            {
                if (selectionBehaviour.State != State.NotSelected)
                {
                    OnHightlighted?.Invoke();
                }
            }
        }

        public void Select(State state, Vector3 selectionPoint)
        {
            SetSelected(state, selectionPoint);
        }

        public void EnableColliders(bool enable = true)
        {
            if (this.interactableCollider.IsNullWithErrorLog(this.name))
            {
                return;
            }
            interactableCollider.Collider.isTrigger = !enable;
        }

        private void SetSelected(State state, Vector3 selectionPoint)
        {
            if (currentState == State.Selected && state == State.Highlighted)
            {
                state = State.Selected;
            }
            bool isSelected = state == State.Selected;
            bool prevIsSelected = currentState == State.Selected;
            State prevState = currentState;

            currentState = state;
            this.selectionPoint = selectionPoint;

            if (prevState != state)
            {
                Hightlight(state);

            }

            if (prevIsSelected != isSelected)
            {
                foreach (var inputAction in inputActions)
                {
                    inputAction.Enable(isSelected);
                }
                if (isSelected)
                {
                    OnSelected.Invoke();
                }
                else
                {
                    OnDeselected.Invoke();
                }
            }
        }

        public void StartInteraction(InputAction triggerringInputAction)
        {
            if (IsBlocked())
            {
                return;
            }
            var interactionAction = GetInteractionAction(triggerringInputAction);
            if (interactionAction == null)
            {
                return;
            }
            interactionAction.StartInteraction();
        }

        public void Interact(InputAction triggerringInputAction)
        {
            if (IsBlocked())
            {
                return;
            }
            var interactionAction = GetInteractionAction(triggerringInputAction);
            if (interactionAction == null)
            {
                return;
            }
            interactionAction.Interact();
        }

        public void CancelInteraction(InputAction triggerringInputAction)
        {
            var interactionAction = GetInteractionAction(triggerringInputAction);
            if (interactionAction == null)
            {
                return;
            }
            interactionAction.CancelInteraction();
        }

        public bool IsBlocked()
        {
            if (this.TryGetComponent(out InteractableBlocker interactableBlocker))
            {
                return interactableBlocker.IsBlocking();
            }
            return false;
        }

        private InteractionAction GetInteractionAction(InputAction triggerringInputAction)
        {
            int actionsCount = inputActions.Length;
            for (int i = 0; i < actionsCount; i++)
            {
                var inputAction = inputActions[i];
                if (inputAction.id == triggerringInputAction.id)
                {
                    var interactionAction = interactionActionsComponents[i];
                    if (interactionAction.enabled)
                    {
                        return interactionAction;
                    }
                }
            }
            //Debug.LogError($"No interaction action with triggerring input action {triggerringInputAction.name} on interactable {this.gameObject}");
            return null;
        }

        private void FillInputActionsArray()
        {
            int count = interactionActionsComponents.Length;
            inputActions = new InputAction[count];
            for (int i = 0; i < count; i++)
            {
                inputActions[i] = interactionActionsComponents[i].TriggerringInputAction;
            }
        }

        [Button]
        private void RefreshInteractionActionsArray()
        {
            interactionActionsComponents = GetComponents<InteractionAction>();
        }

        private void SubscribeToInteractionActionsEnabledStatusChange()
        {
            int count = interactionActionsComponents.Length;
            for (int i = 0; i < count; i++)
            {
                var action = interactionActionsComponents[i];
                action.OnEnabledStatusChanged += Action_OnEnabledStatusChanged;
            }
        }

        private void UnsubscribeFromInteractionActionsEnabledStatusChange()
        {
            int count = interactionActionsComponents.Length;
            for (int i = 0; i < count; i++)
            {
                var action = interactionActionsComponents[i];
                action.OnEnabledStatusChanged -= Action_OnEnabledStatusChanged;
            }
        }

        private void Action_OnEnabledStatusChanged()
        {
            OnActionEnabledStatusChanged?.Invoke();
        }

        public enum State
        {
            NotSelected = 0,
            Highlighted = 1,//object which needs locking to select will be hightlighted when poited by cursor
            HightlightedBlocked = 2,    //object which is highlighted or selected but interaction is blocked
            Selected = 3    //object which needs locking will be selected after locked. Objects which does not need locking to be selected will be selected immediately after highlighted
        }
    }
}