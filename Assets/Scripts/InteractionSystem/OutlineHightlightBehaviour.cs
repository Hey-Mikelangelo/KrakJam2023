using Sirenix.OdinInspector;
using UnityEngine;
namespace InteractionSystem
{
    public class OutlineHightlightBehaviour : SelectionHighlightBehaviour
    {
        [SerializeField, Required] private QuickOutline.Outline outline;
        private Interactable.State state;
        public override Interactable.State State => state;
        public override void ShowHighlight(bool show)
        {
            outline.enabled = show;
        }
        public override void SetSelected(Interactable.State interactableState)
        {
            if (outline.IsNullWithErrorLog())
            {
                return;
            }
            state = interactableState;
            switch (interactableState)
            {
                case Interactable.State.NotSelected:
                    outline.enabled = false;
                    break;
                case Interactable.State.Highlighted:
                    outline.enabled = true;
                    outline.OutlineColor = InteractionSystemSharedValuesSO.Instance.HightlightedInteractableOutlineColor;
                    break;
                case Interactable.State.HightlightedBlocked:
                    outline.enabled = true;
                    outline.OutlineColor = InteractionSystemSharedValuesSO.Instance.BlockedInteractableOutlineColor;
                    break;
                case Interactable.State.Selected:
                    outline.enabled = true;
                    outline.OutlineColor = InteractionSystemSharedValuesSO.Instance.SelectedInteractableOutlineColor;
                    break;
                default:
                    throw new System.Exception($"Not implemented case for Interactable state {interactableState}");

            }
        }
    }
}