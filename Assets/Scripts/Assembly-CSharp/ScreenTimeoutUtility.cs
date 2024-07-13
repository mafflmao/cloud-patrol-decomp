using UnityEngine;

public class ScreenTimeoutUtility
{
	private static ScreenTimeoutUtility _instance;

	public static ScreenTimeoutUtility Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ScreenTimeoutUtility();
			}
			return _instance;
		}
	}

	public bool AllowTimeout
	{
		set
		{
			Screen.sleepTimeout = ((!value) ? (-1) : (-2));
		}
	}

	private ScreenTimeoutUtility()
	{
	}
}
