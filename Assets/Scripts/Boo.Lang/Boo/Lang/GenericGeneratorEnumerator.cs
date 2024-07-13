using System;
using System.Collections;
using System.Collections.Generic;

namespace Boo.Lang
{
	public abstract class GenericGeneratorEnumerator<T> : IEnumerator, IDisposable, IEnumerator<T>
	{
		protected T _current;

		public int _state;

		object IEnumerator.Current
		{
			get
			{
				return _current;
			}
		}

		public T Current
		{
			get
			{
				return _current;
			}
		}

		public GenericGeneratorEnumerator()
		{
			_state = 0;
		}

		public virtual void Dispose()
		{
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public abstract bool MoveNext();

		protected bool Yield(int state, T value)
		{
			_state = state;
			_current = value;
			return true;
		}

		protected bool YieldDefault(int state)
		{
			_state = state;
			_current = default(T);
			return true;
		}
	}
}
