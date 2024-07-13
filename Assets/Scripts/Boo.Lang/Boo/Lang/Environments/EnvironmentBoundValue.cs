namespace Boo.Lang.Environments
{
	public static class EnvironmentBoundValue
	{
		public static EnvironmentBoundValue<T> Capture<T>() where T : class
		{
			return Return(My<T>.Instance);
		}

		public static EnvironmentBoundValue<T> Return<T>(T value)
		{
			return Create(ActiveEnvironment.Instance, value);
		}

		public static EnvironmentBoundValue<T> Create<T>(IEnvironment environment, T value)
		{
			return new EnvironmentBoundValue<T>(environment, value);
		}
	}
	public struct EnvironmentBoundValue<T>
	{
		public readonly T Value;

		public readonly IEnvironment Environment;

		public EnvironmentBoundValue(IEnvironment environment, T value)
		{
			Environment = environment;
			Value = value;
		}

		public EnvironmentBoundValue<TResult> Select<TResult>(Function<T, TResult> selector)
		{
			T v = Value;
			EnvironmentBoundValue<TResult> r = default(EnvironmentBoundValue<TResult>);
			ActiveEnvironment.With(Environment, delegate
			{
				r = EnvironmentBoundValue.Return(selector(v));
			});
			return r;
		}
	}
}
