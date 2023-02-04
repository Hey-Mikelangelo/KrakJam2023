using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;



public class ScriptableSingletonIniter : MonoBehaviour
{
    [SerializeField, Required] private ScriptableSingleton scriptableSingleton;

    private void Awake()
    {
        if (scriptableSingleton.IsSet == false)
        {
            scriptableSingleton.Awake();
        }
    }
}