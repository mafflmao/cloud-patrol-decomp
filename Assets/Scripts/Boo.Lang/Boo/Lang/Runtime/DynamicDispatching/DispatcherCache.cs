using System.Collections.Generic;

namespace Boo.Lang.Runtime.DynamicDispatching
{
	public class DispatcherCache
	{
		public delegate Dispatcher DispatcherFactory();

		private static Dictionary<DispatcherKey, Dispatcher> _cache = new Dictionary<DispatcherKey, Dispatcher>(DispatcherKey.EqualityComparer);

		public Dispatcher Get(DispatcherKey key, DispatcherFactory factory)
		{
			Dispatcher value;
			if (!_cache.TryGetValue(key, out value))
			{
				lock (_cache)
				{
					if (!_cache.TryGetValue(key, out value))
					{
						value = factory();
						_cache.Add(key, value);
					}
				}
			}
			return value;
		}

		public void Clear()
		{
			lock (_cache)
			{
				_cache.Clear();
			}
		}
	}
}
