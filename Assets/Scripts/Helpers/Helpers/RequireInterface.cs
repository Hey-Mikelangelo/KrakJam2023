using UnityEngine;

public class RequireInterface : PropertyAttribute
{
    public System.Type requiredType { get; private set; }

    public RequireInterface(System.Type type)
    {
        this.requiredType = type;
    }
}