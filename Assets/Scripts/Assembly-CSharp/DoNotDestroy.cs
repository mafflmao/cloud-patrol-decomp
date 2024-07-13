using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
