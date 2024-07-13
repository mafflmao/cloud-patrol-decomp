using UnityEngine;

public class TriggerCover : MonoBehaviour
{
	private Hazard isHazard;

	private Health myHealth;

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == Layers.EnemiesDontTarget && (bool)myHealth)
		{
			other.gameObject.layer = Layers.Enemies;
		}
		if ((bool)isHazard && other.gameObject.layer == Layers.EnemiesDontTarget)
		{
			isHazard.SetActive(true);
			other.gameObject.layer = Layers.Enemies;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		myHealth = other.gameObject.GetComponent<Health>();
		if (other.gameObject.layer == Layers.Enemies && (bool)myHealth)
		{
			ShipManager.instance.RemoveTarget(other.gameObject);
			other.gameObject.layer = Layers.EnemiesDontTarget;
		}
		isHazard = other.gameObject.GetComponent<Hazard>();
		if ((bool)isHazard && other.gameObject.layer == Layers.Enemies)
		{
			isHazard.SetActive(false);
			other.gameObject.layer = Layers.EnemiesDontTarget;
		}
	}
}
