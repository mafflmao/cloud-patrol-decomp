using UnityEngine;

public class BackgroundRootNodeData : MonoBehaviour
{
	private void Start()
	{
		LevelManager.Instance.AddBackground(this);
	}

	private void OnDrawGizmos()
	{
		Vector3 center = base.transform.position + new Vector3(0f, 0f, 6f);
		Gizmos.DrawWireCube(center, new Vector3(60f, 40f, 12f));
	}
}
