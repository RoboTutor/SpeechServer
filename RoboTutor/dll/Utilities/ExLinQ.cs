using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Example:
/// string[] s = {"zero", "one", "two", "three", "four", "five"};
/// var x = s.IndexesWhere(t => t.StartsWith("t"));
/// </summary>
public static class ExLinQ
{
    public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        int index = 0;
        foreach (T element in source)
        {
            if (predicate(element))
            {
                yield return index;
            }
            index++;
        }
    }
}

