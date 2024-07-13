using UnityEngine;

public class Killplane : MonoBehaviour
{
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == Layers.Enemies || other.gameObject.layer == Layers.EnemiesDontTarget)
		{
			Object.Destroy(other.gameObject);
		}
	}
}
