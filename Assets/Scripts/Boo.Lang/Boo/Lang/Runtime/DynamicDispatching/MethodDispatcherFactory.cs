using System;
using System.Collections.Generic;
using System.Reflection;
using Boo.Lang.Runtime.DynamicDispatching.Emitters;

namespace Boo.Lang.Runtime.DynamicDispatching
{
	public class MethodDispatcherFactory : AbstractDispatcherFactory
	{
		public MethodDispatcherFactory(ExtensionRegistry extensions, object target, Type type, string methodName, params object[] arguments)
			: base(extensions, target, type, methodName, arguments)
		{
		}

		public Dispatcher Create()
		{
			Type[] argumentTypes = GetArgumentTypes();
			CandidateMethod candidateMethod = ResolveMethod(argumentTypes);
			if (candidateMethod != null)
			{
				return EmitMethodDispatcher(candidateMethod, argumentTypes);
			}
			return ProduceExtensionDispatcher();
		}

		private Dispatcher ProduceExtensionDispatcher()
		{
			CandidateMethod candidateMethod = ResolveExtensionMethod();
			if (candidateMethod == null)
			{
				throw new MissingMethodException(_type.FullName + "." + _name);
			}
			return EmitExtensionDispatcher(candidateMethod);
		}

		private CandidateMethod ResolveExtensionMethod()
		{
			return ResolveExtension(GetExtensionMethods());
		}

		private CandidateMethod ResolveMethod(Type[] argumentTypes)
		{
			return AbstractDispatcherFactory.ResolveMethod(argumentTypes, GetCandidates());
		}

		private IEnumerable<MethodInfo> GetCandidates()
		{
			MethodInfo[] methods = _type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.OptionalParamBinding);
			foreach (MethodInfo method in methods)
			{
				if (!(_name != method.Name))
				{
					yield return method;
				}
			}
		}

		private Dispatcher EmitMethodDispatcher(CandidateMethod found, Type[] argumentTypes)
		{
			return new MethodDispatcherEmitter(_type, found, argumentTypes).Emit();
		}
	}
}
