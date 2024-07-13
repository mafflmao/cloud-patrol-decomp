using System;
using System.Collections.Generic;
using System.Reflection;
using Boo.Lang.Runtime.DynamicDispatching.Emitters;

namespace Boo.Lang.Runtime.DynamicDispatching
{
	internal class SliceDispatcherFactory : AbstractDispatcherFactory
	{
		public SliceDispatcherFactory(ExtensionRegistry extensions, object target, Type type, string name, params object[] arguments)
			: base(extensions, target, type, (name.Length != 0) ? name : RuntimeServices.GetDefaultMemberName(type), arguments)
		{
		}

		public Dispatcher CreateGetter()
		{
			MemberInfo[] array = ResolveMember();
			if (array.Length == 1)
			{
				return CreateGetter(array[0]);
			}
			return EmitMethodDispatcher(Getters(array));
		}

		private IEnumerable<MethodInfo> Getters(MemberInfo[] candidates)
		{
			foreach (MemberInfo info in candidates)
			{
				PropertyInfo p = info as PropertyInfo;
				if (p != null)
				{
					MethodInfo getter = p.GetGetMethod(true);
					if (getter != null)
					{
						yield return getter;
					}
				}
			}
		}

		private Dispatcher CreateGetter(MemberInfo member)
		{
			switch (member.MemberType)
			{
			case MemberTypes.Field:
			{
				FieldInfo field = (FieldInfo)member;
				return (object o, object[] arguments) => RuntimeServices.GetSlice(field.GetValue(o), string.Empty, arguments);
			}
			case MemberTypes.Property:
			{
				MethodInfo getter = ((PropertyInfo)member).GetGetMethod(true);
				if (getter == null)
				{
					throw MissingField();
				}
				if (getter.GetParameters().Length > 0)
				{
					return EmitMethodDispatcher(getter);
				}
				return (object o, object[] arguments) => RuntimeServices.GetSlice(getter.Invoke(o, null), string.Empty, arguments);
			}
			default:
				throw MissingField();
			}
		}

		private Dispatcher EmitMethodDispatcher(MethodInfo candidate)
		{
			return EmitMethodDispatcher(new MethodInfo[1] { candidate });
		}

		private Dispatcher EmitMethodDispatcher(IEnumerable<MethodInfo> candidates)
		{
			CandidateMethod candidateMethod = AbstractDispatcherFactory.ResolveMethod(GetArgumentTypes(), candidates);
			if (candidateMethod == null)
			{
				throw MissingField();
			}
			return new MethodDispatcherEmitter(_type, candidateMethod, GetArgumentTypes()).Emit();
		}

		private MemberInfo[] ResolveMember()
		{
			MemberInfo[] member = _type.GetMember(_name, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.OptionalParamBinding);
			if (member.Length == 0)
			{
				throw MissingField();
			}
			return member;
		}

		public Dispatcher CreateSetter()
		{
			MemberInfo[] array = ResolveMember();
			if (array.Length == 1)
			{
				return CreateSetter(array[0]);
			}
			return EmitMethodDispatcher(Setters(array));
		}

		private IEnumerable<MethodInfo> Setters(MemberInfo[] candidates)
		{
			foreach (MemberInfo info in candidates)
			{
				PropertyInfo p = info as PropertyInfo;
				if (p != null)
				{
					MethodInfo setter = p.GetSetMethod(true);
					if (setter != null)
					{
						yield return setter;
					}
				}
			}
		}

		private Dispatcher CreateSetter(MemberInfo member)
		{
			switch (member.MemberType)
			{
			case MemberTypes.Field:
			{
				FieldInfo field = (FieldInfo)member;
				return (object o, object[] arguments) => RuntimeServices.SetSlice(field.GetValue(o), string.Empty, arguments);
			}
			case MemberTypes.Property:
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				if (propertyInfo.GetIndexParameters().Length > 0)
				{
					MethodInfo setMethod = propertyInfo.GetSetMethod(true);
					if (setMethod == null)
					{
						throw MissingField();
					}
					return EmitMethodDispatcher(setMethod);
				}
				return (object o, object[] arguments) => RuntimeServices.SetSlice(RuntimeServices.GetProperty(o, _name), string.Empty, arguments);
			}
			default:
				throw MissingField();
			}
		}
	}
}
