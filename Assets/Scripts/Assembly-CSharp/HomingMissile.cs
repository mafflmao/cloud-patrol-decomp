using System;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
	public float speed = 1f;

	public Transform targetTransform;

	private bool fired;

	public GameObject deathFX;

	public static event EventHandler HomingMissileExploded;

	public void Fire(Transform target)
	{
		targetTransform = target;
		base.transform.LookAt(target.GetComponent<Collider>().bounds.center, Vector3.up);
		base.GetComponent<Rigidbody>().AddForce(base.transform.forward * speed);
		base.GetComponent<Collider>().enabled = false;
		InvokeHelper.InvokeSafe(ArmMissile, 0.3f, this);
	}

	private void ArmMissile()
	{
		base.GetComponent<Collider>().enabled = true;
		fired = true;
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (fired)
		{
			UnityEngine.Object.Instantiate(deathFX, base.transform.position, base.transform.rotation);
			OnHomingMissileExploded();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnHomingMissileExploded()
	{
		if (HomingMissile.HomingMissileExploded != null)
		{
			HomingMissile.HomingMissileExploded(this, new EventArgs());
		}
	}
}
