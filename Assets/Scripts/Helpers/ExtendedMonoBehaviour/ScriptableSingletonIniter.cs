using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;



public class ScriptableSingletonIniter : AdvancedMonoBehaviour
{
    [SerializeField, Required] private ScriptableSingleton scriptableSingleton;

    public override void OnInit()
    {
        base.OnInit();
        if (scriptableSingleton.IsSet == false)
        {
            scriptableSingleton.Awake();
        }
    }
}