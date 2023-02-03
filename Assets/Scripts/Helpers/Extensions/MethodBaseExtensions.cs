using System.Reflection;

public static class MethodBaseExtensions
{
    public static bool IsGetter(this MethodBase method)
    {
        bool isSpecialMethod = method.IsSpecialName;
        return isSpecialMethod && method.Name.StartsWith("get_");
    }

    public static bool IsSetter(this MethodBase method)
    {
        bool isSpecialMethod = method.IsSpecialName;
        return isSpecialMethod && method.Name.StartsWith("set_");
    }
    public static string GetInCodeMethodName(this MethodBase method)
    {
        string methodName = method.Name;

        if (IsGetter(method) || IsSetter(method))
        {
            methodName = methodName.Substring(4);
        }
        return methodName;
    }

}