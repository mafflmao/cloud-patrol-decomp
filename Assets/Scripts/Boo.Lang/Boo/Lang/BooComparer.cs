using System;
using System.Collections;

namespace Boo.Lang
{
	[Serializable]
	public class BooComparer : IComparer
	{
		public static readonly IComparer Default = new BooComparer();

		private BooComparer()
		{
		}

		public int Compare(object lhs, object rhs)
		{
			if (lhs == null)
			{
				return (rhs != null) ? (-1) : 0;
			}
			if (rhs == null)
			{
				return 1;
			}
			IComparable comparable = lhs as IComparable;
			if (comparable == null)
			{
				IComparable comparable2 = rhs as IComparable;
				if (comparable2 == null)
				{
					IEnumerable enumerable = lhs as IEnumerable;
					IEnumerable enumerable2 = rhs as IEnumerable;
					if (enumerable != null && enumerable2 != null)
					{
						return CompareEnumerables(enumerable, enumerable2);
					}
					return (!lhs.Equals(rhs)) ? 1 : 0;
				}
				return -1 * comparable2.CompareTo(lhs);
			}
			return comparable.CompareTo(rhs);
		}

		private int CompareEnumerables(IEnumerable lhs, IEnumerable rhs)
		{
			IEnumerator enumerator = lhs.GetEnumerator();
			IEnumerator enumerator2 = rhs.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!enumerator2.MoveNext())
				{
					return 1;
				}
				int num = Compare(enumerator.Current, enumerator2.Current);
				if (num == 0)
				{
					continue;
				}
				return num;
			}
			if (enumerator2.MoveNext())
			{
				return -1;
			}
			return 0;
		}
	}
}
