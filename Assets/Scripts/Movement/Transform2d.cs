using UnityEngine;

public class Transform2d : MonoBehaviour
{
    private void FixedUpdate()
    {
        if(transform.position.z != 0)
        {
            transform.position = transform.position.WithZ(0);
        }
    }
}
