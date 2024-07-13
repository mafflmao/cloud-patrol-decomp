using UnityEngine;

public class HideAccessory : MonoBehaviour
{
	public string accessoryName;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(HideAccessory), LogLevel.Warning);

	private void Start()
	{
		Hide();
	}

	public void Hide()
	{
		Transform transform = TransformUtil.FindRecursive(base.transform, accessoryName);
		if (transform == null)
		{
			_log.LogWarning("Accessory '{0}' not found on GameObject '{1}'", accessoryName, base.gameObject.name);
		}
		else
		{
			transform.gameObject.GetComponent<Renderer>().enabled = false;
		}
	}
}
