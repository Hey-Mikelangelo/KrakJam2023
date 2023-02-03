using Sirenix.OdinInspector;
using UnityEngine;
[ExecuteInEditMode]
public class DirectionHandle : MonoBehaviour
{
    [SerializeField] public Quaternion rotation;
    public Vector3 Direction { get => (rotation * Vector3.up).normalized; set => rotation = Quaternion.LookRotation(value); }

}
