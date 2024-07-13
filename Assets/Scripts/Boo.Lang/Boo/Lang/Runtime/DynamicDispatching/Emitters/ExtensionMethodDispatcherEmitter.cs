using System;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime.DynamicDispatching.Emitters
{
	internal class ExtensionMethodDispatcherEmitter : MethodDispatcherEmitter
	{
		protected override int FixedArgumentOffset
		{
			get
			{
				return 1;
			}
		}

		public ExtensionMethodDispatcherEmitter(CandidateMethod found, Type[] argumentTypes)
			: base(found, argumentTypes)
		{
		}

		protected override void EmitLoadTargetObject()
		{
			_il.Emit(OpCodes.Ldarg_0);
			EmitCastOrUnbox(_found.GetParameterType(0));
		}
	}
}
