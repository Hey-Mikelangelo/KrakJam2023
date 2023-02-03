using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class InteractableGroupName : ValueDropdown<string, InteractableGroupsNamesTable>
{
    protected override InteractableGroupsNamesTable GetTable()
    {
        return InteractableGroupsNamesTable.Instance;
    }
}



[CreateAssetMenu()]
public class InteractableGroupsNamesTable : ValuesTable<string, InteractableGroupsNamesTable> { }
