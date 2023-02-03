using UnityEngine;

public abstract class SingletonComponentData<TypeT, DataT> : MonoComponentData<TypeT, DataT> where TypeT : SingletonInputComponentData<TypeT, DataT>
{
    public static TypeT Instance { get; private set; }

    public override void OnInit()
    {
        base.OnInit();
        if (Instance != null)
        {
            Debug.LogError($"More than one instance of {typeof(TypeT).Name}");
            return;
        }
        Instance = this as TypeT;
    }
}
