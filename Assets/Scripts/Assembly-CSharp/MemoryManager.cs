using UnityEngine;

public class MemoryManager : SingletonMonoBehaviour
{
	private ILogger _log = LogBuilder.Instance.GetLogger(typeof(MemoryManager), LogLevel.Log);

	protected override void AwakeOnce()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void HandleMemoryWarning()
	{
		_log.LogWarning("LOW MEMORY WARNING RECEIVED!");
		Resources.UnloadUnusedAssets();
	}
}
