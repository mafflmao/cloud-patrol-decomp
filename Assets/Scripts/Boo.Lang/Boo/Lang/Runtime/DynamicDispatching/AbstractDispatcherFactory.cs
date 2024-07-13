using System;
using System.Collections.Generic;
using System.Reflection;
using Boo.Lang.Runtime.DynamicDispatching.Emitters;

namespace Boo.Lang.Runtime.DynamicDispatching
{
	public abstract class AbstractDispatcherFactory
	{
		private readonly ExtensionRegistry _extensions;

		private readonly object _target;

		protected readonly Type _type;

		protected readonly string _name;

		private readonly object[] _arguments;

		protected IEnumerable<MemberInfo> Extensions
		{
			get
			{
				return _extensions.Extensions;
			}
		}

		public AbstractDispatcherFactory(ExtensionRegistry extensions, object target, Type type, string name, params object[] arguments)
		{
			_extensions = extensions;
			_target = target;
			_type = type;
			_name = name;
			_arguments = arguments;
		}

		protected object[] GetExtensionArgs()
		{
			return AdjustExtensionArgs(_target, _arguments);
		}

		private static object[] AdjustExtensionArgs(object target, object[] originalArguments)
		{
			if (originalArguments == null)
			{
				return new object[1] { target };
			}
			object[] array = new object[originalArguments.Length + 1];
			array[0] = target;
			Array.Copy(originalArguments, 0, array, 1, originalArguments.Length);
			return array;
		}

		protected Type[] GetArgumentTypes()
		{
			return MethodResolver.GetArgumentTypes(_arguments);
		}

		protected Type[] GetExtensionArgumentTypes()
		{
			return MethodResolver.GetArgumentTypes(GetExtensionArgs());
		}

		protected Dispatcher EmitExtensionDispatcher(CandidateMethod found)
		{
			return new ExtensionMethodDispatcherEmitter(found, GetArgumentTypes()).Emit();
		}

		protected CandidateMethod ResolveExtension(IEnumerable<MethodInfo> candidates)
		{
			MethodResolver methodResolver = new MethodResolver(GetExtensionArgumentTypes());
			return methodResolver.ResolveMethod(candidates);
		}

		protected IEnumerable<MethodInfo> GetExtensionMethods()
		{
			return GetExtensions<MethodInfo>(MemberTypes.Method);
		}

		protected IEnumerable<T> GetExtensions<T>(MemberTypes memberTypes) where T : MemberInfo
		{
			foreach (MemberInfo i in Extensions)
			{
				if (i.MemberType == memberTypes && !(i.Name != _name))
				{
					yield return (T)i;
				}
			}
		}

		protected static CandidateMethod ResolveMethod(Type[] argumentTypes, IEnumerable<MethodInfo> candidates)
		{
			return new MethodResolver(argumentTypes).ResolveMethod(candidates);
		}

		protected MissingFieldException MissingField()
		{
			return new MissingFieldException(_type.FullName + "." + _name);
		}
	}
}
