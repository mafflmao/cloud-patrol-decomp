using System;
using UnityEngine;

[Serializable]
public class AssetBundleObjectAttachmentData
{
	public string parentName;

	public string resourceName;

	public bool attachAtParentLocation = true;

	public bool attachWithParentOrientation = true;

	public bool attachWithParentScale = true;

	public GameObject Instance { get; set; }

	public override string ToString()
	{
		return string.Format("{0} -> {1}", (parentName != null) ? parentName : "(ROOT)", resourceName);
	}
}
