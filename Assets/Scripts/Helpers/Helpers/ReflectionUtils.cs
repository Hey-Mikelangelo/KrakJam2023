using System.Reflection;
using UnityEngine;

public static class ReflectionUtils
{
    public static bool IsOverride(this MethodInfo methodInfo)
    {
        return (methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType);
    }
}
