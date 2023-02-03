using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public static class LocalizedStringExtensions
{
    public static void SetArguments(this LocalizedString localizedString, params (string key, object value)[] argumentsPairs)
    {
        localizedString.SetArguments((IReadOnlyList<(string key, object value)>)argumentsPairs);
    }

    public static void SetArgumentsFromKeysAndValues(this LocalizedString localizedString)
    {
        var keys = localizedString.Keys;
        var values = localizedString.Values;
        if(keys.IsNullWithErrorLog() || values.IsNullWithErrorLog())
        {
            return;
        }
        object[] arguments = new object[1];
        var dict = new ListDictionary<string, object>();
        foreach (var key in keys)
        {
            dict.Add(key, null);
        }
        int index = 0;
        foreach (var value in values)
        {
            dict.SetValueAtIndex(index, value);
            index++;
        }
        arguments[0] = dict;

        localizedString.Arguments = arguments;
    }
    public static void SetArguments(this LocalizedString localizedString, IReadOnlyList<(string key, object value)> argumentsPairs)
    {
        object[] arguments = new object[1];
        var dict = new Dictionary<string, object>();
        for (int i = 0; i < argumentsPairs.Count; i++)
        {
            var (key, value) = argumentsPairs[i];
            bool added = dict.TryAdd(key, value);
            if (added == false)
            {
                Debug.LogError($"Duplicate localization key {key}");
            }
        }
        arguments[0] = dict;

        localizedString.Arguments = arguments;
    }
}
