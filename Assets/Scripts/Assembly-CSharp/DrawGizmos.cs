using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
	private Transform location;

	private void Update()
	{
		OnDrawGizmos();
	}

	public void OnDrawGizmos()
	{
		Color color = new Color(1f, 1f, 1f, 0.5f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 1f);
		Gizmos.color = color;
		Gizmos.DrawSphere(base.transform.position, 1.5f);
	}
}
