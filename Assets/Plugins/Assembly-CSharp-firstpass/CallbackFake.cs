public class CallbackFake
{
	public Callback m_Callback;

	public CallbackFake(Callback i_Callback)
	{
		m_Callback = i_Callback;
	}
}
public class CallbackFake<T>
{
	public Callback<T> m_Callback;

	public CallbackFake(Callback<T> i_Callback)
	{
		m_Callback = i_Callback;
	}
}
