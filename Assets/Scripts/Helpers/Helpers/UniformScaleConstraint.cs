using UnityEngine;

[ExecuteAlways]
public class UniformScaleConstraint : MonoBehaviour
{
    private void Update()
    {
        var scale = this.transform.localScale;
        scale.y = scale.x;
        scale.z = scale.x;
        this.transform.localScale = scale;
    }
}
