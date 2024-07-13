using System;

namespace Boo.Lang.Environments
{
	public static class ActiveEnvironment
	{
		[ThreadStatic]
		private static IEnvironment _instance;

		public static IEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}

		public static void With(IEnvironment environment, Procedure code)
		{
			IEnvironment instance = _instance;
			try
			{
				_instance = environment;
				code();
			}
			finally
			{
				_instance = instance;
			}
		}
	}
}
