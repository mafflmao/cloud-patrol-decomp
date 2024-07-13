using System;
using System.Collections.Generic;

namespace Boo.Lang.Environments
{
	public class CachingEnvironment : IEnvironment
	{
		private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

		private readonly IEnvironment _source;

		public event Action<object> InstanceCached;

		public CachingEnvironment(IEnvironment source)
		{
			_source = source;
		}

		public TNeed Provide<TNeed>() where TNeed : class
		{
			object value;
			if (_cache.TryGetValue(typeof(TNeed), out value))
			{
				return (TNeed)value;
			}
			foreach (object value2 in _cache.Values)
			{
				if (value2 is TNeed)
				{
					_cache.Add(typeof(TNeed), value2);
					return (TNeed)value2;
				}
			}
			TNeed val = _source.Provide<TNeed>();
			if (val != null)
			{
				Add(typeof(TNeed), val);
			}
			return val;
		}

		public void Add(Type type, object instance)
		{
			if (!type.IsInstanceOfType(instance))
			{
				throw new ArgumentException(string.Format("{0} is not an instance of {1}", instance, type));
			}
			_cache.Add(type, instance);
			if (this.InstanceCached != null)
			{
				this.InstanceCached(instance);
			}
		}
	}
}
