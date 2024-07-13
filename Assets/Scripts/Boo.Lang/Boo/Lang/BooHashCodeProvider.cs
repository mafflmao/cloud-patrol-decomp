using System;
using System.Collections;
using System.Collections.Generic;

namespace Boo.Lang
{
	[Serializable]
	public class BooHashCodeProvider : IEqualityComparer, IEqualityComparer<object>
	{
		public static readonly IEqualityComparer Default = new BooHashCodeProvider();

		private BooHashCodeProvider()
		{
		}

		public int GetHashCode(object o)
		{
			if (o == null)
			{
				return 0;
			}
			Array array = o as Array;
			return (array == null) ? o.GetHashCode() : GetArrayHashCode(array);
		}

		public new bool Equals(object lhs, object rhs)
		{
			return BooComparer.Default.Compare(lhs, rhs) == 0;
		}

		public int GetArrayHashCode(Array array)
		{
			int num = 1;
			int num2 = 0;
			foreach (object item in array)
			{
				num ^= GetHashCode(item) * ++num2;
			}
			return num;
		}
	}
}
