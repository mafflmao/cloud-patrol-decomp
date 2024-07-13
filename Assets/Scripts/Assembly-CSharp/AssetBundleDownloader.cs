using System.Collections;
using UnityEngine;

public class AssetBundleDownloader : SingletonMonoBehaviour
{
	public string overrideUrl;

	private AssetBundle _assetBundle;

	private AssetBundleManifest _manifest;

	public static AssetBundleDownloader Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<AssetBundleDownloader>();
		}
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		SwrveSaleAssetBundle.Instance.UpdateFromSwrve();
	}

	private void DownloadAssetBundleCoroutine()
	{
	}

	public void LoadAssetFromBundle()
	{
	}

	public void UnloadAssetBundleAssets(bool unloadAllLoadedObjects)
	{
		if (_assetBundle == null)
		{
			Debug.LogError("AssetBundle not loaded.");
		}
		else
		{
			_assetBundle.Unload(unloadAllLoadedObjects);
		}
	}
}
