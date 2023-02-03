using System;
using System.Collections.Generic;
public static class TypeExtensions
{
    public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }

    public static bool IsSubclassOfRawGenericInterface(this Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var interfacesTypes = toCheck.GetInterfaces();
            foreach (var interfaceType in interfacesTypes)
            {
                var cur = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
                if (generic == cur)
                {
                    return true;
                }
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }
}
