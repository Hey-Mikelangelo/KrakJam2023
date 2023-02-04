using UnityEngine;
namespace InteractionSystem
{
    [RequireComponent(typeof(Interactable))]
    public class PlayerTooFarInteractableBlocker : InteractableBlocker
    {
        [SerializeField] private float maxDistance = 1;
        private Interactable interactable;
        private void Awake()
        {
            interactable = GetComponent<Interactable>();
        }
        public override bool IsBlocking()
        {
            return Vector2.Distance(Player.Instance.transform.position, interactable.transform.position) > maxDistance;
        }
    }
}