using UnityEngine;

public static class ResourceUtils
{
	public static T LoadResource<T>(string resourcePath) where T : Object
	{
		T val = (T)Resources.Load(resourcePath, typeof(T));
		if (val == null)
		{
			Debug.LogError(string.Format("Unable to load resource '{0}'.", resourcePath));
		}
		return val;
	}
}
