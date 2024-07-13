using System;
using System.Collections.Generic;
using System.Reflection;

namespace Boo.Lang.Runtime
{
	public class MethodResolver
	{
		private readonly Type[] _arguments;

		public MethodResolver(params Type[] argumentTypes)
		{
			_arguments = argumentTypes;
		}

		public static Type[] GetArgumentTypes(object[] arguments)
		{
			if (arguments.Length == 0)
			{
				return Type.EmptyTypes;
			}
			Type[] array = new Type[arguments.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetObjectTypeOrNull(arguments[i]);
			}
			return array;
		}

		private static Type GetObjectTypeOrNull(object arg)
		{
			if (arg == null)
			{
				return null;
			}
			return arg.GetType();
		}

		public CandidateMethod ResolveMethod(IEnumerable<MethodInfo> candidates)
		{
			List<CandidateMethod> list = FindApplicableMethods(candidates);
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			List<CandidateMethod> list2 = list.FindAll(DoesNotRequireConversions);
			if (list2.Count > 0)
			{
				return BestMethod(list2);
			}
			return BestMethod(list);
		}

		private static bool DoesNotRequireConversions(CandidateMethod candidate)
		{
			return candidate.DoesNotRequireConversions;
		}

		private CandidateMethod BestMethod(List<CandidateMethod> applicable)
		{
			applicable.Sort(BetterCandidate);
			return applicable[applicable.Count - 1];
		}

		private static int TotalScore(CandidateMethod c1)
		{
			int num = 0;
			int[] argumentScores = c1.ArgumentScores;
			foreach (int num2 in argumentScores)
			{
				num += num2;
			}
			return num;
		}

		private int BetterCandidate(CandidateMethod c1, CandidateMethod c2)
		{
			int num = Math.Sign(TotalScore(c1) - TotalScore(c2));
			if (num != 0)
			{
				return num;
			}
			if (c1.VarArgs && !c2.VarArgs)
			{
				return -1;
			}
			if (c2.VarArgs && !c1.VarArgs)
			{
				return 1;
			}
			int num2 = Math.Min(c1.MinimumArgumentCount, c2.MinimumArgumentCount);
			for (int i = 0; i < num2; i++)
			{
				num += MoreSpecificType(c1.GetParameterType(i), c2.GetParameterType(i));
			}
			if (num != 0)
			{
				return num;
			}
			if (c1.VarArgs && c2.VarArgs)
			{
				return MoreSpecificType(c1.VarArgsParameterType, c2.VarArgsParameterType);
			}
			return 0;
		}

		private static int MoreSpecificType(Type t1, Type t2)
		{
			int num = GetTypeGenerity(t2) - GetTypeGenerity(t1);
			if (num != 0)
			{
				return num;
			}
			return GetLogicalTypeDepth(t1) - GetLogicalTypeDepth(t2);
		}

		private static int GetTypeGenerity(Type type)
		{
			if (!type.ContainsGenericParameters)
			{
				return 0;
			}
			return type.GetGenericArguments().Length;
		}

		private static int GetLogicalTypeDepth(Type type)
		{
			int typeDepth = GetTypeDepth(type);
			return (!type.IsValueType) ? typeDepth : (typeDepth - 1);
		}

		private static int GetTypeDepth(Type type)
		{
			if (type.IsByRef)
			{
				return GetTypeDepth(type.GetElementType());
			}
			if (type.IsInterface)
			{
				return GetInterfaceDepth(type);
			}
			return GetClassDepth(type);
		}

		private static int GetClassDepth(Type type)
		{
			int num = 0;
			Type typeFromHandle = typeof(object);
			while (type != null && type != typeFromHandle)
			{
				type = type.BaseType;
				num++;
			}
			return num;
		}

		private static int GetInterfaceDepth(Type type)
		{
			Type[] interfaces = type.GetInterfaces();
			if (interfaces.Length > 0)
			{
				int num = 0;
				Type[] array = interfaces;
				foreach (Type type2 in array)
				{
					int interfaceDepth = GetInterfaceDepth(type2);
					if (interfaceDepth > num)
					{
						num = interfaceDepth;
					}
				}
				return 1 + num;
			}
			return 1;
		}

		private List<CandidateMethod> FindApplicableMethods(IEnumerable<MethodInfo> candidates)
		{
			List<CandidateMethod> list = new List<CandidateMethod>();
			foreach (MethodInfo candidate in candidates)
			{
				CandidateMethod candidateMethod = IsApplicableMethod(candidate);
				if (candidateMethod != null)
				{
					list.Add(candidateMethod);
				}
			}
			return list;
		}

		private CandidateMethod IsApplicableMethod(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			bool flag = IsVarArgs(parameters);
			if (!ValidArgumentCount(parameters, flag))
			{
				return null;
			}
			CandidateMethod candidateMethod = new CandidateMethod(method, _arguments.Length, flag);
			if (CalculateCandidateScore(candidateMethod))
			{
				return candidateMethod;
			}
			return null;
		}

		private bool ValidArgumentCount(ParameterInfo[] parameters, bool varargs)
		{
			if (varargs)
			{
				int num = parameters.Length - 1;
				return _arguments.Length >= num;
			}
			return _arguments.Length == parameters.Length;
		}

		private bool IsVarArgs(ParameterInfo[] parameters)
		{
			if (parameters.Length == 0)
			{
				return false;
			}
			return HasParamArrayAttribute(parameters[parameters.Length - 1]);
		}

		private bool HasParamArrayAttribute(ParameterInfo info)
		{
			return info.IsDefined(typeof(ParamArrayAttribute), true);
		}

		private bool CalculateCandidateScore(CandidateMethod candidateMethod)
		{
			ParameterInfo[] parameters = candidateMethod.Parameters;
			for (int i = 0; i < candidateMethod.MinimumArgumentCount; i++)
			{
				if (parameters[i].IsOut)
				{
					return false;
				}
				if (!CalculateCandidateArgumentScore(candidateMethod, i, parameters[i].ParameterType))
				{
					return false;
				}
			}
			if (candidateMethod.VarArgs)
			{
				Type varArgsParameterType = candidateMethod.VarArgsParameterType;
				for (int j = candidateMethod.MinimumArgumentCount; j < _arguments.Length; j++)
				{
					if (!CalculateCandidateArgumentScore(candidateMethod, j, varArgsParameterType))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool CalculateCandidateArgumentScore(CandidateMethod candidateMethod, int argumentIndex, Type paramType)
		{
			int num = CandidateMethod.CalculateArgumentScore(paramType, _arguments[argumentIndex]);
			if (num < 0)
			{
				return false;
			}
			candidateMethod.ArgumentScores[argumentIndex] = num;
			return true;
		}
	}
}
