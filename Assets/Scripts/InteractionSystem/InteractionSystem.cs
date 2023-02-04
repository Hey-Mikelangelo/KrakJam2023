using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace InteractionSystem
{
    public sealed class InteractionSystem : Singleton<InteractionSystem>
    {
        private List<InputAction>
            startedInputActions = new(3),
            performedInputActions = new(3),
            cancelledInputActions = new(3);

        private List<InputAction> bufferedCancelledInputActions = new(3);
        private void Start()
        {
            SubcribeToTriggerringInputActions();
            InteractionAction.OnNewTriggerringInputActionAdded += InteractionAction_OnNewTriggerringInputActionAdded;
        }

        private void OnDestroy()
        {
            UnsubcribeFromTriggerringInputActions();
        }

        private void Update()
        {
            var interactable = InteractableSelection.SelectedInteractable;
            if (interactable == null)
            {
                return;
            }
            /*if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }*/
            if (cancelledInputActions.Count != 0)
            {
                try
                {
                    for (int i = 0; i < cancelledInputActions.Count; i++)
                    {
                        try
                        {
                            interactable.CancelInteraction(cancelledInputActions[i]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                finally
                {
                    cancelledInputActions.Clear();
                }
            }
            if (startedInputActions.Count != 0)
            {
                try
                {
                    for (int i = 0; i < startedInputActions.Count; i++)
                    {
                        try
                        {
                            interactable.StartInteraction(startedInputActions[i]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                finally
                {
                    startedInputActions.Clear();
                }
            }
            if (performedInputActions.Count != 0)
            {
                try
                {
                    for (int i = 0; i < performedInputActions.Count; i++)
                    {
                        try
                        {
                            interactable.Interact(performedInputActions[i]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                finally
                {
                    performedInputActions.Clear();
                }
            }

        }

        private void LateUpdate()
        {
            startedInputActions.Clear();
            performedInputActions.Clear();
            cancelledInputActions.Clear();
            bufferedCancelledInputActions.Clear();

        }

        private void UnsubcribeFromTriggerringInputActions()
        {
            int count = InteractionAction.PossibleTriggeringInputActions.Count;
            for (int i = 0; i < count; i++)
            {
                var inputAction = InteractionAction.PossibleTriggeringInputActions[i];
                UnsubscribeFromInputAction(inputAction);
            }
        }
        private void SubcribeToTriggerringInputActions()
        {
            int count = InteractionAction.PossibleTriggeringInputActions.Count;
            for (int i = 0; i < count; i++)
            {
                var inputAction = InteractionAction.PossibleTriggeringInputActions[i];
                SubscribeToInputAction(inputAction);
            }
        }

        private void SubscribeToInputAction(InputAction inputAction)
        {
            if (inputAction == null)
            {
                return;
            }
            //Debug.Log($"Subcribe to action {inputAction.name} {inputAction.GetHashCode()}");
            inputAction.performed += InputAction_performed;
            inputAction.started += InputAction_started;
            inputAction.canceled += InputAction_canceled;
        }

        private void UnsubscribeFromInputAction(InputAction inputAction)
        {
            if (inputAction == null)
            {
                return;
            }
            inputAction.performed -= InputAction_performed;
            inputAction.started -= InputAction_started;
            inputAction.canceled -= InputAction_canceled;
        }

        private void InteractionAction_OnNewTriggerringInputActionAdded(InputAction inputAction)
        {
            SubscribeToInputAction(inputAction);
        }

        private void InputAction_canceled(InputAction.CallbackContext obj)
        {
            bufferedCancelledInputActions.Add(obj.action);
            //Debug.Log($"Cancelled {obj.action.name}");
        }

        private void InputAction_started(InputAction.CallbackContext obj)
        {
            startedInputActions.Add(obj.action);

        }

        private void InputAction_performed(InputAction.CallbackContext obj)
        {
            performedInputActions.Add(obj.action);
            Debug.Log($"Performed {obj.action.name}");
        }
    }
}