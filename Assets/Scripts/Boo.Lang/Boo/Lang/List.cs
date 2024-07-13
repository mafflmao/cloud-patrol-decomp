using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Runtime;

namespace Boo.Lang
{
	[Serializable]
	public class List<T> : IEnumerable, ICollection, IEnumerable<T>, IList<T>, ICollection<T>, IEquatable<List<T>>, IList
	{
		private sealed class ComparisonComparer : IComparer<T>
		{
			private readonly Comparison<T> _comparison;

			public ComparisonComparer(Comparison<T> comparison)
			{
				_comparison = comparison;
			}

			public int Compare(T x, T y)
			{
				return _comparison(x, y);
			}
		}

		private static readonly T[] EmptyArray = new T[0];

		protected T[] _items;

		protected int _count;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = Coerce(value);
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public IEnumerable<T> Reversed
		{
			get
			{
				for (int i = _count - 1; i >= 0; i--)
				{
					yield return _items[i];
				}
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return _items;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public T this[int index]
		{
			get
			{
				return _items[CheckIndex(NormalizeIndex(index))];
			}
			set
			{
				_items[CheckIndex(NormalizeIndex(index))] = value;
			}
		}

		public List()
		{
			_items = EmptyArray;
		}

		public List(IEnumerable enumerable)
			: this()
		{
			Extend(enumerable);
		}

		public List(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity");
			}
			_items = new T[initialCapacity];
			_count = 0;
		}

		public List(T[] items, bool takeOwnership)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			_items = ((!takeOwnership) ? ((T[])items.Clone()) : items);
			_count = items.Length;
		}

		void ICollection<T>.Add(T item)
		{
			Push(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		void IList<T>.Insert(int index, T item)
		{
			Insert(index, item);
		}

		void IList<T>.RemoveAt(int index)
		{
			InnerRemoveAt(CheckIndex(NormalizeIndex(index)));
		}

		bool ICollection<T>.Remove(T item)
		{
			return InnerRemove(item);
		}

		int IList.Add(object value)
		{
			Add((T)value);
			return Count - 1;
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, Coerce(value));
		}

		void IList.Remove(object value)
		{
			Remove(Coerce(value));
		}

		int IList.IndexOf(object value)
		{
			return IndexOf(Coerce(value));
		}

		bool IList.Contains(object value)
		{
			return Contains(Coerce(value));
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			Array.Copy(_items, 0, array, index, _count);
		}

		public List<T> Multiply(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			T[] array = new T[_count * count];
			for (int i = 0; i < count; i++)
			{
				Array.Copy(_items, 0, array, i * _count, _count);
			}
			return NewConcreteList(array, true);
		}

		protected virtual List<T> NewConcreteList(T[] items, bool takeOwnership)
		{
			return new List<T>(items, takeOwnership);
		}

		public IEnumerator<T> GetEnumerator()
		{
			int originalCount = _count;
			T[] originalItems = _items;
			for (int i = 0; i < _count; i++)
			{
				if (originalCount != _count || originalItems != _items)
				{
					throw new InvalidOperationException("The list was modified.");
				}
				yield return _items[i];
			}
		}

		public void CopyTo(T[] target, int index)
		{
			Array.Copy(_items, 0, target, index, _count);
		}

		public T FastAt(int normalizedIndex)
		{
			return _items[normalizedIndex];
		}

		public List<T> Push(T item)
		{
			return Add(item);
		}

		public virtual List<T> Add(T item)
		{
			EnsureCapacity(_count + 1);
			_items[_count] = item;
			_count++;
			return this;
		}

		public List<T> AddUnique(T item)
		{
			if (!Contains(item))
			{
				Add(item);
			}
			return this;
		}

		public List<T> Extend(IEnumerable enumerable)
		{
			AddRange(enumerable);
			return this;
		}

		public void AddRange(IEnumerable enumerable)
		{
			foreach (T item in enumerable)
			{
				Add(item);
			}
		}

		public List<T> ExtendUnique(IEnumerable enumerable)
		{
			foreach (T item in enumerable)
			{
				AddUnique(item);
			}
			return this;
		}

		public List<T> Collect(Predicate<T> condition)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			List<T> list = NewConcreteList(new T[0], true);
			InnerCollect(list, condition);
			return list;
		}

