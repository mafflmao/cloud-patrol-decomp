using UnityEngine;

public class ScreenRootNodeData : MonoBehaviour
{
	private void Awake()
	{
		if (LevelManager.Instance == null)
		{
			Application.LoadLevel("MainScene");
			GameObject gameObject = new GameObject("OverrideSetter");
			ApplySettingsOnFirstGameUpdate applySettingsOnFirstGameUpdate = gameObject.AddComponent<ApplySettingsOnFirstGameUpdate>();
			applySettingsOnFirstGameUpdate.levelOverride = Application.loadedLevelName;
		}
	}

	private void Start()
	{
		if (LevelManager.Instance != null)
		{
			LevelManager.Instance.Add(this);
		}
		else
		{
			Debug.LogError("Couldn't find LevelManager.");
		}
	}
}
