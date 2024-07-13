namespace Boo.Lang.Environments
{
	public class ClosedEnvironment : IEnvironment
	{
		private readonly object[] _bindings;

		public ClosedEnvironment(params object[] bindings)
		{
			_bindings = bindings;
		}

		public TNeed Provide<TNeed>() where TNeed : class
		{
			object[] bindings = _bindings;
			foreach (object obj in bindings)
			{
				if (obj is TNeed)
				{
					return (TNeed)obj;
				}
			}
			return (TNeed)null;
		}
	}
}
