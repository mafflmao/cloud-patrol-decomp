using UnityEngine;

public sealed class MessengerHelper : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void OnDisable()
	{
		Messenger.Cleanup();
	}
}
