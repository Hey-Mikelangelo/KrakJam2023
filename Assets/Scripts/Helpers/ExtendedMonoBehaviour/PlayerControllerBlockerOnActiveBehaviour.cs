using UnityEngine;
public class PlayerControllerBlockerOnActiveBehaviour : MonoBehaviour
{
    private PlayerController disabledController;
    private bool prevActiveStatus;
    private void OnEnable()
    {
        disabledController = PlayerController.ActiveController;
        if (disabledController != null)
        {
            prevActiveStatus = disabledController.IsInputActive;
            disabledController.SetInputActive(false);
        }
    }

    private void OnDisable()
    {
        if (disabledController != null)
        {
            disabledController.SetInputActive(prevActiveStatus);
        }
    }
}
