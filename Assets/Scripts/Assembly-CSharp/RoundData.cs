using System;
using System.Collections.Generic;

[Serializable]
public class RoundData
{
	public int easy;

	public int medium;

	public int hard;

	public int bonus;

	public int boss;

	public int superEasy;

	public int Count
	{
		get
		{
			return easy + medium + hard + bonus + superEasy + boss;
		}
	}

	public List<Difficulty> BuildListOfDifficulties()
	{
		List<Difficulty> list = new List<Difficulty>();
		for (int i = 0; i < easy; i++)
		{
			list.Add(Difficulty.Easy);
		}
		for (int j = 0; j < medium; j++)
		{
			list.Add(Difficulty.Medium);
		}
		for (int k = 0; k < hard; k++)
		{
			list.Add(Difficulty.Hard);
		}
		for (int l = 0; l < bonus; l++)
		{
			list.Add(Difficulty.Bonus);
		}
		for (int m = 0; m < superEasy; m++)
		{
			list.Add(Difficulty.SuperEasy);
		}
		for (int n = 0; n < boss; n++)
		{
			list.Add(Difficulty.Boss);
		}
		return list;
	}
}
