using System;
using UnityEngine;

/// <summary>
/// Serializable wrapper for System.Guid.
/// Can be implicitly converted to/from System.Guid.
///
/// Author: Katie Kennedy (Searous)
/// </summary>
/// 
[Serializable]
public struct SerializableGuid : ISerializationCallbackReceiver
{
    [SerializeField] private byte[] guidByteArray;

    public byte[] GuidByteArray
    {
        get
        {
            if (guidByteArray == null || guidByteArray.Length != 16)
            {
                guidByteArray = new byte[16];
            }
            return guidByteArray;
        }
    }
    public Guid Value => (guidByteArray == null || guidByteArray.Length != 16) ? Guid.Empty : new Guid(guidByteArray);
    public SerializableGuid(Guid guid)
    {
        guidByteArray = guid.ToByteArray();
    }

    public SerializableGuid(byte[] byteArray)
    {
        guidByteArray = new byte[16];
        Array.Copy(byteArray, guidByteArray, 16);
    }

    public override bool Equals(object obj)
    {
        return obj is SerializableGuid guid &&
            this.GuidByteArray.ItemsSequenceEqual(guid.guidByteArray);
    }

    public override int GetHashCode()
    {
        return -1324198676 + new Guid(GuidByteArray).GetHashCode();
    }

    public override string ToString() => new Guid(GuidByteArray).ToString();

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if(guidByteArray == null || guidByteArray.Length != 16)
        {
            guidByteArray = new byte[16];
        }
    }

    public static bool operator ==(SerializableGuid a, SerializableGuid b) => a.Value == b.Value;
    public static bool operator !=(SerializableGuid a, SerializableGuid b) => a.Value != b.Value;
    public static implicit operator SerializableGuid(Guid guid) => new SerializableGuid(guid);
    public static implicit operator Guid(SerializableGuid serializable) => serializable.Value;
    public static implicit operator SerializableGuid(string serializedGuid)
    {
        if (string.IsNullOrEmpty(serializedGuid))
        {
            return new SerializableGuid(Guid.Empty);
        }
        return new SerializableGuid(Guid.Parse(serializedGuid));
    }
    public static implicit operator string(SerializableGuid serializedGuid) => serializedGuid.ToString();
}