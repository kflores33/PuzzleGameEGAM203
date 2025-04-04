using System.Collections.Generic;
using UnityEngine;

// https://discussions.unity.com/t/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code/535113/2
public static class IListExtensions
{
    public static void Shuffle<T>(this IList<T> ts) // Fisher-Yates shuffle algorithm
    {
        var count = ts.Count;
        var last = count - 1; // last element in the list
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count); // select a random element from the list
            var tmp = ts[i]; // store the current element in a temporary variable
            ts[i] = ts[r]; // get the random element (r) and set it to the current element (i)
            ts[r] = tmp; // move the temp element to the position of the random element (r) in the list (replaces the values, more accurately)
        }
    }
}
