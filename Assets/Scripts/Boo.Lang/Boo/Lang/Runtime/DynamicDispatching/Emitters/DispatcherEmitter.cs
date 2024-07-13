using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Boo.Lang.Runtime.DynamicDispatching.Emitters
{
	public abstract class DispatcherEmitter
	{
		private DynamicMethod _dynamicMethod;

		protected readonly ILGenerator _il;

		public DispatcherEmitter(Type owner, string dynamicMethodName)
		{
			_dynamicMethod = new DynamicMethod(owner.Name + "$" + dynamicMethodName, typeof(object), new Type[2]
			{
				typeof(object),
				typeof(object[])
			}, owner);
			_il = _dynamicMethod.GetILGenerator();
		}

		public Dispatcher Emit()
		{
			EmitMethodBody();
			return CreateMethodDispatcher();
		}

		protected abstract void EmitMethodBody();

		protected Dispatcher CreateMethodDispatcher()
		{
			return (Dispatcher)_dynamicMethod.CreateDelegate(typeof(Dispatcher));
		}

		protected bool IsStobj(OpCode code)
		{
			return OpCodes.Stobj.Value == code.Value;
		}

		protected void EmitCastOrUnbox(Type type)
		{
			if (type.IsValueType)
			{
				_il.Emit(OpCodes.Unbox, type);
				_il.Emit(OpCodes.Ldobj, type);
			}
			else
			{
				_il.Emit(OpCodes.Castclass, type);
			}
		}

		protected void BoxIfNeeded(Type returnType)
		{
			if (returnType.IsValueType)
			{
				_il.Emit(OpCodes.Box, returnType);
			}
		}

		protected void EmitLoadTargetObject(Type expectedType)
		{
			_il.Emit(OpCodes.Ldarg_0);
			if (expectedType.IsValueType)
			{
				_il.Emit(OpCodes.Unbox, expectedType);
			}
			else
			{
				_il.Emit(OpCodes.Castclass, expectedType);
			}
		}

		protected void EmitReturn(Type typeOnStack)
		{
			if (typeOnStack == typeof(void))
			{
				_il.Emit(OpCodes.Ldnull);
			}
			else
			{
				BoxIfNeeded(typeOnStack);
			}
			_il.Emit(OpCodes.Ret);
		}

		protected void EmitPromotion(Type expectedType, Type actualType)
		{
			_il.Emit(OpCodes.Unbox_Any, actualType);
			_il.Emit(NumericPromotionOpcodeFor(Type.GetTypeCode(expectedType), true));
		}

		private static OpCode NumericPromotionOpcodeFor(TypeCode typeCode, bool @checked)
		{
			switch (typeCode)
			{
			case TypeCode.SByte:
				return (!@checked) ? OpCodes.Conv_I1 : OpCodes.Conv_Ovf_I1;
			case TypeCode.Byte:
				return (!@checked) ? OpCodes.Conv_U1 : OpCodes.Conv_Ovf_U1;
			case TypeCode.Int16:
				return (!@checked) ? OpCodes.Conv_I2 : OpCodes.Conv_Ovf_I2;
			case TypeCode.Char:
			case TypeCode.UInt16:
				return (!@checked) ? OpCodes.Conv_U2 : OpCodes.Conv_Ovf_U2;
			case TypeCode.Int32:
				return (!@checked) ? OpCodes.Conv_I4 : OpCodes.Conv_Ovf_I4;
			case TypeCode.UInt32:
				return (!@checked) ? OpCodes.Conv_U4 : OpCodes.Conv_Ovf_U4;
			case TypeCode.Int64:
				return (!@checked) ? OpCodes.Conv_I8 : OpCodes.Conv_Ovf_I8;
			case TypeCode.UInt64:
				return (!@checked) ? OpCodes.Conv_U8 : OpCodes.Conv_Ovf_U8;
			case TypeCode.Single:
				return OpCodes.Conv_R4;
			case TypeCode.Double:
				return OpCodes.Conv_R8;
			default:
				throw new ArgumentException(typeCode.ToString());
			}
		}

		protected void EmitArgArrayElement(int argumentIndex)
		{
			_il.Emit(OpCodes.Ldarg_1);
			_il.Emit(OpCodes.Ldc_I4, argumentIndex);
			_il.Emit(OpCodes.Ldelem_Ref);
		}

		private MethodInfo GetPromotionMethod(Type type)
		{
			return typeof(IConvertible).GetMethod("To" + Type.GetTypeCode(type));
		}

		protected void Dup()
		{
			_il.Emit(OpCodes.Dup);
		}

		protected void EmitCoercion(Type actualType, Type expectedType, int score)
		{
			switch (score)
			{
			case 3:
			case 5:
				EmitPromotion(expectedType, actualType);
				break;
			case 4:
				EmitCastOrUnbox(actualType);
				_il.Emit(OpCodes.Call, RuntimeServices.FindImplicitConversionOperator(actualType, expectedType));
				break;
			default:
				EmitCastOrUnbox(expectedType);
				break;
			}
		}

		protected void LoadLocal(LocalBuilder value)
		{
			_il.Emit(OpCodes.Ldloc, value);
		}

		protected void StoreLocal(LocalBuilder value)
		{
			_il.Emit(OpCodes.Stloc, value);
		}

		protected LocalBuilder DeclareLocal(Type type)
		{
			return _il.DeclareLocal(type);
		}
	}
}
