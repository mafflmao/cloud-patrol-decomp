using System.Reflection;

public class CloneUtility
{
	public static void CopyPublicFields<T>(T source, T destination)
	{
		FieldInfo[] fields = typeof(T).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			object value = fieldInfo.GetValue(source);
			fieldInfo.SetValue(destination, value);
		}
	}
}
