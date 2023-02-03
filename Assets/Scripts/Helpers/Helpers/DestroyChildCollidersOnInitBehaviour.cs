using UnityEngine;

public class DestroyChildCollidersOnInitBehaviour : InitableMonoBehaviour
{
    public override void OnInit()
    {
        base.OnInit();
        var childColliders = GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < childColliders.Length; i++)
        {
            Destroy(childColliders[i]);
        }
    }
}
