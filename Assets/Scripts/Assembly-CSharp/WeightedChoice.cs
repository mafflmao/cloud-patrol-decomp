using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeightedChoice
{
	public class Data<T>
	{
		public T Value { get; private set; }

		public int Weight { get; private set; }

		public Data(T value, int weight)
		{
			Value = value;
			Weight = weight;
		}
	}

	public static T Choose<T>(params Data<T>[] sourceWeights)
	{
		if (!sourceWeights.Any())
		{
			return default(T);
		}
		List<int> list = new List<int>();
		for (int i = 0; i < sourceWeights.Length; i++)
		{
			Data<T> data = sourceWeights[i];
			for (int j = 0; j < data.Weight; j++)
			{
				list.Add(i);
			}
		}
		int index = Random.Range(0, list.Count);
		return sourceWeights[list[index]].Value;
	}
}
