using System;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime.DynamicDispatching.Emitters
{
	public class MethodDispatcherEmitter : DispatcherEmitter
	{
		protected readonly CandidateMethod _found;

		protected readonly Type[] _argumentTypes;

		protected virtual int FixedArgumentOffset
		{
			get
			{
				return 0;
			}
		}

		public MethodDispatcherEmitter(CandidateMethod found, params Type[] argumentTypes)
			: this(found.Method.DeclaringType, found, argumentTypes)
		{
		}

		public MethodDispatcherEmitter(Type owner, CandidateMethod found, Type[] argumentTypes)
			: base(owner, found.Method.Name + "$" + Builtins.join(argumentTypes, "$"))
		{
			_found = found;
			_argumentTypes = argumentTypes;
		}

		protected override void EmitMethodBody()
		{
			EmitInvocation();
			EmitMethodReturn();
		}

		protected void EmitInvocation()
		{
			EmitLoadTargetObject();
			EmitMethodArguments();
			EmitMethodCall();
		}

		protected void EmitMethodCall()
		{
			_il.Emit((!_found.Method.IsStatic) ? OpCodes.Callvirt : OpCodes.Call, _found.Method);
		}

		protected void EmitMethodArguments()
		{
			EmitFixedMethodArguments();
			if (_found.VarArgs)
			{
				EmitVarArgsMethodArguments();
			}
		}

		private void EmitFixedMethodArguments()
		{
			int fixedArgumentOffset = FixedArgumentOffset;
			int num = MinimumArgumentCount();
			for (int i = 0; i < num; i++)
			{
				Type parameterType = _found.GetParameterType(i + fixedArgumentOffset);
				EmitMethodArgument(i, parameterType);
			}
		}

		private void EmitVarArgsMethodArguments()
		{
			int num = _argumentTypes.Length - MinimumArgumentCount();
			Type varArgsParameterType = _found.VarArgsParameterType;
			OpCode storeElementOpCode = GetStoreElementOpCode(varArgsParameterType);
			_il.Emit(OpCodes.Ldc_I4, num);
			_il.Emit(OpCodes.Newarr, varArgsParameterType);
			for (int i = 0; i < num; i++)
			{
				Dup();
				_il.Emit(OpCodes.Ldc_I4, i);
				if (IsStobj(storeElementOpCode))
				{
					_il.Emit(OpCodes.Ldelema, varArgsParameterType);
					EmitMethodArgument(MinimumArgumentCount() + i, varArgsParameterType);
					_il.Emit(storeElementOpCode, varArgsParameterType);
				}
				else
				{
					EmitMethodArgument(MinimumArgumentCount() + i, varArgsParameterType);
					_il.Emit(storeElementOpCode);
				}
			}
		}

		private int MinimumArgumentCount()
		{
			return _found.MinimumArgumentCount - FixedArgumentOffset;
		}

		private static OpCode GetStoreElementOpCode(Type type)
		{
			if (type.IsValueType)
			{
				if (type.IsEnum)
				{
					return OpCodes.Stelem_I4;
				}
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Byte:
					return OpCodes.Stelem_I1;
				case TypeCode.Int16:
					return OpCodes.Stelem_I2;
				case TypeCode.Int32:
					return OpCodes.Stelem_I4;
				case TypeCode.Int64:
					return OpCodes.Stelem_I8;
				case TypeCode.Single:
					return OpCodes.Stelem_R4;
				case TypeCode.Double:
					return OpCodes.Stelem_R8;
				default:
					return OpCodes.Stobj;
				}
			}
			return OpCodes.Stelem_Ref;
		}

		protected void EmitMethodArgument(int argumentIndex, Type expectedType)
		{
			EmitArgArrayElement(argumentIndex);
			EmitCoercion(argumentIndex, expectedType, _found.ArgumentScores[argumentIndex]);
		}

		private void EmitCoercion(int argumentIndex, Type expectedType, int score)
		{
			EmitCoercion(_argumentTypes[argumentIndex], expectedType, score);
		}

		protected virtual void EmitLoadTargetObject()
		{
			if (!_found.Method.IsStatic)
			{
				EmitLoadTargetObject(_found.Method.DeclaringType);
			}
		}

		private void EmitMethodReturn()
		{
			EmitReturn(_found.Method.ReturnType);
		}
	}
}
