using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SplashDamage : MonoBehaviour
{
	public float radius = 1.5f;

	public float damageDelay = 0.1f;

	public int damageAmount = 10;

	public float explosiveForce = 500f;

	public float explosionDepth = 2f;

	public static event EventHandler<EventArgs> Killed;

	private void Start()
	{
		SplashDamageUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<SplashDamageUpgrade>();
		float num = explosionDepth / 2f;
		int layerMask = 1 << Layers.Enemies;
		RaycastHit[] source = Physics.SphereCastAll(base.transform.position - Vector3.forward * num, radius, Vector3.forward, explosionDepth, layerMask);
		foreach (Collider item in source.Select((RaycastHit hit) => hit.collider))
		{
			Health component = item.GetComponent<Health>();
			if (component != null)
			{
				AddExplosiveForce(component.gameObject);
				ScoreKeeper.Instance.AddScore(component.scoreType, component.transform.position, true);
				if (passiveUpgradeOrDefault != null)
				{
					passiveUpgradeOrDefault.TrySpawnCoin(component.transform.position);
				}
				StartCoroutine(DestroyDelayed(item, component, damageDelay));
			}
		}
	}

	private void AddExplosiveForce(GameObject gameObject)
	{
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.AddExplosionForce(explosiveForce, base.transform.position, radius);
		rigidbody.isKinematic = false;
		rigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * 20f);
		rigidbody.detectCollisions = false;
		gameObject.GetComponent<Rigidbody>().angularDrag = 0f;
	}

	public IEnumerator DestroyDelayed(Collider collider, Health health, float delay)
	{
		if (collider == null || health == null)
		{
			yield break;
		}
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (health != null && !health.isDead)
		{
			health.isDeflecting = false;
			health.isForceFielded = false;
			health.Kill();
			if (SplashDamage.Killed != null)
			{
				SplashDamage.Killed(health, new EventArgs());
			}
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = Vector3.forward * (explosionDepth / 2f);
		Gizmos.DrawWireSphere(base.transform.position, radius);
		Gizmos.DrawWireSphere(base.transform.position + vector, radius);
		Gizmos.DrawWireSphere(base.transform.position - vector, radius);
		Gizmos.DrawLine(base.transform.position - vector, base.transform.position + vector);
	}
}
