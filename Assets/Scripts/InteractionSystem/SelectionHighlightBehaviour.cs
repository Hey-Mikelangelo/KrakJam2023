using UnityEngine;
namespace InteractionSystem
{
    public abstract class SelectionHighlightBehaviour : MonoBehaviour
    {
        public abstract void ShowHighlight(bool show);
        public abstract void SetSelected(Interactable.State interactableState);
        public abstract Interactable.State State { get; }
        public void ShowHighlight()
        {
            ShowHighlight(true);
        }
        public void Hidehighlight()
        {
            ShowHighlight(false);
        }

    }
}