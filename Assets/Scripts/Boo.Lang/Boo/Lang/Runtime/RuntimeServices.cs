using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Boo.Lang.Runtime.DynamicDispatching;
using Boo.Lang.Runtime.DynamicDispatching.Emitters;

namespace Boo.Lang.Runtime
{
	public class RuntimeServices
	{
		public struct ValueTypeChange
		{
			public object Target;

			public string Member;

			public object Value;

			public ValueTypeChange(object target, string member, object value)
			{
				Target = target;
				Member = member;
				Value = value;
			}
		}

		public delegate void CodeBlock();

		internal const BindingFlags InstanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		internal const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.OptionalParamBinding;

		private const BindingFlags InvokeBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.OptionalParamBinding;

		private const BindingFlags SetPropertyBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.OptionalParamBinding;

		private const BindingFlags GetPropertyBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.OptionalParamBinding;

		private static readonly object[] NoArguments = new object[0];

		private static readonly Type RuntimeServicesType = typeof(RuntimeServices);

		private static readonly DispatcherCache _cache = new DispatcherCache();

		private static readonly ExtensionRegistry _extensions = new ExtensionRegistry();

		private static readonly object True = true;

		public static string RuntimeDisplayName
		{
			get
			{
				Type type = Type.GetType("Mono.Runtime");
				return (type == null) ? ("CLR " + Environment.Version.ToString()) : ((string)type.GetMethod("GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null));
			}
		}

		public static void WithExtensions(Type extensions, CodeBlock block)
		{
			RegisterExtensions(extensions);
			try
			{
				block();
			}
			finally
			{
				UnRegisterExtensions(extensions);
			}
		}

		public static void RegisterExtensions(Type extensions)
		{
			_extensions.Register(extensions);
		}

		public static void UnRegisterExtensions(Type extensions)
		{
			_extensions.UnRegister(extensions);
		}

		public static object Invoke(object target, string name, object[] args)
		{
			Dispatcher dispatcher = GetDispatcher(target, args, name, () => CreateMethodDispatcher(target, name, args));
			return dispatcher(target, args);
		}

		private static Dispatcher CreateMethodDispatcher(object target, string name, object[] args)
		{
			IQuackFu quackFu = target as IQuackFu;
			if (quackFu != null)
			{
				return (object o, object[] arguments) => ((IQuackFu)o).QuackInvoke(name, arguments);
			}
			Type type = target as Type;
			if (type != null)
			{
				return DoCreateMethodDispatcher(null, type, name, args);
			}
			Type type2 = target.GetType();
			if (type2.IsCOMObject)
			{
				return (object o, object[] arguments) => o.GetType().InvokeMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.OptionalParamBinding, null, target, arguments);
			}
			return DoCreateMethodDispatcher(target, type2, name, args);
		}

		private static Dispatcher DoCreateMethodDispatcher(object target, Type targetType, string name, object[] args)
		{
			return new MethodDispatcherFactory(_extensions, target, targetType, name, args).Create();
		}

		private static Dispatcher GetDispatcher(object target, object[] args, string cacheKeyName, DispatcherCache.DispatcherFactory factory)
		{
			Type[] argumentTypes = MethodResolver.GetArgumentTypes(args);
			return GetDispatcher(target, cacheKeyName, argumentTypes, factory);
		}

		private static Dispatcher GetDispatcher(object target, string cacheKeyName, Type[] cacheKeyTypes, DispatcherCache.DispatcherFactory factory)
		{
			Type type = (target as Type) ?? target.GetType();
			DispatcherKey key = new DispatcherKey(type, cacheKeyName, cacheKeyTypes);
			return _cache.Get(key, factory);
		}

		public static object GetProperty(object target, string name)
		{
			Dispatcher dispatcher = GetDispatcher(target, NoArguments, name, () => CreatePropGetDispatcher(target, name));
			return dispatcher(target, NoArguments);
		}

		private static Dispatcher CreatePropGetDispatcher(object target, string name)
		{
			IQuackFu quackFu = target as IQuackFu;
			if (quackFu != null)
			{
				return (object o, object[] args) => ((IQuackFu)o).QuackGet(name, null);
			}
			Type type = target as Type;
			if (type != null)
			{
				return DoCreatePropGetDispatcher(null, type, name);
			}
			Type type2 = target.GetType();
			if (type2.IsCOMObject)
			{
				return (object o, object[] args) => o.GetType().InvokeMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.OptionalParamBinding, null, o, null);
			}
			return DoCreatePropGetDispatcher(target, target.GetType(), name);
		}

		private static Dispatcher DoCreatePropGetDispatcher(object target, Type type, string name)
		{
			return new PropertyDispatcherFactory(_extensions, target, type, name).CreateGetter();
		}

		public static object SetProperty(object target, string name, object value)
		{
			object[] args = new object[1] { value };
			Dispatcher dispatcher = GetDispatcher(target, args, name, () => CreatePropSetDispatcher(target, name, value));
			return dispatcher(target, args);
		}

