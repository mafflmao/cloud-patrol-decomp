using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CsvUtilities
{
	internal static List<List<string>> LoadCSVLines(Stream csvDataStream)
	{
		List<List<string>> list = new List<List<string>>();
		using (TextReader textReader = new StreamReader(csvDataStream))
		{
			List<string> list2 = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			char[] array = new char[32768];
			int num = textReader.ReadBlock(array, 0, array.Length);
			while (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					char c = array[i];
					bool flag2 = true;
					bool flag3 = false;
					bool flag4 = false;
					if (!flag)
					{
						switch (c)
						{
						case '\r':
							flag2 = false;
							break;
						case '\n':
							flag3 = true;
							flag4 = true;
							flag2 = false;
							break;
						case ',':
							flag2 = false;
							flag3 = true;
							break;
						}
					}
					if (c == '"')
					{
						if (i < num - 1 && array[i + 1] == '"')
						{
							i++;
						}
						else
						{
							flag2 = false;
							flag = !flag;
						}
					}
					if (flag2)
					{
						if (c == '…')
						{
							stringBuilder.Append("...");
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
					if (flag3)
					{
						list2.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
					}
					if (flag4 && list2.Count > 0)
					{
						list.Add(list2);
						list2 = new List<string>();
					}
				}
				num = textReader.Read(array, 0, array.Length);
				if (num == 0 && stringBuilder.Length > 0)
				{
					list2.Add(stringBuilder.ToString());
					stringBuilder.Length = 0;
				}
			}
			if (list2.Count > 0)
			{
				list.Add(list2);
			}
		}
		return list;
	}

	public static string GetValueFromListOrNull(List<string> values, int index)
	{
		if (index >= values.Count)
		{
			return null;
		}
		return values[index];
	}

	public static int? GetIntValueFromListOrNull(List<string> values, int index)
	{
		string valueFromListOrNull = GetValueFromListOrNull(values, index);
		if (valueFromListOrNull != null)
		{
			int result;
			if (int.TryParse(valueFromListOrNull, out result))
			{
				return result;
			}
			if (valueFromListOrNull != string.Empty)
			{
				Debug.LogError("Unable to parse '" + valueFromListOrNull + "' into integer from column '" + index + "' in data: '" + string.Join(", ", values.ToArray()) + "'");
			}
		}
		return null;
	}

	public static bool? GetBoolValueFromListOrNull(List<string> values, int index)
	{
		string valueFromListOrNull = GetValueFromListOrNull(values, index);
		if (valueFromListOrNull != null)
		{
			bool result;
			if (bool.TryParse(valueFromListOrNull, out result))
			{
				return result;
			}
			if (valueFromListOrNull != string.Empty)
			{
				Debug.LogError("Unable to parse '" + valueFromListOrNull + "' into bool from column '" + index + "' in data: '" + string.Join(", ", values.ToArray()) + "'");
			}
		}
		return null;
	}

	public static int GetIntValueFromListOrDefault(List<string> values, int index, int defaultValue)
	{
		string valueFromListOrNull = GetValueFromListOrNull(values, index);
		int result;
		if (!int.TryParse(valueFromListOrNull, out result))
		{
			return defaultValue;
		}
		return result;
	}

	public static bool IsValueEmptyOrMissing(List<string> values, int index)
	{
		string valueFromListOrNull = GetValueFromListOrNull(values, index);
		return string.IsNullOrEmpty(valueFromListOrNull);
	}

	public static List<List<string>> LoadCsvDataFromResource(string resourcePath)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(resourcePath, typeof(TextAsset));
		if (textAsset == null)
		{
			Debug.LogError("Unable to find CSV file for resource at '" + resourcePath + "'");
		}
		return LoadCsvDataFromAsset(textAsset);
	}

	public static List<List<string>> LoadCsvDataFromAsset(TextAsset textAsset)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (StreamWriter streamWriter = new StreamWriter(memoryStream))
			{
				streamWriter.Write(textAsset.text);
				streamWriter.Flush();
				memoryStream.Position = 0L;
				return LoadCSVLines(memoryStream);
			}
		}
	}

	public static List<List<string>> LoadCsvDataFromFile(string fileName)
	{
		using (FileStream csvDataStream = new FileStream(fileName, FileMode.Open))
		{
			return LoadCSVLines(csvDataStream);
		}
	}
}
