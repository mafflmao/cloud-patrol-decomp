using System;
using System.Collections;
using System.Collections.Generic;

namespace Prime31
{
	public static class JsonExtensions
	{
		public static string toJson(this IList obj)
		{
			return Json.jsonEncode(obj);
		}

		public static string toJson(this IDictionary obj)
		{
			return Json.jsonEncode(obj);
		}

		public static List<object> listFromJson(this string json)
		{
			return Json.jsonDecode(json, true) as List<object>;
		}

		public static Dictionary<string, object> dictionaryFromJson(this string json)
		{
			return Json.jsonDecode(json, true) as Dictionary<string, object>;
		}

		[Obsolete("Switch to the generic listFromJson method")]
		public static ArrayList arrayListFromJson(this string json)
		{
			return Json.jsonDecode(json) as ArrayList;
		}

		[Obsolete("Switch to the generic dictionaryFromJson method")]
		public static Hashtable hashtableFromJson(this string json)
		{
			return Json.jsonDecode(json) as Hashtable;
		}
	}
}