		public List<T> Collect(List<T> target, Predicate<T> condition)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			InnerCollect(target, condition);
			return target;
		}

		public T[] ToArray()
		{
			if (_count == 0)
			{
				return EmptyArray;
			}
			T[] array = new T[_count];
			CopyTo(array, 0);
			return array;
		}

		public T[] ToArray(T[] array)
		{
			CopyTo(array, 0);
			return array;
		}

		public TOut[] ToArray<TOut>(Function<T, TOut> selector)
		{
			TOut[] array = new TOut[_count];
			for (int i = 0; i < _count; i++)
			{
				array[i] = selector(_items[i]);
			}
			return array;
		}

		public List<T> Sort()
		{
			Array.Sort(_items, 0, _count, BooComparer.Default);
			return this;
		}

		public List<T> Sort(IComparer comparer)
		{
			Array.Sort(_items, 0, _count, comparer);
			return this;
		}

		public List<T> Sort(Comparison<T> comparison)
		{
			return Sort(new ComparisonComparer(comparison));
		}

		public List<T> Sort(IComparer<T> comparer)
		{
			Array.Sort(_items, 0, _count, comparer);
			return this;
		}

		public List<T> Sort(Comparer<T> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			Array.Sort(_items, 0, _count, comparer);
			return this;
		}

		public override string ToString()
		{
			return "[" + Join(", ") + "]";
		}

		public string Join(string separator)
		{
			return Builtins.join(this, separator);
		}

		public override int GetHashCode()
		{
			int num = _count;
			for (int i = 0; i < _count; i++)
			{
				T val = _items[i];
				if (val != null)
				{
					num ^= val.GetHashCode();
				}
			}
			return num;
		}

		public override bool Equals(object other)
		{
			return this == other || Equals(other as List<T>);
		}

		public bool Equals(List<T> other)
		{
			if (other == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (_count != other.Count)
			{
				return false;
			}
			for (int i = 0; i < _count; i++)
			{
				if (!RuntimeServices.EqualityOperator(_items[i], other[i]))
				{
					return false;
				}
			}
			return true;
		}

		public void Clear()
		{
			for (int i = 0; i < _count; i++)
			{
				_items[i] = default(T);
			}
			_count = 0;
		}

		public List<T> GetRange(int begin)
		{
			return InnerGetRange(AdjustIndex(NormalizeIndex(begin)), _count);
		}

		public List<T> GetRange(int begin, int end)
		{
			return InnerGetRange(AdjustIndex(NormalizeIndex(begin)), AdjustIndex(NormalizeIndex(end)));
		}

		public bool Contains(T item)
		{
			return -1 != IndexOf(item);
		}

		public bool Contains(Predicate<T> condition)
		{
			return -1 != IndexOf(condition);
		}

		public bool Find(Predicate<T> condition, out T found)
		{
			int num = IndexOf(condition);
			if (num != -1)
			{
				found = _items[num];
				return true;
			}
			found = default(T);
			return false;
		}

		public List<T> FindAll(Predicate<T> condition)
		{
			List<T> list = NewConcreteList(new T[0], true);
			using (IEnumerator<T> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (condition(current))
					{
						list.Add(current);
					}
				}
				return list;
			}
		}

