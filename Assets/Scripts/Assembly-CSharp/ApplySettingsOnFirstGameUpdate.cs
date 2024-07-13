using UnityEngine;

public class ApplySettingsOnFirstGameUpdate : MonoBehaviour
{
	public string levelOverride;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (LevelManager.Instance != null)
		{
			LevelManager.Instance.levelOverride = levelOverride;
			Object.Destroy(base.gameObject);
		}
	}
}
