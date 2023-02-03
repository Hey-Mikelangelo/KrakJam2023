using Sirenix.OdinInspector;
using System;
using UnityEngine;

[System.Serializable]
public class GameObjectsVisibilityGroupRef : GuidReference<GameObjectsVisibilityGroup> { }
public class GameObjectsVisibilityGroup : MonoBehaviour, ICrossSceneComponent<GameObjectsVisibilityGroup>
{
    [SerializeField] private GameObject[] gameObjects = new GameObject[0];
    [SerializeField] private SerializableGuid guid;
    public event Action<ICrossSceneComponent> OnDestroyed;

    public Guid Guid => guid;

    protected void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }
    public void EnableGroup(bool enable)
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(enable);
        }
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
}
