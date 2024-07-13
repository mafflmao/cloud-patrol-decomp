using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime.DynamicDispatching.Emitters
{
	internal class SetPropertyEmitter : MethodDispatcherEmitter
	{
		public SetPropertyEmitter(Type type, CandidateMethod found, Type[] argumentTypes)
			: base(type, found, argumentTypes)
		{
		}

		protected override void EmitMethodBody()
		{
			Type valueType = GetValueType();
			LocalBuilder value = DeclareLocal(valueType);
			EmitLoadTargetObject();
			EmitMethodArguments();
			Dup();
			StoreLocal(value);
			EmitMethodCall();
			LoadLocal(value);
			EmitReturn(valueType);
		}

		private Type GetValueType()
		{
			ParameterInfo[] parameters = _found.Parameters;
			return parameters[parameters.Length - 1].ParameterType;
		}
	}
}
