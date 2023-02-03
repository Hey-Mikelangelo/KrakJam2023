using Sirenix.OdinInspector;
using UnityEngine;

public class RuntimeTagSetter : MonoBehaviour
{
    [Button]
    public void AddTagComponents()
    {
        if (TryGetComponent(out InitableMonoBehaviour _) && TryGetComponent(out IsRuntimePrefabTag _) == false)
        {
            gameObject.AddComponent<IsRuntimePrefabTag>();
        }
        foreach (Transform child in this.transform)
        {
            if(child.TryGetComponent(out InitableMonoBehaviour _) && child.TryGetComponent(out IsRuntimePrefabTag _) == false)
            {
                child.gameObject.AddComponent<IsRuntimePrefabTag>();
            }
        }
    }
}