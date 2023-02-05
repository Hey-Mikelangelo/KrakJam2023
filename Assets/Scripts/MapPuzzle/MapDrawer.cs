using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class MapDrawer : MonoBehaviour
{
    [SerializeField] private List<Transform> pointsTransforms = new();
    [SerializeField] private Transform firstPoint, lastPoint;
    [SerializeField] private CameraGuidReference cameraRef = new();
    [SerializeField] private InputActionReference placePointInputActionRef;
    [SerializeField, Required] private LineRenderer lineRenderer;
    [SerializeField, Required] private Button firstPointButton, lastPointButton;
    [SerializeField, Required] private InputActionReference cancelPlacingInputActionRef;
    private Vector2[] points;
    private InputActionWrapper placePointInputAction, cancelPlacingInputAction;
    private Vector3 nearestPoint;
    private bool startedPlacing;
    private List<Vector2> selectedPoints = new(); 
    private void Awake()
    {
        points = pointsTransforms.Select(x => x != null ? (Vector2)x.position : Vector2.zero).ToArray();
        placePointInputAction = new(placePointInputActionRef.action, PlacePoint);
        cancelPlacingInputAction = new(cancelPlacingInputActionRef.action, CancelPlacing);
        firstPointButton.onClick.AddListener(StartPlacing);
        lastPointButton.onClick.AddListener(FinishPlacing);
        ClearPath();
    }
    private void OnDestroy()
    {
        placePointInputAction.Unsubscribe();
        cancelPlacingInputAction.Unsubscribe();
        firstPointButton.onClick.RemoveListener(StartPlacing);
        lastPointButton.onClick.RemoveListener(FinishPlacing);
    }
    private void Update()
    {
        var mousePos = GetCursorPosition();
        nearestPoint = GetNearestPoint(mousePos);
        if (startedPlacing)
        {

        }
    }

    public void FinishPlacing()
    {

    }
    public void CancelPlacing()
    {
        startedPlacing = false;
        selectedPoints.Clear();
    }
    public void StartPlacing()
    {
        startedPlacing = true;
        selectedPoints.Clear();
        selectedPoints.Add(firstPoint.position);
    }
    private void PlacePoint()
    {

    }

    private void ClearPath()
    {
        lineRenderer.SetPositions(new Vector3[0]);

    }
    private Vector2 GetNearestPoint(Vector2 position)
    {
        int count = points.Length;
        if(count == 0)
        {
            Debug.LogError("No points");
            return Vector2.zero;

        }
        float nearestDistance = float.MaxValue;
        int nearestPointIndex = -1;
        for (int i = 0; i < count; i++)
        {
            var point = points[i];
            var distance = Vector2.Distance(position, point);
            if (distance < nearestDistance)
            {
                nearestPointIndex = i;
                nearestDistance = distance;
            }
        }
        return points[nearestPointIndex];
    }
    private Vector2 GetCursorPosition()
    {
        Vector3 mousePos = RaycastUtils.GetMousePositionRay(cameraRef.Component).origin;
        return mousePos;
    }

}
