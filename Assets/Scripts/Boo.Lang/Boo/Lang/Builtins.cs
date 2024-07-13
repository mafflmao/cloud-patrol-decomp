using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Boo.Lang.Runtime;

namespace Boo.Lang
{
	public class Builtins
	{
		public class duck
		{
		}

		[EnumeratorItemType(typeof(object[]))]
		public class ZipEnumerator : IEnumerable, IEnumerator, IDisposable
		{
			private IEnumerator[] _enumerators;

			public object Current
			{
				get
				{
					object[] array = new object[_enumerators.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = _enumerators[i].Current;
					}
					return array;
				}
			}

			internal ZipEnumerator(params IEnumerator[] enumerators)
			{
				_enumerators = enumerators;
			}

			public void Dispose()
			{
				for (int i = 0; i < _enumerators.Length; i++)
				{
					IDisposable disposable = _enumerators[i] as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}

			public void Reset()
			{
				for (int i = 0; i < _enumerators.Length; i++)
				{
					_enumerators[i].Reset();
				}
			}

			public bool MoveNext()
			{
				for (int i = 0; i < _enumerators.Length; i++)
				{
					if (!_enumerators[i].MoveNext())
					{
						return false;
					}
				}
				return true;
			}

			public IEnumerator GetEnumerator()
			{
				return this;
			}
		}

		public static Version BooVersion
		{
			get
			{
				return new Version("0.9.7.0");
			}
		}

		public static void print(object o)
		{
			Console.WriteLine(o);
		}

		public static string gets()
		{
			return Console.ReadLine();
		}

		public static string prompt(string message)
		{
			Console.Write(message);
			return Console.ReadLine();
		}

		public static string join(IEnumerable enumerable, string separator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerator enumerator = enumerable.GetEnumerator();
			using (enumerator as IDisposable)
			{
				if (enumerator.MoveNext())
				{
					stringBuilder.Append(enumerator.Current);
					while (enumerator.MoveNext())
					{
						stringBuilder.Append(separator);
						stringBuilder.Append(enumerator.Current);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string join(IEnumerable enumerable, char separator)
		{
			return join(enumerable, separator.ToString());
		}

		public static string join(IEnumerable enumerable)
		{
			return join(enumerable, " ");
		}

		public static IEnumerable map(object enumerable, ICallable function)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			if (function == null)
			{
				throw new ArgumentNullException("function");
			}
			object[] args = new object[1];
			foreach (object item in iterator(enumerable))
			{
				args[0] = item;
				yield return function.Call(args);
			}
		}

		public static object[] array(IEnumerable enumerable)
		{
			return new List(enumerable).ToArray();
		}

		private static Array ArrayFromCollection(Type elementType, ICollection collection)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			Array array = Array.CreateInstance(elementType, collection.Count);
			if (RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(elementType)))
			{
				int num = 0;
				foreach (object item in collection)
				{
					object value = RuntimeServices.CheckNumericPromotion(item).ToType(elementType, null);
					array.SetValue(value, num);
					num++;
				}
			}
			else
			{
				collection.CopyTo(array, 0);
			}
			return array;
		}

		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		public static Array array(Type elementType, IEnumerable enumerable)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			ICollection collection = enumerable as ICollection;
			if (collection != null)
			{
				return ArrayFromCollection(elementType, collection);
			}
			List list = null;
			if (RuntimeServices.IsPromotableNumeric(Type.GetTypeCode(elementType)))
			{
				list = new List();
				foreach (object item2 in enumerable)
				{
					object item = RuntimeServices.CheckNumericPromotion(item2).ToType(elementType, null);
					list.Add(item);
				}
			}
			else
			{
				list = new List(enumerable);
			}
			return list.ToArray(elementType);
		}

		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		public static Array array(Type elementType, int length)
		{
			if (length < 0)
			{
				throw new ArgumentException("`length' cannot be negative", "length");
			}
			return matrix(elementType, length);
		}

		public static Array matrix(Type elementType, params int[] lengths)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (lengths == null || lengths.Length == 0)
			{
				throw new ArgumentException("A matrix must have at least one dimension", "lengths");
			}
			return Array.CreateInstance(elementType, lengths);
		}

		public static T[] array<T>(int length)
		{
			throw new NotSupportedException("Operation should have been optimized away by the compiler!");
		}

		public static T[,] matrix<T>(int length0, int length1)
		{
			throw new NotSupportedException("Operation should have been optimized away by the compiler!");
		}

		public static T[,,] matrix<T>(int length0, int length1, int length2)
		{
			throw new NotSupportedException("Operation should have been optimized away by the compiler!");
		}

		public static T[,,,] matrix<T>(int length0, int length1, int length2, int length3)
		{
			throw new NotSupportedException("Operation should have been optimized away by the compiler!");
		}

		public static IEnumerable iterator(object enumerable)
		{
			return RuntimeServices.GetEnumerable(enumerable);
		}

		public static Process shellp(string filename, string arguments)
		{
			Process process = new Process();
			process.StartInfo.Arguments = arguments;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.FileName = filename;
			process.Start();
			return process;
		}

		public static string shell(string filename, string arguments)
		{
			Process process = shellp(filename, arguments);
			string result = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			return result;
		}

		public static IEnumerable<object[]> enumerate(object enumerable)
		{
			int i = 0;
			foreach (object item in iterator(enumerable))
			{
				yield return new object[2]
				{
					i++,
					item
				};
			}
		}

		public static IEnumerable<int> range(int max)
		{
			if (max < 0)
			{
				throw new ArgumentOutOfRangeException("max < 0");
			}
			return range(0, max);
		}

		public static IEnumerable<int> range(int begin, int end)
		{
			if (begin < end)
			{
				for (int i = begin; i < end; i++)
				{
					yield return i;
				}
			}
			else if (begin > end)
			{
				for (int j = begin; j > end; j--)
				{
					yield return j;
				}
			}
		}

		public static IEnumerable<int> range(int begin, int end, int step)
		{
			if (step == 0)
			{
				throw new ArgumentOutOfRangeException("step == 0");
			}
			if (step < 0)
			{
				if (begin < end)
				{
					throw new ArgumentOutOfRangeException("begin < end && step < 0");
				}
				for (int i = begin; i > end; i += step)
				{
					yield return i;
				}
			}
			else
			{
				if (begin > end)
				{
					throw new ArgumentOutOfRangeException("begin > end && step > 0");
				}
				for (int j = begin; j < end; j += step)
				{
					yield return j;
				}
			}
		}

		public static IEnumerable reversed(object enumerable)
		{
			return new List(iterator(enumerable)).Reversed;
		}

		public static ZipEnumerator zip(params object[] enumerables)
		{
			IEnumerator[] array = new IEnumerator[enumerables.Length];
			for (int i = 0; i < enumerables.Length; i++)
			{
				array[i] = GetEnumerator(enumerables[i]);
			}
			return new ZipEnumerator(array);
		}

		public static IEnumerable<object> cat(params object[] args)
		{
			foreach (object e in args)
			{
				foreach (object item in iterator(e))
				{
					yield return item;
				}
			}
		}

		private static IEnumerator GetEnumerator(object enumerable)
		{
			return RuntimeServices.GetEnumerable(enumerable).GetEnumerator();
		}
	}
}
