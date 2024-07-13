using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Boo.Lang.Runtime
{
	public class ExtensionRegistry
	{
		private List<MemberInfo> _extensions = new List<MemberInfo>();

		private object _classLock = new object();

		public IEnumerable<MemberInfo> Extensions
		{
			get
			{
				return _extensions;
			}
		}

		public void Register(Type type)
		{
			lock (_classLock)
			{
				_extensions = AddExtensionMembers(CopyExtensions(), type);
			}
		}

		public void UnRegister(Type type)
		{
			lock (_classLock)
			{
				List<MemberInfo> list = CopyExtensions();
				list.RemoveAll((MemberInfo member) => member.DeclaringType == type);
				_extensions = list;
			}
		}

		private static List<MemberInfo> AddExtensionMembers(List<MemberInfo> extensions, Type type)
		{
			MemberInfo[] members = type.GetMembers(BindingFlags.Static | BindingFlags.Public);
			foreach (MemberInfo memberInfo in members)
			{
				if (Attribute.IsDefined(memberInfo, typeof(ExtensionAttribute)) && !extensions.Contains(memberInfo))
				{
					extensions.Add(memberInfo);
				}
			}
			return extensions;
		}

		private List<MemberInfo> CopyExtensions()
		{
			return new List<MemberInfo>(_extensions);
		}
	}
}
