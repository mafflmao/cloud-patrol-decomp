using System.Collections;

namespace Boo.Lang
{
	public abstract class AbstractGeneratorEnumerator : IEnumerator
	{
		protected object _current;

		protected int _state;

		public object Current
		{
			get
			{
				return _current;
			}
		}

		public AbstractGeneratorEnumerator()
		{
		}

		public void Reset()
		{
			_state = 0;
		}

		public abstract bool MoveNext();

		protected bool Yield(int state, object value)
		{
			_state = state;
			_current = value;
			return true;
		}
	}
}
