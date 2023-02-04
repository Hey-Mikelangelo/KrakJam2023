using UnityEngine;
namespace InteractionSystem
{
    public struct InteractableSelectionData
    {
        public Interactable Interactable;
        public bool CanInteract;
        public Vector3 SelectionPoint;

        public InteractableSelectionData(Interactable interactable, bool canInteract, Vector3 selectionPoint)
        {
            Interactable = interactable;
            CanInteract = canInteract;
            SelectionPoint = selectionPoint;
        }

        public void Deconstruct(out Interactable interactable, out bool canInteract, out Vector3 selectionPoint)
        {
            interactable = this.Interactable;
            canInteract = this.CanInteract;
            selectionPoint = this.SelectionPoint;
        }
    }
}