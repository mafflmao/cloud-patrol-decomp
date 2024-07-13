using UnityEngine;

public class AssetBundleUiScreenListener : MonoBehaviour
{
	public string sceneName;

	public bool tryAddRightAway;

	public AssetBundleObjectAttachmentData[] attachmentData;

	private void OnEnable()
	{
		StateManager.StateActivated += HandleStateManagerStateActivated;
	}

	private void OnDisable()
	{
		StateManager.StateActivated -= HandleStateManagerStateActivated;
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(this);
		if (tryAddRightAway)
		{
			TryAttachResourcesToCurrentState();
		}
	}

	private void HandleStateManagerStateActivated(object sender, StateEventArgs e)
	{
		TryAttachResourcesToCurrentState();
	}

	private void TryAttachResourcesToCurrentState()
	{
		if (StateManager.Instance != null && StateManager.Instance.CurrentStateName == sceneName)
		{
			Debug.Log("AssetBundleUiScreenListener attaching to current state! - " + sceneName);
			AssetBundleObjectAttachmentData[] array = attachmentData;
			foreach (AssetBundleObjectAttachmentData data in array)
			{
				AttachInstanceOfResourceTo();
			}
		}
	}

	private void AttachInstanceOfResourceTo()
	{
	}
}
