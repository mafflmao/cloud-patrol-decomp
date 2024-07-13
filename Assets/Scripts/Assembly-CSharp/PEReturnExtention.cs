using System.Collections;

public static class PEReturnExtention
{
	public static int PEToInt(this object obj)
	{
		if (obj is int)
		{
			return (int)obj;
		}
		if (obj is string)
		{
			int result = int.MinValue;
			try
			{
				result = int.Parse((string)obj);
			}
			catch
			{
			}
			return result;
		}
		if (obj is double)
		{
			return (int)(double)obj;
		}
		if (obj is uint)
		{
			return (int)(uint)obj;
		}
		if (obj is long)
		{
			return (int)(long)obj;
		}
		if (obj is ulong)
		{
			return (int)(ulong)obj;
		}
		return int.MinValue;
	}

	public static string PEToString(this object obj)
	{
		if (obj is string)
		{
			return (string)obj;
		}
		if (obj is double)
		{
			return ((double)obj).ToString();
		}
		return null;
	}

	public static string[] PEToStringArray(this object obj)
	{
		if (obj is string)
		{
			string text = (string)obj;
			int num = 1;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == ',')
				{
					num++;
				}
			}
			string[] array = new string[num];
			int num2 = 0;
			for (int j = 0; j < num; j++)
			{
				num2 = ((text.IndexOf(",") != -1) ? text.IndexOf(",") : text.Length);
				array[j] = text.Substring(0, num2);
				if (text.IndexOf(",") != -1)
				{
					text = text.Remove(0, num2 + 1);
				}
			}
			return array;
		}
		if (obj is ArrayList)
		{
			ArrayList arrayList = (ArrayList)obj;
			string[] array2 = null;
			int count = arrayList.Count;
			array2 = new string[count];
			for (int k = 0; k < count; k++)
			{
				array2[k] = arrayList[k].ToString();
			}
			return array2;
		}
		return null;
	}

	public static float PEToFloat(this object obj)
	{
		if (obj is float)
		{
			return (float)obj;
		}
		if (obj is double)
		{
			return (float)(double)obj;
		}
		if (obj is string)
		{
			float result = 0f;
			try
			{
				result = float.Parse((string)obj);
			}
			catch
			{
			}
			return result;
		}
		if (obj is uint)
		{
			return (uint)obj;
		}
		if (obj is uint)
		{
			return (uint)obj;
		}
		return float.NaN;
	}

	public static double PEToDouble(this object obj)
	{
		if (obj is float)
		{
			return (float)obj;
		}
		if (obj is double)
		{
			return (double)obj;
		}
		if (obj is string)
		{
			double result = 0.0;
			try
			{
				result = double.Parse((string)obj);
			}
			catch
			{
			}
			return result;
		}
		return double.NaN;
	}

	public static uint PEToUint(this object obj)
	{
		if (obj is uint)
		{
			return (uint)obj;
		}
		if (obj is double)
		{
			return (uint)(double)obj;
		}
		if (obj is string)
		{
			uint result = 0u;
			try
			{
				result = uint.Parse((string)obj);
			}
			catch
			{
			}
			return result;
		}
		return 0u;
	}

	public static long PEToLong(this object obj)
	{
		if (obj is long)
		{
			return (long)obj;
		}
		if (obj is double)
		{
			return (long)(double)obj;
		}
		if (obj is string)
		{
			long result = 0L;
			try
			{
				result = long.Parse((string)obj);
			}
			catch
			{
			}
			return result;
		}
		return 0L;
	}

	public static ulong PEToULong(this object obj)
	{
		if (obj is ulong)
		{
			return (ulong)obj;
		}
		if (obj is double)
		{
			return (ulong)(double)obj;
		}
		if (obj is string)
		{
			ulong result = 0uL;
			try
			{
				result = ulong.Parse((string)obj);
			}
			catch
			{
			}
			return result;
		}
		return 0uL;
	}

	public static bool PEToBool(this object obj)
	{
		if (obj is bool)
		{
			return (bool)obj;
		}
		if (obj is double)
		{
			return (double)obj != 0.0;
		}
		if (obj is string)
		{
			bool result = false;
			try
			{
				result = bool.Parse((string)obj);
			}
			catch
			{
			}
			return result;
		}
		return false;
	}
}
