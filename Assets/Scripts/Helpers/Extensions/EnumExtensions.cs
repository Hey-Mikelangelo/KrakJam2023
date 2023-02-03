using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class EnumExtensions
{
    public static IEnumerable<T> GetFlags<T>(this T enumValue, bool withZeroValue = false) where T : Enum
    {
        if (enumValue.IsFlagEnum() == false)
        {
            throw new Exception($"{enumValue.GetType().Name} Enum is not a flag enum but you are trying to get flags from it");
        }
        var enumValues = Enum.GetValues(enumValue.GetType());
        foreach (T value in enumValues)
        {
            int intValue = Convert.ToInt32(value);
            bool hasFlag = enumValue.HasFlag(value);
            if (hasFlag && (withZeroValue || intValue != 0))
            {
                yield return value;
            }
        }
    }
    public static bool IsFlagEnum(this Enum enumValue)
    {
        return enumValue.GetType().IsDefined(typeof(FlagsAttribute), false);
    }
    public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
    {
        return enumValue.GetType()?
                        .GetMember(enumValue.ToString())?
                        .First()?
                        .GetCustomAttribute<T>();
    }
}