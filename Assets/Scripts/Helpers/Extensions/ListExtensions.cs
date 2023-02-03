using System;
using System.Collections.Generic;
using System.Linq;
public static class ListExtensions
{
    /// <summary>
    /// Pops element by <paramref name="index"/>.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="list">List with elements.</param>
    /// <param name="index">Index of element to pop.</param>
    /// <returns>The popped element.</returns>
    public static T Pop<T>(this IList<T> list, int index)
    {
        var element = list[index];
        list.RemoveAt(index);

        return element;
    }

    /// <summary>
    /// Pops elements by <paramref name="indexes"/>.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="list">List with elements.</param>
    /// <param name="indexes">Indexes of elements to be popped.</param>
    /// <returns>The popped element.</returns>
    public static List<T> Pop<T>(this IList<T> list, params int[] indexes)
    {
        var popped = new List<T>();

        foreach (var index in indexes)
            popped.Add(list.Pop(index));

        return popped;
    }

    /// <summary>
    /// Pops random element from <paramref name="list"/>.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="list">List with elements.</param>
    /// <returns>The popped element.</returns>
    public static T PopRandom<T>(this IList<T> list) => list.Pop(UnityEngine.Random.Range(0, list.Count));

    /// <summary>
    /// Pops random elements from list.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="list">List with elements.</param>
    /// <param name="count">Count of elements to be popped.</param>
    /// <returns></returns>
    public static List<T> PopRandoms<T>(this IList<T> list, int count)
    {
        var popped = new List<T>();

        for (int i = 0; i < count; i++)
            popped.Add(list.PopRandom());

        return popped;
    }

    public static List<T> ToList<T>(this T[] arr)
    {
        List<T> list = new List<T>();
        list.AddRange(arr);
        return list;
    }

    public static int IndexOf<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public static int IndexOf<T>(this IReadOnlyList<T> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(value))
            {
                return i;
            }
        }
        return -1;
    }



    public static void Swap<T>(this T[] array, int item1Index, int item2Index)
    {
        if (item1Index == item2Index)
        {
            return;
        }
        var item1Value = array[item1Index];
        array[item1Index] = array[item2Index];
        array[item2Index] = item1Value;
    }

    public static bool IsAllElementsZero(this IReadOnlyList<byte> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            if (list[i] != 0)
            {
                return false;
            }
        }
        return true;
    }

    public static int RemoveAllNulls<T>(this IList<T> list)
    {
        int count = list.Count;
        int removed = 0;
        for (int i = count - 1; i >= 0; i--)
        {
            if (list[i].IsNull())
            {
                list.RemoveAt(i);
                removed++;
            }
        }
        return removed;
    }

    public static void RemoveDuplicates<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T element = list[i];

            // Remove all subsequent occurrences of the element
            while (list.LastIndexOf(element) > i)
            {
                list.RemoveAt(list.LastIndexOf(element));
            }
        }
    }
}