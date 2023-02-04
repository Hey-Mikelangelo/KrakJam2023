using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
namespace InteractionSystem
{
    [RequireComponent(typeof(Interactable))]
    public sealed class InteractionAction : MonoBehaviour
    {
        [SerializeField, Required, LabelText("Triggerring Action")]
        private InputActionReference triggerringActionReference;
        [SerializeField] private LocalizedString interactionActionName;
        [SerializeField, OdinSerialize] private UltEvent onStarted = new UltEvent();
        [SerializeField, OdinSerialize] private UltEvent onPerformed = new UltEvent();
        [SerializeField, OdinSerialize] private UltEvent onCancelled = new UltEvent();
        private Interactable interactable;
        public InputAction TriggerringInputAction => triggerringActionReference.action;
        public LocalizedString ActionName => interactionActionName;
        public event System.Action OnEnabledStatusChanged;

        private static List<InputAction> possibleTriggeringInputActions = new List<InputAction>();
        public static List<InputAction> PossibleTriggeringInputActions => possibleTriggeringInputActions;

        public static event System.Action<InputAction> OnNewTriggerringInputActionAdded;
        public bool IsBlocked => false;

        private void OnEnable()
        {

            OnEnabledStatusChanged?.Invoke();
        }

        private void OnDisable()
        {
            OnEnabledStatusChanged?.Invoke();
            var _ = this.name;
        }
        private void Awake()
        {
            interactable = GetComponent<Interactable>();
        }
        private void Start()
        {
            if (triggerringActionReference.IsNullWithErrorLog())
            {
                return;
            }
            var inputAction = triggerringActionReference.action;
            bool addedNew = possibleTriggeringInputActions.AddDistinct(inputAction);
            if (addedNew)
            {
                OnNewTriggerringInputActionAdded?.Invoke(inputAction);
            }
            interactable.OnSelected += OnInteractableSelected;
            interactable.OnDeselected += OnInteractableDeselected;
        }

        private void OnDestroy()
        {
            interactable.OnSelected -= OnInteractableSelected;
            interactable.OnDeselected -= OnInteractableDeselected;
        }

        public void StartInteraction()
        {
            if (IsBlocked)
            {
                Debug.LogError("action is blocked");
                return;
            }
            onStarted.Invoke();
        }
        public void Interact()
        {
            if (IsBlocked)
            {
                Debug.LogError("action is blocked");
                return;
            }
            onPerformed.Invoke();
        }

        public void CancelInteraction()
        {
            if (IsBlocked)
            {
                Debug.LogError("action is blocked");
                return;
            }
            onCancelled.Invoke();
        }

        private void OnInteractableSelected(Vector3 selectionPoint)
        {
            if (triggerringActionReference.action.Is_Not_NullWithErrorLog())
            {
                triggerringActionReference.action.Enable();
            }
        }
        private void OnInteractableDeselected()
        {
            if (triggerringActionReference.action.Is_Not_NullWithErrorLog())
            {
                triggerringActionReference.action.Disable();
            }
        }
    }
}