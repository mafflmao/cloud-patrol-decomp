using UnityEngine;

public class IconPlaceholder : MonoBehaviour
{
	public int width = 64;

	public int height = 64;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnDrawGizmos()
	{
		Gizmos.DrawCube(base.transform.position, new Vector3(width, height, 0f));
	}
}
