using System;
using System.Collections;
using System.Collections.Generic;

namespace Boo.Lang.Environments
{
	public class DeferredEnvironment : IEnumerable, IEnvironment, IEnumerable<KeyValuePair<Type, ObjectFactory>>
	{
		private readonly List<KeyValuePair<Type, ObjectFactory>> _bindings = new List<KeyValuePair<Type, ObjectFactory>>();

		IEnumerator<KeyValuePair<Type, ObjectFactory>> IEnumerable<KeyValuePair<Type, ObjectFactory>>.GetEnumerator()
		{
			return _bindings.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _bindings.GetEnumerator();
		}

		TNeed IEnvironment.Provide<TNeed>()
		{
			foreach (KeyValuePair<Type, ObjectFactory> binding in _bindings)
			{
				if (typeof(TNeed).IsAssignableFrom(binding.Key))
				{
					return (TNeed)binding.Value();
				}
			}
			return (TNeed)null;
		}

		public void Add(Type need, ObjectFactory binder)
		{
			_bindings.Add(new KeyValuePair<Type, ObjectFactory>(need, binder));
		}
	}
}
