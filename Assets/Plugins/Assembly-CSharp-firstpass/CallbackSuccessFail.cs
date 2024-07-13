public class CallbackSuccessFail<T, U>
{
	public Callback<T> m_CallbackSuccess;

	public Callback<U> m_CallbackFail;

	public CallbackSuccessFail(Callback<T> i_Success, Callback<U> i_Fail)
	{
		m_CallbackSuccess = i_Success;
		m_CallbackFail = i_Fail;
	}
}
