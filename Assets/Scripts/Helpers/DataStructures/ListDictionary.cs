using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class ListDictionary<TKey, TValue> : IDictionary<TKey, TValue> , IReadOnlyDictionary<TKey, TValue>, ISerializationCallbackReceiver, IEnumerable<TValue>
{
    private List<KeyValuePair<TKey, TValue>> keyValuePairs;
    [SerializeField] private TKey[] keysSerialized;
    [SerializeField] private TValue[] valuesSerialized;

    public IReadOnlyList<KeyValuePair<TKey, TValue>> KeyValuePairs => keyValuePairs;
    public TValue this[TKey key] { get => GetValue(key); set => SetValue(key, value); }

    public ICollection<TKey> Keys => keyValuePairs.Select(x => x.Key).ToArray();

    public ICollection<TValue> Values => keyValuePairs.Select(x => x.Value).ToArray();

    public int Capacity => keyValuePairs.Capacity;
    public int Count => keyValuePairs.Count;

    public bool IsReadOnly => false;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this.Values;
    public ListDictionary()
    {
        keyValuePairs = new();
    }
    public ListDictionary(int count)
    {
        keyValuePairs = new(count);
    }
    private TValue GetValue(TKey key)
    {
        if(TryGetValue(key, out var value))
        {
            return value;
        }
        throw new Exception($"Dictionary does not contain a key {key}");
    }

    private void SetValue(TKey key, TValue value)
    {
        int index = GetIndexOfKey(key);
        if(index == -1)
        {
            throw new Exception($"Dictionary does not contain a key {key}");
        }
        keyValuePairs[index] = new KeyValuePair<TKey, TValue>(key, value);
    }

    public void EnsureCapacity(int capacity)
    {
        keyValuePairs.Capacity = capacity;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        if (GetIndexOfKey(item.Key) != -1)
        {
            throw new System.Exception($"Dictionary already contains key {item.Key}");
        }
        keyValuePairs.Add(item);
    }

    public bool AddOverride(TKey key, TValue value)
    {
        bool overriden = false;
        if (ContainsKey(key))
        {
            Remove(key);
            overriden = true;
        }
        Add(key, value);
        return overriden;
    }
    public void Add(TKey key, TValue value)
    {
        Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public void Clear()
    {
        keyValuePairs.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int count = keyValuePairs.Count;
        for (int i = 0; i < count; i++)
        {
            if(keyValuePairs[i].Key.Equals(item.Key) && keyValuePairs[i].Value.Equals(item.Value))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return GetIndexOfKey(key) != -1;
    }

    public bool ContainsValue(TValue value)
    {
        return GetIndexOfValue(value) != -1;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        keyValuePairs.CopyTo(array, arrayIndex);
    }

    public IEnumerator<TValue> GetValuesEnumerator()
    {
        int count = keyValuePairs.Count;
        for (int i = 0; i < count; i++)
        {
            yield return keyValuePairs[i].Value;
        }
    }
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        int count = keyValuePairs.Count;
        for (int i = 0; i < count; i++)
        {
            yield return keyValuePairs[i];
        }
    }

    public void SetValueAtIndex(int index, TValue value)
    {
        keyValuePairs[index] = new KeyValuePair<TKey, TValue>(keyValuePairs[index].Key, value);
    }
    public TValue GetValueAtIndex(int index)
    {
        return keyValuePairs[index].Value;
    }
    public TKey GetKeyAtIndex(int index)
    {
        return keyValuePairs[index].Key;
    }
    public KeyValuePair<TKey, TValue> GetKeyValuePairAtIndex(int index)
    {
        return keyValuePairs[index];
    }

    public List<KeyValuePair<TKey, TValue>> ToKeyValuePairList()
    {
        int count = Count;
        List<KeyValuePair<TKey, TValue>> newList = new List<KeyValuePair<TKey, TValue>>(count);
        for (int i = 0; i < count; i++)
        {
            newList.Add(GetKeyValuePairAtIndex(i));
        }
        return newList;
    }

    public bool Remove(TKey key)
    {
        int index = GetIndexOfKey(key);
        if(index == -1)
        {
            return false;
        }
        keyValuePairs.RemoveAt(index);
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int itemToRemoveIndex = keyValuePairs.IndexOf(item);
        if(itemToRemoveIndex != -1)
        {
            keyValuePairs.RemoveAt(itemToRemoveIndex);
            return true;
        }
        return false;
    }
    public bool TryGetValueAndRemoveEntry(TKey key, out TValue value)
    {
        int index = GetIndexOfKey(key);
        if (index == -1)
        {
            value = default;
            return false;
        }
        value = keyValuePairs[index].Value;
        keyValuePairs.RemoveAt(index);
        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = GetIndexOfKey(key);
        if(index == -1)
        {
            value = default;
            return false;
        }
        value = keyValuePairs[index].Value;
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        int count = keyValuePairs.Count;
        for (int i = 0; i < count; i++)
        {
            yield return keyValuePairs[i];
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndexOfKey(TKey searchedKey)
    {
        int count = keyValuePairs.Count;
        int searchedKeyHashCode = searchedKey.GetHashCode();
        for (int i = 0; i < count; i++)
        {
            if (typeof(System.IEquatable<TKey>).IsAssignableFrom(typeof(TKey)))
            {
                IEquatable<TKey> key = keyValuePairs[i].Key as IEquatable<TKey>;
                if (key.Equals(searchedKey))
                {
                    return i;
                }
            }
            else if (typeof(TKey).IsValueType)
            {
                if (keyValuePairs[i].Key.GetHashCode() == searchedKeyHashCode)
                {
                    return i;
                }
            }
            else
            {
                if (keyValuePairs[i].Key.Equals(searchedKey))
                {
                    return i;
                }
            }
           
        }
        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndexOfValue(TValue searchedValue)
    {
        int count = keyValuePairs.Count;
        int searchedValueHashCode = searchedValue.GetHashCode();
        for (int i = 0; i < count; i++)
        {
            if (typeof(System.IEquatable<TValue>).IsAssignableFrom(typeof(TValue)))
            {
                IEquatable<TValue> value = keyValuePairs[i].Value as IEquatable<TValue>;
                if (value.Equals(searchedValue))
                {
                    return i;
                }
            }
            else
            {
                if (keyValuePairs[i].Value.GetHashCode() == searchedValueHashCode)
                {
                    return i;
                }
            }
            
        }
        return -1;
    }

    public void OnBeforeSerialize()
    {
        if (keyValuePairs == null || keyValuePairs.Count == 0)
        {
            return;
        }
        int count = keyValuePairs.Count;
        if(keysSerialized == null || keysSerialized.Length != count)
        {
            keysSerialized = new TKey[count];
        }
        if (valuesSerialized == null || valuesSerialized.Length != count)
        {
            valuesSerialized = new TValue[count];
        }
        for (int i = 0; i < count; i++)
        {
            keysSerialized[i] = keyValuePairs[i].Key;
            valuesSerialized[i] = keyValuePairs[i].Value;
        }
    }

    public void OnAfterDeserialize()
    {
        if(keysSerialized == null || valuesSerialized == null)
        {
            return;
        }
        if(keysSerialized.Length != valuesSerialized.Length)
        {
            Debug.LogError("keys count != values count");
            return;
        }
        int count = keysSerialized.Length;
        keyValuePairs.Clear();
        for (int i = 0; i < count; i++)
        {
            keyValuePairs.Add(new KeyValuePair<TKey, TValue>(keysSerialized[i], valuesSerialized[i]));
        }
    }

    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
        return GetValuesEnumerator();
    }
}
