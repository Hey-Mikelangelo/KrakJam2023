using Sirenix.OdinInspector;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField, Required] private PlayerMovementController playerController;
    public PlayerMovementController PlayerController => playerController;
}
