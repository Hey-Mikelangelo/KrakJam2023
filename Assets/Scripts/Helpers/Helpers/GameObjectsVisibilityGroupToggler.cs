using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class GameObjectsVisibilityGroupToggler : MonoBehaviour
{
    [SerializeField] private GameObjectsVisibilityGroupRef visibilityGroupRef = new GameObjectsVisibilityGroupRef();
    [SerializeField] private float delay = 1;
    public void EnableGroup(bool enable)
    {
        StopAllCoroutines();
        if (enable)
        {
            EnableGroupImmediate(true);
        }
        else
        {
            StartCoroutine(EnableGroupDelayed(false, delay));
        }
    }

    public void DisableGroup(bool disable)
    {
        EnableGroup(!disable);
    }
    [Button]
    public void EnableGroup()
    {
        EnableGroup(true);
    }
    [Button]
    public void DisableGroup()
    {
        EnableGroup(false);
    }

    private IEnumerator EnableGroupDelayed(bool enable, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableGroupImmediate(enable);
    }
    private void EnableGroupImmediate(bool enable)
    {
        visibilityGroupRef.Component.EnableGroup(enable);

    }
}
