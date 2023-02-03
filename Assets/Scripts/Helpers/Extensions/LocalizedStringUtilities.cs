using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public static class LocalizedStringUtilities
{
    public static LocalizedString GetLocalizedString(LocalizedStringTable localizedStringTable, string key,
        IReadOnlyList<(string localeId, string localizedName)> localizedNames, out List<StringTable> affectedTables)
    {
        LocalizedString nameLocalizedString = null;
         affectedTables = new();
        try
        {
            if (localizedNames.Count != 0)
            {
                var availableLocales = LocalizationSettings.AvailableLocales.Locales;
                var stringDatabase = LocalizationSettings.StringDatabase;
                var tableReference = localizedStringTable.TableReference;
                foreach ((string localeId, string localizedName) in localizedNames)
                {
                    var matchingLocale = availableLocales.FirstOrDefault(x => x.Identifier == new LocaleIdentifier(localeId));
                    if (matchingLocale != null)
                    {
                        StringTable table = stringDatabase.GetTable(tableReference, matchingLocale);
                        SharedTableData sharedTableData = table.SharedData;
                        var tableEntry = sharedTableData.GetEntryFromReference(key);
                        if (tableEntry == null || table.ContainsKey(tableEntry.Id) == false)
                        {
                            table.AddEntry(key, $"{localizedName}");
                            affectedTables.AddDistinct(table);
                        }
                        else
                        {
                            Debug.Log($"key { key} already exist in a table {table.name}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Locale {localeId} not found");
                    }
                }
                nameLocalizedString = new LocalizedString(tableReference, key);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        if (nameLocalizedString == null)
        {
            nameLocalizedString = new LocalizedString();
        }
        return nameLocalizedString;

    }
}
