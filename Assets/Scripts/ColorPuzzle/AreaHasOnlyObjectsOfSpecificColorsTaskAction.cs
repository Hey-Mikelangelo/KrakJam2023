using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class AreaHasOnlyObjectsOfSpecificColorsTaskAction : TaskAction
{
    [SerializeField, Required] private BoundsCollidersTracker room1BoundsCollidersTracker;
    [SerializeField, Required] private BoundsCollidersTracker room2BoundsCollidersTracker;
    [SerializeField] private List<ColorSO> validColorsSO = new();
    private List<Color> validColors = new();
    private void Awake()
    {
        validColors = validColorsSO.Select(x => x != null ? x.Value : Color.magenta).ToList();
    }

    public override bool IsCompleted()
    {
        var colliders1 = room1BoundsCollidersTracker.CollidersInsideBounds;
        var colliders2 = room2BoundsCollidersTracker.CollidersInsideBounds;
        var isOnlyValidColorsInRoom1 = IsOnlyValidColorsInBoundsColliders(colliders1);
        var isOnlyValidColorsInRoom2 = IsOnlyValidColorsInBoundsColliders(colliders2);
        var isOnlyNotValidCollidersInRoom1 = IsOnlyNotValidColorsInBoundsColliders(colliders1);
        var isOnlyNotValidCollidersInRoom2 = IsOnlyNotValidColorsInBoundsColliders(colliders2);
        return (isOnlyValidColorsInRoom1 && isOnlyNotValidCollidersInRoom2) || (isOnlyValidColorsInRoom2 && isOnlyNotValidCollidersInRoom1);
    }

    private bool IsOnlyNotValidColorsInBoundsColliders(ReadOnlyArray<Collider> colliders)
    {
        int count = colliders.Count;
        bool hasAnyColoredObjects = false;
        for (int i = 0; i < count; i++)
        {
            var collider = colliders[i];
            if (collider.IsNullWithErrorLog())
            {
                continue;
            }
            if (collider.TryGetComponent(out ObjectColorTag objectColorTag))
            {
                hasAnyColoredObjects = true;
                if (validColors.Contains(objectColorTag.Color))
                {
                    return false;
                }
            }
        }
        return true;
    }
    private bool IsOnlyValidColorsInBoundsColliders(ReadOnlyArray<Collider> colliders)
    {
        int count = colliders.Count;
        bool hasAnyColoredObjects = false;
        for (int i = 0; i < count; i++)
        {
            var collider = colliders[i];
            if (collider.IsNullWithErrorLog())
            {
                continue;
            }
            if (collider.TryGetComponent(out ObjectColorTag objectColorTag))
            {
                hasAnyColoredObjects = true;
                if (validColors.Contains(objectColorTag.Color) == false)
                {
                    return false;
                }
            }
        }
        if (hasAnyColoredObjects)
        {
            return true;
        }
        return false;
    }
}
