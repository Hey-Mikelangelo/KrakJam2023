using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct InputActionMapName
{
    [SerializeField, Required] private InputActionAsset inputActionAsset;
    [SerializeField, ValueDropdown(nameof(GetActionMapsNames), IsUniqueList = true)] private string actionMapName;

    public string Name => actionMapName;

    private IEnumerable<string> GetActionMapsNames()
    {
        if(inputActionAsset == null)
        {
            return null;
        }    
        return inputActionAsset.actionMaps.Select(x => x.name);
    }
}
