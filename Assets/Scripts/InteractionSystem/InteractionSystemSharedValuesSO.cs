using UnityEngine;
namespace InteractionSystem
{
    [CreateAssetMenu]
    public class InteractionSystemSharedValuesSO : ScriptableSingleton<InteractionSystemSharedValuesSO>
    {
        [SerializeField] private Color hightlightedInteractableOutlineColor;
        [SerializeField] private Color blockedInteractableOutlineColor;
        [SerializeField] private Color selectedInteractableOutlineColor;
        public Color HightlightedInteractableOutlineColor => hightlightedInteractableOutlineColor;
        public Color BlockedInteractableOutlineColor => blockedInteractableOutlineColor;
        public Color SelectedInteractableOutlineColor => selectedInteractableOutlineColor;

    }
}