		public int IndexOf(Predicate<T> condition)
		{
			if (condition == null)
			{
				throw new ArgumentNullException("condition");
			}
			for (int i = 0; i < _count; i++)
			{
				if (condition(_items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < _count; i++)
			{
				if (RuntimeServices.EqualityOperator(_items[i], item))
				{
					return i;
				}
			}
			return -1;
		}

		public List<T> Insert(int index, T item)
		{
			int num = NormalizeIndex(index);
			EnsureCapacity(Math.Max(_count, num) + 1);
			if (num < _count)
			{
				Array.Copy(_items, num, _items, num + 1, _count - num);
			}
			_items[num] = item;
			_count++;
			return this;
		}

		public T Pop()
		{
			return Pop(-1);
		}

		public T Pop(int index)
		{
			int num = CheckIndex(NormalizeIndex(index));
			T result = _items[num];
			InnerRemoveAt(num);
			return result;
		}

		public List<T> PopRange(int begin)
		{
			int num = AdjustIndex(NormalizeIndex(begin));
			List<T> result = InnerGetRange(num, AdjustIndex(NormalizeIndex(_count)));
			for (int i = num; i < _count; i++)
			{
				_items[i] = default(T);
			}
			_count = num;
			return result;
		}

		public List<T> RemoveAll(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			for (int i = 0; i < _count; i++)
			{
				if (match(_items[i]))
				{
					InnerRemoveAt(i--);
				}
			}
			return this;
		}

		public List<T> Remove(T item)
		{
			InnerRemove(item);
			return this;
		}

		public List<T> RemoveAt(int index)
		{
			InnerRemoveAt(CheckIndex(NormalizeIndex(index)));
			return this;
		}

		private void EnsureCapacity(int minCapacity)
		{
			if (minCapacity > _items.Length)
			{
				T[] array = NewArray(minCapacity);
				Array.Copy(_items, 0, array, 0, _count);
				_items = array;
			}
		}

		private T[] NewArray(int minCapacity)
		{
			int val = Math.Max(1, _items.Length) * 2;
			return new T[Math.Max(val, minCapacity)];
		}

		private void InnerRemoveAt(int index)
		{
			_count--;
			_items[index] = default(T);
			if (index != _count)
			{
				Array.Copy(_items, index + 1, _items, index, _count - index);
			}
		}

		private bool InnerRemove(T item)
		{
			int num = IndexOf(item);
			if (num != -1)
			{
				InnerRemoveAt(num);
				return true;
			}
			return false;
		}

		private void InnerCollect(List<T> target, Predicate<T> condition)
		{
			for (int i = 0; i < _count; i++)
			{
				T val = _items[i];
				if (condition(val))
				{
					target.Add(val);
				}
			}
		}

		private List<T> InnerGetRange(int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				T[] array = new T[num];
				Array.Copy(_items, begin, array, 0, num);
				return NewConcreteList(array, true);
			}
			return NewConcreteList(new T[0], true);
		}

		private int AdjustIndex(int index)
		{
			if (index > _count)
			{
				return _count;
			}
			if (index < 0)
			{
				return 0;
			}
			return index;
		}

		private int CheckIndex(int index)
		{
			if (index >= _count)
			{
				throw new IndexOutOfRangeException();
			}
			return index;
		}

		private int NormalizeIndex(int index)
		{
			return (index >= 0) ? index : (index + _count);
		}

		private static T Coerce(object value)
		{
			if (value is T)
			{
				return (T)value;
			}
			return (T)RuntimeServices.Coerce(value, typeof(T));
		}

		public static List<T> operator *(List<T> lhs, int count)
		{
			return lhs.Multiply(count);
		}

		public static List<T> operator *(int count, List<T> rhs)
		{
			return rhs.Multiply(count);
		}

		public static List<T> operator +(List<T> lhs, IEnumerable rhs)
		{
			List<T> list = lhs.NewConcreteList(lhs.ToArray(), true);
			list.Extend(rhs);
			return list;
		}
	}
	[Serializable]
	public class List : List<object>
	{
		public List()
		{
		}

		public List(IEnumerable enumerable)
			: base(enumerable)
		{
		}

		public List(int initialCapacity)
			: base(initialCapacity)
		{
		}

		public List(object[] items, bool takeOwnership)
			: base(items, takeOwnership)
		{
		}

		public object Find(Predicate<object> predicate)
		{
			object found;
			return (!Find(predicate, out found)) ? null : found;
		}

		protected override List<object> NewConcreteList(object[] items, bool takeOwnership)
		{
			return new List(items, takeOwnership);
		}

		public Array ToArray(Type targetType)
		{
			Array array = Array.CreateInstance(targetType, _count);
			Array.Copy(_items, 0, array, 0, _count);
			return array;
		}

		public static string operator %(string format, List rhs)
		{
			return string.Format(format, rhs.ToArray());
		}
	}
}
