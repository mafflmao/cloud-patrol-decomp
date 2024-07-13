using System;

namespace Boo.Lang
{
	public class DynamicVariable<T>
	{
		private T _current;

		public T Value
		{
			get
			{
				return _current;
			}
		}

		public DynamicVariable()
		{
			_current = default(T);
		}

		public DynamicVariable(T initialValue)
		{
			_current = initialValue;
		}

		[Obsolete("Use With(T, System.Action) and access the variable value directly from the closure")]
		public void With(T value, Action<T> code)
		{
			With(value, (Procedure)delegate
			{
				code(value);
			});
		}

		public void With(T value, Procedure code)
		{
			T current = _current;
			_current = value;
			try
			{
				code();
			}
			finally
			{
				_current = current;
			}
		}
	}
}
