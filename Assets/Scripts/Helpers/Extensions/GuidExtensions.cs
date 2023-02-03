using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GuidExtensions
{
    public static void SetNewIfNotSet(this ref Guid guid)
    {
        if (guid == default)
        {
            guid = Guid.NewGuid();
        }
    }

    public static void SetNewIfNotSet(this ref SerializableGuid guid)
    {
        if (guid.Value == Guid.Empty)
        {
            guid = Guid.NewGuid();
        }
    }

    public static Guid GetGuidWithByteAdd(this Guid guid, int add)
    {
        if(add == 0)
        {
            return guid;
        }
        var byteArray = guid.ToByteArray();
        if(add < 0)
        {
            throw new Exception("Index cannot be negative");
        }
        for (int i = 0; i < 16; i++)
        {
            int intValue = (int)byteArray[i];
            if (intValue + add <= 255)
            {
                byteArray[i] = (byte)(intValue + add);
                return new Guid(byteArray);
            }
            else
            {
                var addAmount = 255 - intValue;
                byteArray[i] = (byte)(intValue + addAmount);
                add -= addAmount;
            }
        }
        for (int i = 0; i < 16; i++)
        {
            int intValue = (int)byteArray[i];
            if (intValue - add > 0)
            {
                byteArray[i] = (byte)(intValue - add);
                return new Guid(byteArray);
            }
            else
            {
                var removeAmount = intValue - 1;
                byteArray[i] = (byte)(intValue - removeAmount);
                add -= removeAmount;
            }
        }
        Debug.Log($"Cannot distribute {add} amount in guid {guid}");
        return new Guid(byteArray);
    }
}