		private static Dispatcher CreatePropSetDispatcher(object target, string name, object value)
		{
			IQuackFu quackFu = target as IQuackFu;
			if (quackFu != null)
			{
				return (object o, object[] args) => ((IQuackFu)o).QuackSet(name, null, args[0]);
			}
			Type type = target as Type;
			if (type != null)
			{
				return DoCreatePropSetDispatcher(null, type, name, value);
			}
			Type type2 = target.GetType();
			if (type2.IsCOMObject)
			{
				return (object o, object[] args) => o.GetType().InvokeMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.OptionalParamBinding, null, o, args);
			}
			return DoCreatePropSetDispatcher(target, type2, name, value);
		}

		private static Dispatcher DoCreatePropSetDispatcher(object target, Type type, string name, object value)
		{
			return new PropertyDispatcherFactory(_extensions, target, type, name, value).CreateSetter();
		}

		public static void PropagateValueTypeChanges(ValueTypeChange[] changes)
		{
			for (int i = 0; i < changes.Length; i++)
			{
				ValueTypeChange valueTypeChange = changes[i];
				if (!(valueTypeChange.Value is ValueType))
				{
					break;
				}
				try
				{
					SetProperty(valueTypeChange.Target, valueTypeChange.Member, valueTypeChange.Value);
				}
				catch (MissingFieldException)
				{
					break;
				}
			}
		}

		public static object Coerce(object value, Type toType)
		{
			if (value == null)
			{
				return null;
			}
			object[] args = new object[1] { toType };
			Dispatcher dispatcher = GetDispatcher(value, "$Coerce$", new Type[1] { toType }, () => CreateCoerceDispatcher(value, toType));
			return dispatcher(value, args);
		}

		private static Dispatcher CreateCoerceDispatcher(object value, Type toType)
		{
			if (toType.IsInstanceOfType(value))
			{
				return IdentityDispatcher;
			}
			if (value is ICoercible)
			{
				return CoercibleDispatcher;
			}
			Type type = value.GetType();
			if (IsPromotableNumeric(type) && IsPromotableNumeric(toType))
			{
				return EmitPromotionDispatcher(type, toType);
			}
			MethodInfo methodInfo = FindImplicitConversionOperator(type, toType);
			if (methodInfo == null)
			{
				return IdentityDispatcher;
			}
			return EmitImplicitConversionDispatcher(methodInfo);
		}

		private static Dispatcher EmitPromotionDispatcher(Type fromType, Type toType)
		{
			return (Dispatcher)Delegate.CreateDelegate(typeof(Dispatcher), typeof(NumericPromotions).GetMethod(string.Concat("From", Type.GetTypeCode(fromType), "To", Type.GetTypeCode(toType))));
		}

		private static bool IsPromotableNumeric(Type fromType)
		{
			return IsPromotableNumeric(Type.GetTypeCode(fromType));
		}

		private static Dispatcher EmitImplicitConversionDispatcher(MethodInfo method)
		{
			return new ImplicitConversionEmitter(method).Emit();
		}

		private static object CoercibleDispatcher(object o, object[] args)
		{
			return ((ICoercible)o).Coerce((Type)args[0]);
		}

		private static object IdentityDispatcher(object o, object[] args)
		{
			return o;
		}

		public static object GetSlice(object target, string name, object[] args)
		{
			Dispatcher dispatcher = GetDispatcher(target, args, name + "[]", () => CreateGetSliceDispatcher(target, name, args));
			return dispatcher(target, args);
		}

		private static Dispatcher CreateGetSliceDispatcher(object target, string name, object[] args)
		{
			IQuackFu quackFu = target as IQuackFu;
			if (quackFu != null)
			{
				return (object o, object[] arguments) => ((IQuackFu)o).QuackGet(name, arguments);
			}
			if (string.Empty == name && args.Length == 1 && target is Array)
			{
				return GetArraySlice;
			}
			return new SliceDispatcherFactory(_extensions, target, target.GetType(), name, args).CreateGetter();
		}

		private static object GetArraySlice(object target, object[] args)
		{
			IList list = (IList)target;
			return list[NormalizeIndex(list.Count, (int)args[0])];
		}

		public static object SetSlice(object target, string name, object[] args)
		{
			Dispatcher dispatcher = GetDispatcher(target, args, name + "[]=", () => CreateSetSliceDispatcher(target, name, args));
			return dispatcher(target, args);
		}

		private static Dispatcher CreateSetSliceDispatcher(object target, string name, object[] args)
		{
			IQuackFu quackFu = target as IQuackFu;
			if (quackFu != null)
			{
				return (object o, object[] arguments) => ((IQuackFu)o).QuackSet(name, (object[])GetRange2(arguments, 0, arguments.Length - 1), arguments[arguments.Length - 1]);
			}
			if (string.Empty == name && args.Length == 2 && target is Array)
			{
				return SetArraySlice;
			}
			return new SliceDispatcherFactory(_extensions, target, target.GetType(), name, args).CreateSetter();
		}

		private static object SetArraySlice(object target, object[] args)
		{
			IList list = (IList)target;
			list[NormalizeIndex(list.Count, (int)args[0])] = args[1];
			return args[1];
		}

		internal static string GetDefaultMemberName(Type type)
		{
			DefaultMemberAttribute defaultMemberAttribute = (DefaultMemberAttribute)Attribute.GetCustomAttribute(type, typeof(DefaultMemberAttribute));
			return (defaultMemberAttribute == null) ? string.Empty : defaultMemberAttribute.MemberName;
		}

