using Sirenix.OdinInspector;
using UnityEngine;
namespace InteractionSystem
{
    [RequireComponent(typeof(Collider))]
    public class InteractableCollider : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private Interactable interactable;
        [ShowInInspector, ReadOnly] private Collider collider;
        public Interactable Interactable => interactable;
        public Collider Collider
        {
            get
            {
                if(collider == null)
                {
                    collider = GetComponent<Collider>();
                }
                return collider;
            }
        }
        private void Awake()
        {
            _ = Collider;
        }
        private void Reset()
        {
            this.gameObject.layer = LayerMask.NameToLayer(LayerNames.Interactable);
        }

        
        public void SetInteractable(Interactable interactable)
        {
            this.interactable = interactable;
        }

    }
}