using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
//Without this attribute this class will be serialized as List (because of IReadOnlyList<T>) and this will cause errors in deserialization
[JsonObject]
public class CompoundCollection<T> : IReadOnlyList<T>
{
    [JsonProperty] [SerializeField, ReadOnly] private List<IReadOnlyList<T>> listOfLists = new List<IReadOnlyList<T>>();
    [JsonIgnore] public int ListsCount => listOfLists.Count;
    [JsonIgnore] public int Count => GetCount();
    public T this[int index] => GetItemAtIndex(index);

    public CompoundCollection(params IReadOnlyList<T>[] arrayOfLists)
    {
        Init(arrayOfLists);
    }
    public CompoundCollection(IReadOnlyList<IReadOnlyList<T>> listOfLists)
    {
        Init(listOfLists);
    }

    public CompoundCollection()
    {
    }

    private void Init(IReadOnlyList<IReadOnlyList<T>> listOfLists)
    {
        if (listOfLists == null)
        {
            throw new NullReferenceException();
        }
        int count = listOfLists.Count;
        for (int i = 0; i < count; i++)
        {
            IReadOnlyList<T> list = listOfLists[i];
            Add(list);
        }
    }
    public List<T> ToFlatList()
    {
        List<T> list = new List<T>();
        int count = listOfLists.Count;
        for (int i = 0; i < count; i++)
        {
            list.AddRange(listOfLists[i]);
        }
        return list;
    }

    public IReadOnlyList<T> GetListAtIndex(int index)
    {
        return listOfLists[index];
    }
    public int IndexOf(T item) 
    {
        //TODO write test
        int previousIndexesCount = 0;
        int listOfCount = listOfLists.Count;
        for (int i = 0; i < listOfCount; i++)
        {
            IReadOnlyList<T> list = listOfLists[i];
            int indexIfItem = list.IndexOf(item);
            if (indexIfItem != 1)
            {
                return previousIndexesCount + indexIfItem;
            }
            previousIndexesCount += list.Count;
        }
        return -1;
    }

    public void Insert(int index, IReadOnlyList<T> item)
    {
        listOfLists.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        listOfLists.RemoveAt(index);
    }

    public void Add(IReadOnlyList<T> item)
    {
        listOfLists.Add(item);
    }

    public void Clear()
    {
        listOfLists.Clear();
    }

    public bool Contains(IReadOnlyList<T> item)
    {
        return listOfLists.Contains(item);
    }

    public void CopyTo(IReadOnlyList<T>[] array, int arrayIndex)
    {
        listOfLists.CopyTo(array, arrayIndex);
    }

    public bool Remove(IReadOnlyList<T> item)
    {
        bool removed = listOfLists.Remove(item);
        return removed;
    }

    private int GetCount()
    {
        int count = 0;
        foreach (var list in listOfLists)
        {
            count += list.Count;
        }
        return count;
    }
    private T GetItemAtIndex(int index)
    {
        //TODO write test

        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException();
        }
        int previousIndexesCount = 0;
        int listsCount = listOfLists.Count;
        for (int i = 0; i < listsCount; i++)
        {
            var list = listOfLists[i];
            int listItemsCount = list.Count;
            int localListItemIndex = index - previousIndexesCount;
            if (localListItemIndex < listItemsCount)
            {
                return list[localListItemIndex];
            }
            previousIndexesCount += listItemsCount;
        }
        throw new IndexOutOfRangeException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        int count = Count;
        for (int i = 0; i < count; i++)
        {
            yield return GetItemAtIndex(i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
