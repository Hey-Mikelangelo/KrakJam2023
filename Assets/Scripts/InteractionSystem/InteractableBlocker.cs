using UnityEngine;
namespace InteractionSystem
{
    public abstract class InteractableBlocker : MonoBehaviour
    {
        public abstract bool IsBlocking();
    }
}