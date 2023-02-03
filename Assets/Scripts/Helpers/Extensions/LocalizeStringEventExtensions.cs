using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public static class LocalizeStringEventExtensions
{
    private static LocalizedString emptyLocalizedString = new LocalizedString();
    public static void UpdateStringToNone(this LocalizeStringEvent localizeStringEvent)
    {
        localizeStringEvent.StringReference = emptyLocalizedString;
        localizeStringEvent.OnUpdateString.Invoke("---------");
    }
    public static void UpdateStringToNew(this LocalizeStringEvent localizeStringEvent, LocalizedString localizedString)
    {
        try
        {
            if (localizedString == null || localizedString.IsEmpty)
            {
                localizeStringEvent.UpdateStringToNone();
                return;
            }
            //arguments are runtime only. If keys and values was set in editor, they need to be copied as arguments
            if (localizedString.Arguments == null)
            {
                localizedString.SetArgumentsFromKeysAndValues();
            }
            localizeStringEvent.StringReference = localizedString;
            localizeStringEvent.RefreshString();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

    }
}
