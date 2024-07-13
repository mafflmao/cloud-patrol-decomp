using Boo.Lang.Resources;

namespace Boo.Lang
{
	public static class ResourceManager
	{
		public static string Format(string name, params object[] args)
		{
			return string.Format(GetString(name), args);
		}

		private static string GetString(string name)
		{
			return (string)typeof(StringResources).GetField(name).GetValue(null);
		}
	}
}
