using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Prime31
{
	public static class DeserializationExtensions
	{

		public static Dictionary<string, object> toDictionary(this object self)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			FieldInfo[] fields = self.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(P31DeserializeableFieldAttribute), true);
				foreach (object obj in customAttributes)
				{
					P31DeserializeableFieldAttribute p31DeserializeableFieldAttribute = obj as P31DeserializeableFieldAttribute;
					if (p31DeserializeableFieldAttribute.isCollection)
					{
						IEnumerable enumerable = fieldInfo.GetValue(self) as IEnumerable;
						ArrayList arrayList = new ArrayList();
						foreach (object item in enumerable)
						{
							arrayList.Add(item.toDictionary());
						}
						dictionary[p31DeserializeableFieldAttribute.key] = arrayList;
					}
					else if (p31DeserializeableFieldAttribute.type != null)
					{
						dictionary[p31DeserializeableFieldAttribute.key] = fieldInfo.GetValue(self).toDictionary();
					}
					else
					{
						dictionary[p31DeserializeableFieldAttribute.key] = fieldInfo.GetValue(self);
					}
				}
			}
			return dictionary;
		}

		[Obsolete("Use the toDictionary method to get a proper generic Dictionary returned. Hashtables are obsolute.")]
		public static Hashtable toHashtable(this object self)
		{
			Hashtable hashtable = new Hashtable();
			FieldInfo[] fields = self.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(P31DeserializeableFieldAttribute), true);
				foreach (object obj in customAttributes)
				{
					P31DeserializeableFieldAttribute p31DeserializeableFieldAttribute = obj as P31DeserializeableFieldAttribute;
					if (p31DeserializeableFieldAttribute.isCollection)
					{
						IEnumerable enumerable = fieldInfo.GetValue(self) as IEnumerable;
						ArrayList arrayList = new ArrayList();
						foreach (object item in enumerable)
						{
							arrayList.Add(item.toHashtable());
						}
						hashtable[p31DeserializeableFieldAttribute.key] = arrayList;
					}
					else if (p31DeserializeableFieldAttribute.type != null)
					{
						hashtable[p31DeserializeableFieldAttribute.key] = fieldInfo.GetValue(self).toHashtable();
					}
					else
					{
						hashtable[p31DeserializeableFieldAttribute.key] = fieldInfo.GetValue(self);
					}
				}
			}
			return hashtable;
		}
	}
}
