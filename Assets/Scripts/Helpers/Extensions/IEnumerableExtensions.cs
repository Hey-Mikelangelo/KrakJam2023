using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DictionaryExtensions
{
    public static bool TryGetValueFromDictWithErrorLog<TKey, TValue>(this SerializableDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
    {
        if (dictionary.TryGetValue(key, out TValue outValue))
        {
            value = outValue;
            return true;
        }
        else
        {
            Debug.LogError($"Dictionary does not contain {typeof(TValue).Name} value for {key} of type {typeof(TKey).Name}");
            value = default;
            return false;
        }
    }
    public static bool TryGetValueFromDictWithErrorLog<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value)
    {
        if (dictionary.TryGetValue(key, out TValue outValue))
        {
            value = outValue;
            return true;
        }
        else
        {
            Debug.LogError($"Dictionary does not contain {typeof(TValue).Name} value for {key} of type {typeof(TKey).Name}");
            value = default;
            return false;
        }
    }

    public static List<KeyValuePair<TKey, TValue>> ToKeyValuePairList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        return dictionary.ToList();
    }

    public static Dictionary<TKey, TValue> FromKeyValuePairList<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> keyValuePairs)
    {
        return keyValuePairs.ToDictionary(x => x.Key, x => x.Value);
    }
}

public static class IEnumerableExtensions
{
    /// <summary>
    /// Get random element from the <paramref name="enumerable"/>.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>Random element from enumerable.</returns>
    public static T GetRandomElement<T>(this IEnumerable<T> enumerable) => enumerable.ElementAt(Random.Range(0, enumerable.Count()));

    /// <summary>
    /// Get random elements from the <paramref name="enumerable"/>.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="count">Count of the random elements.</param>
    /// <returns>Random elements from enumerable.</returns>
    public static List<T> GetRandomElements<T>(this IEnumerable<T> enumerable, int count)
    {
        var poppedIndexes = Enumerable.Range(0, enumerable.Count()).ToList().PopRandoms(count);
        return enumerable.Where((el, i) => poppedIndexes.Contains(i)).ToList();
    }

    /// <summary>
    /// Excepts passed elements from <paramref name="enumerable"/>.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="elements">Elements to exclude.</param>
    /// <returns>Enumerable without passed elements.</returns>
    public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, params T[] elements) => enumerable.Except((IEnumerable<T>)elements);

    /// <summary>
    /// Shuffles <paramref name="enumerable"/>.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>Shuffled <paramref name="enumerable"/>.</returns>
    public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> enumerable) => enumerable.OrderBy(v => Random.value);

    /// <summary>
    /// Represents an enumerable as a string in the format <see langword="[a, b, c, ...]"/> 
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>String representation of the <paramref name="enumerable"/></returns>
    public static string AsString<T>(this IEnumerable<T> enumerable) => $"[{string.Join(", ", enumerable)}]";

    /// <returns>Returns new list with one item <paramref name="item"/></returns>
    public static List<T> ToSingleItemList<T>(this T item)
    {
        return new List<T>() { item };
    }

    /// <returns>Returns new array with one item <paramref name="item"/></returns>
    public static T[] ToSingleItemArray<T>(this T item)
    {
        return new T[] { item };
    }

    public static void AddDistinct<T>(this IList<T> list, IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            if (list.Contains(value) == false)
            {
                list.Add(value);
            }
        }
    }

    public static bool AddDistinct<T>(this IList<T> list, T value)
    {
        if(value == null)
        {
            return false;
        }
        if (list.Contains(value) == false)
        {
            list.Add(value);
            return true;
        }
        return false;
    }

    public static int RemoveAll<T>(this IList<T> list, IEnumerable<T> values)
    {
        if (values.IsNullWithErrorLog())
        {
            return 0;
        }
        if (values.Count() == 0)
        {
            return 0;
        }
        int removedCount = 0;
        foreach (var value in values)
        {
            bool removed = list.Remove(value);
            if (removed)
            {
                removedCount++;
            }
        }
        return removedCount;
    }

    public static bool ContainsAllElementsOf<T>(this IReadOnlyList<T> list, IReadOnlyList<T> other) where T : IEquatable<T>
    {
        if(list == other)
        {
            return true;
        }
        if(list.Count < other.Count)
        {
            return false;
        }
        for (int i = 0; i < other.Count; i++)
        {
            if(list.Contains(other[i]) == false)
            {
                return false;
            }
        }
        return true;
    }
    public static bool ContainsAnyElementOf<T>(this IReadOnlyList<T> list, IEnumerable<T> otherList)
    {
        if (list == otherList)
        {
            return true;
        }
        foreach (var otherItem in otherList)
        {
            if (list.Contains(otherItem))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsSameSequence<T>(this IReadOnlyList<T> list, IReadOnlyList<T> other) where T : IEquatable<T>
    {
        int count = list.Count;
        if(count != other.Count)
        {
            return false;
        }
        for (int i = 0; i < count; i++)
        {
            if(list[i].Equals(other[i]) == false)
            {
                return false;
            }
        }
        return true;
    }

    public static bool Contains<T>(this IReadOnlyList<T> list, T searchedItem)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            if (list[i].Equals(searchedItem))
            {
                return true;
            }
        }
        return false;
    }


    public static bool ContainsObj(this IEnumerable enumerable, object searchedItem)
    {
        foreach (var item in enumerable)
        {
            if (item.Equals(searchedItem))
            {
                return true;
            }
        }
        return false;
    }

    public static bool Contains<T>(this IEnumerable<T> enumerable, T searchedItem) where T : IEquatable<T>
    {
        foreach (var item in enumerable)
        {
            if (item.Equals(searchedItem))
            {
                return true;
            }
        }
        return false;
    }

    public static bool ItemsAreEqual<T>(this IEnumerable<T> enumerable, IEnumerable<T> otherEnumerable)
    {
        return otherEnumerable.All(enumerable.Contains<T>) && enumerable.Count() == otherEnumerable.Count();
    }

    public static bool ItemsSequenceEqual<T>(this IList<T> list1, IList<T> list2)
    {
        int count = list1.Count;
        int otherCount = list2.Count;
        if (count != otherCount)
        {
            return false;
        }
        for (int i = 0; i < count; i++)
        {
            var item1 = list1[i];
            var item2 = list2[i];

            if (item1.Equals(item2) == false)
            {
                return false;
            }
        }
        return true;
    }


}