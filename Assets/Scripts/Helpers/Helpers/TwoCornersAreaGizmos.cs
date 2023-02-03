using Sirenix.OdinInspector;
using UnityEngine;

public class TwoCornersAreaGizmos : MonoBehaviour
{
    [SerializeField, Required] private Transform corner1, corner2;
    [SerializeField, Required] private Transform areaTransform;
    [SerializeField] private Axis flatAxis = Axis.Y;
    public void OnDrawGizmos()
    {
        if (corner1 == null || corner2 == null || areaTransform == null)
        {
            return;
        }
        DrawArea();
    }

    private void DrawArea()
    {
        var localToWorldMatrix = Matrix4x4.TRS(areaTransform.position, areaTransform.rotation, areaTransform.lossyScale);
        Matrix4x4 worldToLocalMatrix = localToWorldMatrix.inverse;
        Vector2 corner1LocalPosition = worldToLocalMatrix.MultiplyPoint(corner1.position).GetVector2WithRemovedValueOnAxis(flatAxis);
        Vector2 corner2LocalPosition = worldToLocalMatrix.MultiplyPoint(corner2.position).GetVector2WithRemovedValueOnAxis(flatAxis);

        var (minLocal, maxLocal) = MathUtils.GetMinMax(corner1LocalPosition, corner2LocalPosition);
        var size = maxLocal - minLocal;
        Vector3 min = localToWorldMatrix.MultiplyPoint3x4(minLocal.GetVector3WithValueOnAxis(flatAxis, 0));
        Vector3 max = localToWorldMatrix.MultiplyPoint3x4(maxLocal.GetVector3WithValueOnAxis(flatAxis, 0));
        Vector3 minXMaxY = localToWorldMatrix.MultiplyPoint3x4(new Vector2(minLocal.x, maxLocal.y).GetVector3WithValueOnAxis(flatAxis, 0));
        Vector3 maxXminY = localToWorldMatrix.MultiplyPoint3x4(new Vector2(maxLocal.x, minLocal.y).GetVector3WithValueOnAxis(flatAxis, 0));

        Vector3 sizeVector = max - min;
        Gizmos.DrawSphere(min, 0.01f);
        Gizmos.DrawSphere(max, 0.01f);

        Vector3 center = min + sizeVector;
        GizmosExtend.DrawLine(min, minXMaxY);
        GizmosExtend.DrawLine(minXMaxY, max);
        GizmosExtend.DrawLine(max, maxXminY);
        GizmosExtend.DrawLine(maxXminY, min);

    }
}
