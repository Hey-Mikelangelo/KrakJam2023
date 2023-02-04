using Sirenix.OdinInspector;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField, Required] private PlayerController playerController;
    public PlayerController PlayerController => playerController;
}
