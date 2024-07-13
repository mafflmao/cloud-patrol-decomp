using System;
using UnityEngine;

public class HazardBombProxy : Hazard
{
	public GameObject objectToDestroy;

	public Vector3 rotationTweak = Vector3.zero;

	[HideInInspector]
	public bool detachShield;

	[HideInInspector]
	public GameObject spawnedExplosion;

	public static event EventHandler<EventArgs> HazardProxyTriggered;

	public override void ApplyDamage()
	{
		if (_isActive && _canDamage)
		{
			OnHazardProxyTriggered();
			_canDamage = false;
			if (detachShield)
			{
				SendMessage("Disable", SendMessageOptions.DontRequireReceiver);
			}
			if (explodeFX != null)
			{
				spawnedExplosion = UnityEngine.Object.Instantiate(explodeFX.gameObject, base.transform.position, Quaternion.identity) as GameObject;
				spawnedExplosion.GetComponent<Hazard>().originatingGameObject = base.gameObject.name;
			}
			if (objectToDestroy != null)
			{
				spawnedExplosion.transform.position = objectToDestroy.transform.position;
				spawnedExplosion.transform.rotation = objectToDestroy.transform.rotation;
				spawnedExplosion.transform.Rotate(rotationTweak);
				UnityEngine.Object.Destroy(objectToDestroy);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	protected void OnHazardProxyTriggered()
	{
		if (HazardBombProxy.HazardProxyTriggered != null)
		{
			HazardBombProxy.HazardProxyTriggered(this, new EventArgs());
		}
	}
}
