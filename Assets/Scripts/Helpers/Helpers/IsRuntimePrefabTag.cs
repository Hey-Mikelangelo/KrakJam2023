using UnityEngine;

/// <summary>
/// tag to ensure that runtime prefab components will not be sucked up into loading system in "Awake" and will stay untouched after instantiation
/// </summary>
public class IsRuntimePrefabTag : MonoBehaviour { } 
