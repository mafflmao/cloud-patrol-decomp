namespace Boo.Lang.Environments
{
	public class EnvironmentChain : IEnvironment
	{
		private readonly IEnvironment[] _chain;

		public EnvironmentChain(params IEnvironment[] chain)
		{
			_chain = chain;
		}

		public TNeed Provide<TNeed>() where TNeed : class
		{
			IEnvironment[] chain = _chain;
			foreach (IEnvironment environment in chain)
			{
				TNeed val = environment.Provide<TNeed>();
				if (val != null)
				{
					return val;
				}
			}
			return (TNeed)null;
		}
	}
}
