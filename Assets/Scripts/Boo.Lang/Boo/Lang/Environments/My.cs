using System;

namespace Boo.Lang.Environments
{
	public static class My<TNeed> where TNeed : class
	{
		public static TNeed Instance
		{
			get
			{
				IEnvironment instance = ActiveEnvironment.Instance;
				if (instance == null)
				{
					throw new InvalidOperationException("Environment is not available!");
				}
				TNeed val = instance.Provide<TNeed>();
				if (val == null)
				{
					throw new InvalidOperationException(string.Format("Environment could not provide '{0}'.", typeof(TNeed)));
				}
				return val;
			}
		}
	}
}
