using UnityEngine;

public abstract class MonoComponentData<TypeT, DataT> : MonoComponentData where TypeT : MonoComponentData<TypeT, DataT> 
{
    public ComponentData GetComponentData()
    {
        return new ComponentData()
        {
            Data = GetData(),
            ComponentDataMonoBehaviour = (TypeT)this
        };
    }
    public abstract DataT GetData();

    [System.Serializable]
    public struct ComponentData
    {
        public DataT Data;
        public TypeT ComponentDataMonoBehaviour;
    }
}

public abstract class MonoComponentData : LoadableMonoBehaviour
{

}