		public static object InvokeCallable(object target, object[] args)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			ICallable callable = target as ICallable;
			if (callable != null)
			{
				return callable.Call(args);
			}
			Delegate @delegate = target as Delegate;
			if ((object)@delegate != null)
			{
				return @delegate.DynamicInvoke(args);
			}
			Type type = target as Type;
			return ((MethodInfo)target).Invoke(null, args);
		}

		private static bool IsNumeric(TypeCode code)
		{
			switch (code)
			{
			case TypeCode.Byte:
				return true;
			case TypeCode.SByte:
				return true;
			case TypeCode.Int16:
				return true;
			case TypeCode.Int32:
				return true;
			case TypeCode.Int64:
				return true;
			case TypeCode.UInt16:
				return true;
			case TypeCode.UInt32:
				return true;
			case TypeCode.UInt64:
				return true;
			case TypeCode.Single:
				return true;
			case TypeCode.Double:
				return true;
			case TypeCode.Decimal:
				return true;
			default:
				return false;
			}
		}

		public static object InvokeBinaryOperator(string operatorName, object lhs, object rhs)
		{
			Type type = lhs.GetType();
			Type type2 = rhs.GetType();
			TypeCode typeCode = Type.GetTypeCode(type);
			TypeCode typeCode2 = Type.GetTypeCode(type2);
			if (IsNumeric(typeCode) && IsNumeric(typeCode2))
			{
				switch ((int)(((uint)operatorName[3] << 8) + operatorName[operatorName.Length - 1]))
				{
				case 16750:
					return op_Addition(lhs, typeCode, rhs, typeCode2);
				case 21358:
					return op_Subtraction(lhs, typeCode, rhs, typeCode2);
				case 19833:
					return op_Multiply(lhs, typeCode, rhs, typeCode2);
				case 17518:
					return op_Division(lhs, typeCode, rhs, typeCode2);
				case 19827:
					return op_Modulus(lhs, typeCode, rhs, typeCode2);
				case 17774:
					return op_Exponentiation(lhs, typeCode, rhs, typeCode2);
				case 19566:
					return op_LessThan(lhs, typeCode, rhs, typeCode2);
				case 19564:
					return op_LessThanOrEqual(lhs, typeCode, rhs, typeCode2);
				case 18286:
					return op_GreaterThan(lhs, typeCode, rhs, typeCode2);
				case 18284:
					return op_GreaterThanOrEqual(lhs, typeCode, rhs, typeCode2);
				case 17010:
					return op_BitwiseOr(lhs, typeCode, rhs, typeCode2);
				case 16996:
					return op_BitwiseAnd(lhs, typeCode, rhs, typeCode2);
				case 17778:
					return op_ExclusiveOr(lhs, typeCode, rhs, typeCode2);
				case 21364:
					return (operatorName[8] != 'L') ? op_ShiftRight(lhs, typeCode, rhs, typeCode2) : op_ShiftLeft(lhs, typeCode, rhs, typeCode2);
				default:
					throw new MissingMethodException(MissingOperatorMessageFor(operatorName, type, type2));
				}
			}
			object[] args = new object[2] { lhs, rhs };
			IQuackFu quackFu = lhs as IQuackFu;
			if (quackFu != null)
			{
				return quackFu.QuackInvoke(operatorName, args);
			}
			quackFu = rhs as IQuackFu;
			if (quackFu != null)
			{
				return quackFu.QuackInvoke(operatorName, args);
			}
			try
			{
				return Invoke(type, operatorName, args);
			}
			catch (MissingMethodException inner)
			{
				try
				{
					return Invoke(type2, operatorName, args);
				}
				catch (MissingMethodException)
				{
					try
					{
						return InvokeRuntimeServicesOperator(operatorName, args);
					}
					catch (MissingMethodException)
					{
					}
				}
				throw new MissingMethodException(MissingOperatorMessageFor(operatorName, type, type2), inner);
			}
		}

		private static string MissingOperatorMessageFor(string operatorName, Type lhsType, Type rhsType)
		{
			return string.Format("{0} is not applicable to operands '{1}' and '{2}'.", FormatOperatorName(operatorName), lhsType, rhsType);
		}

		private static string FormatOperatorName(string operatorName)
		{
			StringBuilder stringBuilder = new StringBuilder(operatorName.Length);
			stringBuilder.Append(operatorName[3]);
			string text = operatorName.Substring(4);
			foreach (char c in text)
			{
				if (char.IsUpper(c))
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(char.ToLower(c));
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static object InvokeUnaryOperator(string operatorName, object operand)
		{
			Type type = operand.GetType();
			TypeCode typeCode = Type.GetTypeCode(type);
			if (IsNumeric(typeCode))
			{
				int num = (int)(((uint)operatorName[3] << 8) + operatorName[operatorName.Length - 1]);
				if (num == 21870)
				{
					return op_UnaryNegation(operand, typeCode);
				}
				throw new ArgumentException(operatorName + " " + operand);
			}
			object[] args = new object[1] { operand };
			IQuackFu quackFu = operand as IQuackFu;
			if (quackFu != null)
			{
				return quackFu.QuackInvoke(operatorName, args);
			}
			try
			{
				return Invoke(type, operatorName, args);
			}
			catch (MissingMethodException)
			{
				try
				{
					return InvokeRuntimeServicesOperator(operatorName, args);
				}
				catch (MissingMethodException)
				{
				}
				throw;
			}
		}

		private static object InvokeRuntimeServicesOperator(string operatorName, object[] args)
		{
			return Invoke(RuntimeServicesType, operatorName, args);
		}

		public static object MoveNext(IEnumerator enumerator)
		{
			if (enumerator == null)
			{
				throw new ApplicationException("Cannot unpack null.");
			}
			if (!enumerator.MoveNext())
			{
				throw new ApplicationException("Unpack list of wrong size.");
			}
			return enumerator.Current;
		}

		public static int Len(object obj)
		{
			if (obj != null)
			{
				ICollection collection = obj as ICollection;
				if (collection != null)
				{
					return collection.Count;
				}
				string text = obj as string;
				if (text != null)
				{
					return text.Length;
				}
			}
			throw new ArgumentException();
		}

		public static string Mid(string s, int begin, int end)
		{
			begin = NormalizeStringIndex(s, begin);
			end = NormalizeStringIndex(s, end);
			return s.Substring(begin, end - begin);
		}

		public static Array GetRange1(Array source, int begin)
		{
			return GetRange2(source, begin, source.Length);
		}

		public static Array GetRange2(Array source, int begin, int end)
		{
			int length = source.Length;
			begin = NormalizeIndex(length, begin);
			end = NormalizeIndex(length, end);
			int length2 = Math.Max(0, end - begin);
			Array array = Array.CreateInstance(source.GetType().GetElementType(), length2);
			Array.Copy(source, begin, array, 0, length2);
			return array;
		}

		public static void SetMultiDimensionalRange1(Array source, Array dest, int[] ranges, bool[] compute_end, bool[] collapse)
		{
			if (dest.Rank != ranges.Length / 2)
			{
				throw new Exception("invalid range passed: " + ranges.Length / 2 + ", expected " + dest.Rank * 2);
			}
			for (int i = 0; i < dest.Rank; i++)
			{
				if (compute_end[i])
				{
					ranges[2 * i + 1] = dest.GetLength(i);
				}
				if (ranges[2 * i] < 0 || ranges[2 * i] >= dest.GetLength(i) || ranges[2 * i + 1] > dest.GetLength(i) || ranges[2 * i + 1] <= ranges[2 * i])
				{
					throw new ApplicationException("Invalid array.");
				}
			}
			int num = 0;
			for (int j = 0; j < collapse.Length; j++)
			{
				if (!collapse[j])
				{
					num++;
				}
			}
			if (num == 0)
			{
				num = 1;
			}
			if (source.Rank != num)
			{
				throw new ApplicationException(string.Format("Cannot assign array of rank {0} into an array subset of rank {1}.", source.Rank, num));
			}
			int[] array = new int[dest.Rank];
			int[] array2 = new int[num];
			int[] array3 = new int[source.Rank];
			int num2 = 0;
			bool flag = false;
			for (int k = 0; k < dest.Rank; k++)
			{
				array[k] = ranges[2 * k + 1] - ranges[2 * k];
				if (!collapse[k])
				{
					array2[num2] = array[k];
					array3[num2] = source.GetLength(num2);
					if (array3[num2] != array[k])
					{
						flag = true;
					}
					num2++;
				}
			}
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder(array3[0]);
				StringBuilder stringBuilder2 = new StringBuilder(array2[0]);
				for (int l = 1; l < source.Rank; l++)
				{
					stringBuilder.Append(" x ");
					stringBuilder.Append(array3[l]);
					stringBuilder2.Append(" x ");
					stringBuilder2.Append(array2[l]);
				}
				throw new ApplicationException(string.Format("Cannot assign array with dimensions {0} into array subset of dimensions {1}.", stringBuilder.ToString(), stringBuilder2.ToString()));
			}
			int[] array4 = new int[source.Rank];
			array4[0] = array3[0];
			for (int m = 1; m < source.Rank; m++)
			{
				array4[m] = array4[m - 1] * array3[m];
			}
			int[] array5 = new int[dest.Rank];
			int[] array6 = new int[source.Rank];
			for (int n = 0; n < source.Length; n++)
			{
				int num3 = 0;
				for (int num4 = 0; num4 < dest.Rank; num4++)
				{
					if (collapse[num4])
					{
						array5[num4] = ranges[2 * num4];
						continue;
					}
					array6[num3] = n % array4[num3];
					array5[num4] = array6[num3] + ranges[2 * num4];
					num3++;
				}
				dest.SetValue(source.GetValue(array6), array5);
			}
		}

		public static Array GetMultiDimensionalRange1(Array source, int[] ranges, bool[] compute_end, bool[] collapse)
		{
			int rank = source.Rank;
			int[] array = new int[rank];
			int num = 0;
			for (int i = 0; i < rank; i++)
			{
				ranges[2 * i] = NormalizeIndex(source.GetLength(i), ranges[2 * i]);
				if (compute_end[i])
				{
					ranges[2 * i + 1] = source.GetLength(i);
				}
				else
				{
					ranges[2 * i + 1] = NormalizeIndex(source.GetLength(i), ranges[2 * i + 1]);
				}
				array[i] = ranges[2 * i + 1] - ranges[2 * i];
				num += (collapse[i] ? 1 : 0);
			}
			int num2 = rank - num;
			int[] array2 = new int[num2];
			int num3 = 0;
			for (int j = 0; j < rank; j++)
			{
				if (!collapse[j])
				{
					array2[num3] = array[j];
					num3++;
				}
			}
			if (num2 == 0)
			{
				num2 = 1;
				array2 = new int[1] { 1 };
			}
			Array array3 = Array.CreateInstance(source.GetType().GetElementType(), array2);
			int[] array4 = new int[rank];
			int[] array5 = new int[num2];
			int[] array6 = new int[rank];
			for (int k = 0; k < rank; k++)
			{
				if (k == 0)
				{
					array4[k] = array3.Length;
				}
				else
				{
					array4[k] = array4[k - 1] / array[k - 1];
				}
			}
			for (int l = 0; l < array3.Length; l++)
			{
				int num4 = 0;
				for (int m = 0; m < rank; m++)
				{
					int num5 = l % array4[m] / (array4[m] / array[m]);
					array6[m] = ranges[2 * m] + num5;
					if (!collapse[m])
					{
						array5[num4] = array6[m] - ranges[2 * m];
						num4++;
					}
				}
				array3.SetValue(source.GetValue(array6), array5);
			}
			return array3;
		}

		public static void CheckArrayUnpack(Array array, int expected)
		{
			if (array == null)
			{
				throw new ApplicationException("Cannot unpack null.");
			}
			if (expected > array.Length)
			{
				Error("Unpack array of wrong size (expected={0}, actual={1}).", expected, array.Length);
			}
		}

		public static int NormalizeIndex(int len, int index)
		{
			return (index >= 0) ? Math.Min(index, len) : Math.Max(0, index + len);
		}

		public static int NormalizeArrayIndex(Array array, int index)
		{
			return (index >= 0) ? Math.Min(index, array.Length) : Math.Max(0, index + array.Length);
		}

		public static int NormalizeStringIndex(string s, int index)
		{
			return (index >= 0) ? Math.Min(index, s.Length) : Math.Max(0, index + s.Length);
		}

		public static IEnumerable GetEnumerable(object enumerable)
		{
			if (enumerable == null)
			{
				throw new ApplicationException("Cannot enumerate null.");
			}
			IEnumerable enumerable2 = enumerable as IEnumerable;
			if (enumerable2 != null)
			{
				return enumerable2;
			}
			TextReader textReader = enumerable as TextReader;
			if (textReader != null)
			{
				return TextReaderEnumerator.lines(textReader);
			}
			throw new ApplicationException("Argument is not enumerable (does not implement System.Collections.IEnumerable).");
		}

		public static Array AddArrays(Type resultingElementType, Array lhs, Array rhs)
		{
			int length = lhs.Length + rhs.Length;
			Array array = Array.CreateInstance(resultingElementType, length);
			Array.Copy(lhs, 0, array, 0, lhs.Length);
			Array.Copy(rhs, 0, array, lhs.Length, rhs.Length);
			return array;
		}

		public static string op_Addition(string lhs, string rhs)
		{
			return lhs + rhs;
		}

		public static string op_Addition(string lhs, object rhs)
		{
			return lhs + rhs;
		}

		public static string op_Addition(object lhs, string rhs)
		{
			return string.Concat(lhs, rhs);
		}

		public static Array op_Multiply(Array lhs, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			Type type = lhs.GetType();
			if (type.GetArrayRank() != 1)
			{
				throw new ArgumentException("lhs");
			}
			int length = lhs.Length;
			Array array = Array.CreateInstance(type.GetElementType(), length * count);
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				Array.Copy(lhs, 0, array, num, length);
				num += length;
			}
			return array;
		}

		public static string op_Multiply(string lhs, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			string result = null;
			if (lhs != null)
			{
				StringBuilder stringBuilder = new StringBuilder(lhs.Length * count);
				for (int i = 0; i < count; i++)
				{
					stringBuilder.Append(lhs);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		public static bool op_NotMember(string lhs, string rhs)
		{
			return !op_Member(lhs, rhs);
		}

		public static bool op_Member(string lhs, string rhs)
		{
			if (lhs == null || rhs == null)
			{
				return false;
			}
			return rhs.IndexOf(lhs) > -1;
		}

		public static bool op_Member(char lhs, string rhs)
		{
			if (rhs == null)
			{
				return false;
			}
			return rhs.IndexOf(lhs) > -1;
		}

		public static bool op_Match(string input, Regex pattern)
		{
			return pattern.IsMatch(input);
		}

		public static bool op_Match(string input, string pattern)
		{
			return Regex.IsMatch(input, pattern);
		}

		public static bool op_NotMatch(string input, Regex pattern)
		{
			return !op_Match(input, pattern);
		}

		public static bool op_NotMatch(string input, string pattern)
		{
			return !op_Match(input, pattern);
		}

		public static string op_Modulus(string lhs, IEnumerable rhs)
		{
			return string.Format(lhs, Builtins.array(rhs));
		}

		public static string op_Modulus(string lhs, object[] rhs)
		{
			return string.Format(lhs, rhs);
		}

		public static bool op_Member(object lhs, IList rhs)
		{
			if (rhs == null)
			{
				return false;
			}
			return rhs.Contains(lhs);
		}

		public static bool op_NotMember(object lhs, IList rhs)
		{
			return !op_Member(lhs, rhs);
		}

		public static bool op_Member(object lhs, IDictionary rhs)
		{
			if (rhs == null)
			{
				return false;
			}
			return rhs.Contains(lhs);
		}

		public static bool op_NotMember(object lhs, IDictionary rhs)
		{
			return !op_Member(lhs, rhs);
		}

		public static bool op_Member(object lhs, IEnumerable rhs)
		{
			if (rhs == null)
			{
				return false;
			}
			foreach (object rh in rhs)
			{
				if (EqualityOperator(lhs, rh))
				{
					return true;
				}
			}
			return false;
		}

		public static bool op_NotMember(object lhs, IEnumerable rhs)
		{
			return !op_Member(lhs, rhs);
		}

		public static bool EqualityOperator(object lhs, object rhs)
		{
			if (lhs == rhs)
			{
				return true;
			}
			if (lhs == null)
			{
				return rhs.Equals(lhs);
			}
			if (rhs == null)
			{
				return lhs.Equals(rhs);
			}
			TypeCode typeCode = Type.GetTypeCode(lhs.GetType());
			TypeCode typeCode2 = Type.GetTypeCode(rhs.GetType());
			if (IsNumeric(typeCode) && IsNumeric(typeCode2))
			{
				return EqualityOperator(lhs, typeCode, rhs, typeCode2);
			}
			Array array = lhs as Array;
			if (array != null)
			{
				Array array2 = rhs as Array;
				if (array2 != null)
				{
					return ArrayEqualityImpl(array, array2);
				}
			}
			return lhs.Equals(rhs) || rhs.Equals(lhs);
		}

		public static bool op_Equality(Array lhs, Array rhs)
		{
			if (lhs == rhs)
			{
				return true;
			}
			if (lhs == null || rhs == null)
			{
				return false;
			}
			return ArrayEqualityImpl(lhs, rhs);
		}

		private static bool ArrayEqualityImpl(Array lhs, Array rhs)
		{
			if (lhs.Rank != 1 || rhs.Rank != 1)
			{
				throw new ArgumentException("array rank must be 1");
			}
			if (lhs.Length != rhs.Length)
			{
				return false;
			}
			for (int i = 0; i < lhs.Length; i++)
			{
				if (!EqualityOperator(lhs.GetValue(i), rhs.GetValue(i)))
				{
					return false;
				}
			}
			return true;
		}

		private static TypeCode GetConvertTypeCode(TypeCode lhsTypeCode, TypeCode rhsTypeCode)
		{
			if (lhsTypeCode == TypeCode.Decimal || rhsTypeCode == TypeCode.Decimal)
			{
				return TypeCode.Decimal;
			}
			if (lhsTypeCode == TypeCode.Double || rhsTypeCode == TypeCode.Double)
			{
				return TypeCode.Double;
			}
			if (lhsTypeCode == TypeCode.Single || rhsTypeCode == TypeCode.Single)
			{
				return TypeCode.Single;
			}
			if (lhsTypeCode == TypeCode.UInt64)
			{
				if (rhsTypeCode == TypeCode.SByte || rhsTypeCode == TypeCode.Int16 || rhsTypeCode == TypeCode.Int32 || rhsTypeCode == TypeCode.Int64)
				{
					return TypeCode.Int64;
				}
				return TypeCode.UInt64;
			}
			if (rhsTypeCode == TypeCode.UInt64)
			{
				if (lhsTypeCode == TypeCode.SByte || lhsTypeCode == TypeCode.Int16 || lhsTypeCode == TypeCode.Int32 || lhsTypeCode == TypeCode.Int64)
				{
					return TypeCode.Int64;
				}
				return TypeCode.UInt64;
			}
			if (lhsTypeCode == TypeCode.Int64 || rhsTypeCode == TypeCode.Int64)
			{
				return TypeCode.Int64;
			}
			if (lhsTypeCode == TypeCode.UInt32)
			{
				if (rhsTypeCode == TypeCode.SByte || rhsTypeCode == TypeCode.Int16 || rhsTypeCode == TypeCode.Int32)
				{
					return TypeCode.Int64;
				}
				return TypeCode.UInt32;
			}
			if (rhsTypeCode == TypeCode.UInt32)
			{
				if (lhsTypeCode == TypeCode.SByte || lhsTypeCode == TypeCode.Int16 || lhsTypeCode == TypeCode.Int32)
				{
					return TypeCode.Int64;
				}
				return TypeCode.UInt32;
			}
			return TypeCode.Int32;
		}

		private static object op_Multiply(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) * convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) * convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) * convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) * convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) * convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) * convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) * convertible2.ToInt32(null);
			}
		}

		private static object op_Division(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) / convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) / convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) / convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) / convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) / convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) / convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) / convertible2.ToInt32(null);
			}
		}

		private static object op_Addition(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) + convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) + convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) + convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) + convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) + convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) + convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) + convertible2.ToInt32(null);
			}
		}

		private static object op_Subtraction(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) - convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) - convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) - convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) - convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) - convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) - convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) - convertible2.ToInt32(null);
			}
		}

		private static bool EqualityOperator(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) == convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) == convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) == convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) == convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) == convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) == convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) == convertible2.ToInt32(null);
			}
		}

		private static bool op_GreaterThan(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) > convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) > convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) > convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) > convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) > convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) > convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) > convertible2.ToInt32(null);
			}
		}

		private static bool op_GreaterThanOrEqual(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) >= convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) >= convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) >= convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) >= convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) >= convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) >= convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) >= convertible2.ToInt32(null);
			}
		}

		private static bool op_LessThan(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) < convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) < convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) < convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) < convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) < convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) < convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) < convertible2.ToInt32(null);
			}
		}

		private static bool op_LessThanOrEqual(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) <= convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) <= convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) <= convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) <= convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) <= convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) <= convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) <= convertible2.ToInt32(null);
			}
		}

		private static object op_Modulus(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Decimal:
				return convertible.ToDecimal(null) % convertible2.ToDecimal(null);
			case TypeCode.Double:
				return convertible.ToDouble(null) % convertible2.ToDouble(null);
			case TypeCode.Single:
				return convertible.ToSingle(null) % convertible2.ToSingle(null);
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) % convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) % convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) % convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) % convertible2.ToInt32(null);
			}
		}

		private static double op_Exponentiation(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			return Math.Pow(convertible.ToDouble(null), convertible2.ToDouble(null));
		}

		private static object op_BitwiseAnd(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				throw new ArgumentException(string.Concat(lhsTypeCode, " & ", rhsTypeCode));
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) & convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) & convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) & convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) & convertible2.ToInt32(null);
			}
		}

		private static object op_BitwiseOr(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				throw new ArgumentException(string.Concat(lhsTypeCode, " | ", rhsTypeCode));
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) | convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) | convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) | convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) | convertible2.ToInt32(null);
			}
		}

		private static object op_ExclusiveOr(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (GetConvertTypeCode(lhsTypeCode, rhsTypeCode))
			{
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				throw new ArgumentException(string.Concat(lhsTypeCode, " ^ ", rhsTypeCode));
			case TypeCode.UInt64:
				return convertible.ToUInt64(null) ^ convertible2.ToUInt64(null);
			case TypeCode.Int64:
				return convertible.ToInt64(null) ^ convertible2.ToInt64(null);
			case TypeCode.UInt32:
				return convertible.ToUInt32(null) ^ convertible2.ToUInt32(null);
			default:
				return convertible.ToInt32(null) ^ convertible2.ToInt32(null);
			}
		}

		private static object op_ShiftLeft(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (rhsTypeCode)
			{
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				throw new ArgumentException(string.Concat(lhsTypeCode, " << ", rhsTypeCode));
			default:
				switch (lhsTypeCode)
				{
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					throw new ArgumentException(string.Concat(lhsTypeCode, " << ", rhsTypeCode));
				case TypeCode.UInt64:
					return convertible.ToUInt64(null) << convertible2.ToInt32(null);
				case TypeCode.Int64:
					return convertible.ToInt64(null) << convertible2.ToInt32(null);
				case TypeCode.UInt32:
					return convertible.ToUInt32(null) << convertible2.ToInt32(null);
				default:
					return convertible.ToInt32(null) << convertible2.ToInt32(null);
				}
			}
		}

		private static object op_ShiftRight(object lhs, TypeCode lhsTypeCode, object rhs, TypeCode rhsTypeCode)
		{
			IConvertible convertible = (IConvertible)lhs;
			IConvertible convertible2 = (IConvertible)rhs;
			switch (rhsTypeCode)
			{
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				throw new ArgumentException(string.Concat(lhsTypeCode, " >> ", rhsTypeCode));
			default:
				switch (lhsTypeCode)
				{
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					throw new ArgumentException(string.Concat(lhsTypeCode, " >> ", rhsTypeCode));
				case TypeCode.UInt64:
					return convertible.ToUInt64(null) >> convertible2.ToInt32(null);
				case TypeCode.Int64:
					return convertible.ToInt64(null) >> convertible2.ToInt32(null);
				case TypeCode.UInt32:
					return convertible.ToUInt32(null) >> convertible2.ToInt32(null);
				default:
					return convertible.ToInt32(null) >> convertible2.ToInt32(null);
				}
			}
		}

		private static object op_UnaryNegation(object operand, TypeCode operandTypeCode)
		{
			IConvertible convertible = (IConvertible)operand;
			switch (operandTypeCode)
			{
			case TypeCode.Decimal:
				return -convertible.ToDecimal(null);
			case TypeCode.Double:
				return 0.0 - convertible.ToDouble(null);
			case TypeCode.Single:
				return 0f - convertible.ToSingle(null);
			case TypeCode.UInt64:
				return -convertible.ToInt64(null);
			case TypeCode.Int64:
				return -convertible.ToInt64(null);
			case TypeCode.UInt32:
				return -convertible.ToInt64(null);
			default:
				return -convertible.ToInt32(null);
			}
		}

		internal static bool IsPromotableNumeric(TypeCode code)
		{
			switch (code)
			{
			case TypeCode.Byte:
				return true;
			case TypeCode.SByte:
				return true;
			case TypeCode.Int16:
				return true;
			case TypeCode.Int32:
				return true;
			case TypeCode.Int64:
				return true;
			case TypeCode.UInt16:
				return true;
			case TypeCode.UInt32:
				return true;
			case TypeCode.UInt64:
				return true;
			case TypeCode.Single:
				return true;
			case TypeCode.Double:
				return true;
			case TypeCode.Boolean:
				return true;
			case TypeCode.Decimal:
				return true;
			case TypeCode.Char:
				return true;
			default:
				return false;
			}
		}

		public static IConvertible CheckNumericPromotion(object value)
		{
			IConvertible convertible = (IConvertible)value;
			return CheckNumericPromotion(convertible);
		}

		public static IConvertible CheckNumericPromotion(IConvertible convertible)
		{
			if (IsPromotableNumeric(convertible.GetTypeCode()))
			{
				return convertible;
			}
			throw new InvalidCastException();
		}

		public static byte UnboxByte(object value)
		{
			if (value is byte)
			{
				return (byte)value;
			}
			return CheckNumericPromotion(value).ToByte(null);
		}

		public static sbyte UnboxSByte(object value)
		{
			if (value is sbyte)
			{
				return (sbyte)value;
			}
			return CheckNumericPromotion(value).ToSByte(null);
		}

		public static char UnboxChar(object value)
		{
			if (value is char)
			{
				return (char)value;
			}
			return CheckNumericPromotion(value).ToChar(null);
		}

		public static short UnboxInt16(object value)
		{
			if (value is short)
			{
				return (short)value;
			}
			return CheckNumericPromotion(value).ToInt16(null);
		}

		public static ushort UnboxUInt16(object value)
		{
			if (value is ushort)
			{
				return (ushort)value;
			}
			return CheckNumericPromotion(value).ToUInt16(null);
		}

		public static int UnboxInt32(object value)
		{
			if (value is int)
			{
				return (int)value;
			}
			return CheckNumericPromotion(value).ToInt32(null);
		}

		public static uint UnboxUInt32(object value)
		{
			if (value is uint)
			{
				return (uint)value;
			}
			return CheckNumericPromotion(value).ToUInt32(null);
		}

		public static long UnboxInt64(object value)
		{
			if (value is long)
			{
				return (long)value;
			}
			return CheckNumericPromotion(value).ToInt64(null);
		}

		public static ulong UnboxUInt64(object value)
		{
			if (value is ulong)
			{
				return (ulong)value;
			}
			return CheckNumericPromotion(value).ToUInt64(null);
		}

		public static float UnboxSingle(object value)
		{
			if (value is float)
			{
				return (float)value;
			}
			return CheckNumericPromotion(value).ToSingle(null);
		}

		public static double UnboxDouble(object value)
		{
			if (value is double)
			{
				return (double)value;
			}
			return CheckNumericPromotion(value).ToDouble(null);
		}

		public static decimal UnboxDecimal(object value)
		{
			if (value is decimal)
			{
				return (decimal)value;
			}
			return CheckNumericPromotion(value).ToDecimal(null);
		}

		public static bool UnboxBoolean(object value)
		{
			if (value is bool)
			{
				return (bool)value;
			}
			return CheckNumericPromotion(value).ToBoolean(null);
		}

		public static bool ToBool(object value)
		{
			if (value == null)
			{
				return false;
			}
			if (value is bool)
			{
				return (bool)value;
			}
			if (value is string)
			{
				return !string.IsNullOrEmpty((string)value);
			}
			Type type = value.GetType();
			Dispatcher dispatcher = GetDispatcher(value, "$ToBool$", new Type[1] { type }, () => CreateBoolConverter(type));
			return (bool)dispatcher(value, new object[1] { value });
		}

		public static bool ToBool(decimal value)
		{
			return 0m != value;
		}

		public static bool ToBool(float value)
		{
			return 0f != value;
		}

		public static bool ToBool(double value)
		{
			return 0.0 != value;
		}

		private static object ToBoolTrue(object value, object[] arguments)
		{
			return True;
		}

		private static object UnboxBooleanDispatcher(object value, object[] arguments)
		{
			return UnboxBoolean(value);
		}

		private static Dispatcher CreateBoolConverter(Type type)
		{
			MethodInfo methodInfo = FindImplicitConversionOperator(type, typeof(bool));
			if (methodInfo != null)
			{
				return EmitImplicitConversionDispatcher(methodInfo);
			}
			if (type.IsValueType)
			{
				return UnboxBooleanDispatcher;
			}
			return ToBoolTrue;
		}

		internal static MethodInfo FindImplicitConversionOperator(Type from, Type to)
		{
			return FindImplicitConversionMethod(from.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy), from, to) ?? FindImplicitConversionMethod(to.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy), from, to) ?? FindImplicitConversionMethod(GetExtensionMethods(), from, to);
		}

		private static IEnumerable<MethodInfo> GetExtensionMethods()
		{
			foreach (MemberInfo member in _extensions.Extensions)
			{
				if (member.MemberType == MemberTypes.Method)
				{
					yield return (MethodInfo)member;
				}
			}
		}

		private static MethodInfo FindImplicitConversionMethod(IEnumerable<MethodInfo> candidates, Type from, Type to)
		{
			foreach (MethodInfo candidate in candidates)
			{
				if (!(candidate.Name != "op_Implicit") && candidate.ReturnType == to)
				{
					ParameterInfo[] parameters = candidate.GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(from))
					{
						return candidate;
					}
				}
			}
			return null;
		}

		private static void Error(string format, params object[] args)
		{
			throw new ApplicationException(string.Format(format, args));
		}
	}
}
