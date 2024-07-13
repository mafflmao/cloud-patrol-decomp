using System;

namespace Prime31
{
	public static class MiniJSON
	{
		[Obsolete("The MinJSON class has been moved to Prime31.Json")]
		public static object jsonDecode(string json)
		{
			return Json.jsonDecode(json);
		}

		[Obsolete("The MinJSON class has been moved to Prime31.Json")]
		public static string jsonEncode(object obj)
		{
			return Json.jsonEncode(obj);
		}
	}
}
