using System;
using UnityEngine;

public static class EnumUtils
{
	public static T[] GetValues<T>() where T : struct, IConvertible, IFormattable
	{
		Type typeFromHandle = typeof(T);
		if (!typeFromHandle.IsEnum)
		{
			throw new InvalidOperationException("Generic type must be an enumeration");
		}
		return (T[])Enum.GetValues(typeFromHandle);
	}

	public static T GetRandomValue<T>() where T : struct, IConvertible, IFormattable
	{
		Type typeFromHandle = typeof(T);
		if (!typeFromHandle.IsEnum)
		{
			throw new InvalidOperationException("Generic type must be an enumeration");
		}
		T[] values = GetValues<T>();
		return values[UnityEngine.Random.Range(0, values.Length)];
	}

	public static T ToEnum<T>(string encodedEnum, T defaultValue) where T : struct, IConvertible, IFormattable
	{
		if (string.IsNullOrEmpty(encodedEnum) || !Enum.IsDefined(typeof(T), encodedEnum))
		{
			return defaultValue;
		}
		return (T)Enum.Parse(typeof(T), encodedEnum);
	}

	public static bool TryParse<T>(string encodedEnum, out T value) where T : struct, IConvertible, IFormattable
	{
		if (string.IsNullOrEmpty(encodedEnum) || !Enum.IsDefined(typeof(T), encodedEnum))
		{
			value = default(T);
			return false;
		}
		value = (T)Enum.Parse(typeof(T), encodedEnum);
		return true;
	}
}
