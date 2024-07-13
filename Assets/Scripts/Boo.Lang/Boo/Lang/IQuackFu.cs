namespace Boo.Lang
{
	public interface IQuackFu
	{
		object QuackGet(string name, object[] parameters);

		object QuackSet(string name, object[] parameters, object value);

		object QuackInvoke(string name, params object[] args);
	}
}
