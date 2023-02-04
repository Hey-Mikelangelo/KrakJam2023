using System.Collections.Generic;
namespace InteractionSystem
{
    public static class InteractableSelection
    {
        public static Interactable HightlightedInteractable { get; set; }
        public static Interactable SelectedIgnoreBlockedInteractable { get; set; }
        public static Interactable SelectedInteractable { get; set; }

        public static IReadOnlyList<InteractableSelectionData> AllHitInteractables { get; set; }
    }
}