using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumerableExtensions
{
	public static T RandomOrDefault<T>(this IEnumerable<T> items)
	{
		if (!items.Any())
		{
			return default(T);
		}
		int index = Random.Range(0, items.Count());
		return items.ElementAt(index);
	}

	public static T ElementBefore<T>(this IEnumerable<T> sequence, T value)
	{
		T val = sequence.First();
		if (object.Equals(value, val))
		{
			return sequence.Last();
		}
		foreach (T item in sequence)
		{
			if (object.Equals(item, value))
			{
				return val;
			}
			val = item;
		}
		Debug.LogWarning("Couldn't find element in sequence.");
		return value;
	}

	public static T ElementAfter<T>(this IEnumerable<T> sequence, T value)
	{
		IEnumerator<T> enumerator = sequence.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (object.Equals(value, enumerator.Current))
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				return sequence.First();
			}
		}
		Debug.LogWarning("Couldn't find element in sequence");
		return value;
	}
}
