using System;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public class CandidateMethod
	{
		public const int ExactMatchScore = 7;

		public const int UpCastScore = 6;

		public const int WideningPromotion = 5;

		public const int ImplicitConversionScore = 4;

		public const int NarrowingPromotion = 3;

		public const int DowncastScore = 2;

		private readonly MethodInfo _method;

		private readonly int[] _argumentScores;

		private readonly bool _varArgs;

		public MethodInfo Method
		{
			get
			{
				return _method;
			}
		}

		public int[] ArgumentScores
		{
			get
			{
				return _argumentScores;
			}
		}

		public bool VarArgs
		{
			get
			{
				return _varArgs;
			}
		}

		public int MinimumArgumentCount
		{
			get
			{
				return (!_varArgs) ? Parameters.Length : (Parameters.Length - 1);
			}
		}

		public ParameterInfo[] Parameters
		{
			get
			{
				return _method.GetParameters();
			}
		}

		public Type VarArgsParameterType
		{
			get
			{
				return GetParameterType(Parameters.Length - 1).GetElementType();
			}
		}

		public bool DoesNotRequireConversions
		{
			get
			{
				return !RequiresConversions;
			}
		}

		private bool RequiresConversions
		{
			get
			{
				return Array.Exists(_argumentScores, RequiresConversion);
			}
		}

		public CandidateMethod(MethodInfo method, int argumentCount, bool varArgs)
		{
			_method = method;
			_argumentScores = new int[argumentCount];
			_varArgs = varArgs;
		}

		public static int CalculateArgumentScore(Type paramType, Type argType)
		{
			if (argType == null)
			{
				return paramType.IsValueType ? (-1) : 7;
			}
			if (paramType == argType)
			{
				return 7;
			}
			if (paramType.IsAssignableFrom(argType))
			{
				return 6;
			}
			if (argType.IsAssignableFrom(paramType))
			{
				return 2;
			}
			if (IsNumericPromotion(paramType, argType))
			{
				return (!NumericTypes.IsWideningPromotion(paramType, argType)) ? 3 : 5;
			}
			MethodInfo methodInfo = RuntimeServices.FindImplicitConversionOperator(argType, paramType);
			if (methodInfo != null)
			{
				return 4;
			}
			return -1;
		}

		private static bool RequiresConversion(int score)
		{
			return score < 5;
		}

		public Type GetParameterType(int i)
		{
			return Parameters[i].ParameterType;
		}

		public static bool IsNumericPromotion(Type paramType, Type argType)
		{
			return RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(paramType)) && RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(argType));
		}

		public object DynamicInvoke(object target, object[] args)
		{
			return _method.Invoke(target, AdjustArgumentsForInvocation(args));
		}

		private object[] AdjustArgumentsForInvocation(object[] arguments)
		{
			if (VarArgs)
			{
				Type varArgsParameterType = VarArgsParameterType;
				int minimumArgumentCount = MinimumArgumentCount;
				object[] array = new object[minimumArgumentCount + 1];
				for (int i = 0; i < minimumArgumentCount; i++)
				{
					array[i] = ((!RequiresConversion(ArgumentScores[i])) ? arguments[i] : RuntimeServices.Coerce(arguments[i], GetParameterType(i)));
				}
				array[minimumArgumentCount] = CreateVarArgsArray(arguments, minimumArgumentCount, varArgsParameterType);
				return array;
			}
			if (RequiresConversions)
			{
				for (int j = 0; j < arguments.Length; j++)
				{
					arguments[j] = ((!RequiresConversion(ArgumentScores[j])) ? arguments[j] : RuntimeServices.Coerce(arguments[j], GetParameterType(j)));
				}
			}
			return arguments;
		}

		private static Array CreateVarArgsArray(object[] arguments, int minimumArgumentCount, Type varArgsParameterType)
		{
			int length = arguments.Length - minimumArgumentCount;
			Array array = Array.CreateInstance(varArgsParameterType, length);
			for (int i = 0; i < array.Length; i++)
			{
				array.SetValue(RuntimeServices.Coerce(arguments[minimumArgumentCount + i], varArgsParameterType), i);
			}
			return array;
		}
	}
}
