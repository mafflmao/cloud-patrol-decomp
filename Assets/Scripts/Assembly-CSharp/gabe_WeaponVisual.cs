using System.Collections;
using UnityEngine;

public class gabe_WeaponVisual : MonoBehaviour
{
	public Animation turret;

	public Transform lookAtTarget;

	public GameObject skylanderNoLimbs;

	public GameObject skylanderRigged;

	private float yaw;

	private Transform turretGunBarrel;

	private Transform turretBase;

	public SoundEventData turretSpawnSFX;

	public SoundEventData skylanderLandSFX;

	private GameObject idleTarget;

	[HideInInspector]
	public bool trackingActive;

	private float updateTime = 0.05f;

	private Quaternion pitchQuat;

	private Quaternion yawQuat;

	private void Start()
	{
		turretBase = TransformUtil.FindRecursive(base.transform, "Sphere01");
		turretGunBarrel = TransformUtil.FindRecursive(base.transform, "Cylinder34");
		GameObjectUtils.HideObject(skylanderNoLimbs);
		GameObjectUtils.HideObject(skylanderRigged);
		idleTarget = new GameObject();
		idleTarget.transform.parent = base.transform;
		idleTarget.transform.localPosition = new Vector3(0.2f, 0f, 1f);
		float num = Vector3.Distance(base.transform.position, idleTarget.transform.position);
		float y = idleTarget.transform.position.y;
		float x = 100f + 57.29578f * Mathf.Asin(y / num);
		pitchQuat = Quaternion.Euler(new Vector3(x, 0f, 0f));
		yaw = 57.29578f * Mathf.Atan((0f - idleTarget.transform.position.x + turretBase.position.x) / (0f - idleTarget.transform.position.z + turretBase.position.z));
		yawQuat = Quaternion.Euler(new Vector3(270f, yaw, 0f));
		trackingActive = false;
		StartCoroutine(Tracking());
	}

	public IEnumerator Tracking()
	{
		while (true)
		{
			if (trackingActive)
			{
				if (ShipManager.instance.shooter[0].targetQueue.Count > 0)
				{
					lookAtTarget = ShipManager.instance.shooter[0].targetQueue.targetQueue.Peek().transform;
					yaw = 57.29578f * Mathf.Atan((0f - lookAtTarget.position.x + turretBase.position.x) / (0f - lookAtTarget.position.z + turretBase.position.z));
					turretBase.rotation = Quaternion.Euler(new Vector3(270f, yaw, 0f));
					turretGunBarrel.LookAt(lookAtTarget.position);
				}
			}
			else
			{
				turretBase.rotation = Quaternion.RotateTowards(turretBase.rotation, yawQuat, 1f);
				turretGunBarrel.localRotation = Quaternion.RotateTowards(turretGunBarrel.localRotation, pitchQuat, 1f);
			}
			yield return new WaitForSeconds(updateTime);
		}
	}

	public IEnumerator SpawnInSequence()
	{
		yield return new WaitForSeconds(0.5f);
		GameObjectUtils.ShowObject(skylanderNoLimbs);
		skylanderNoLimbs.GetComponent<Animation>().Play("SkylanderPartialShow", PlayMode.StopAll);
	}

	public IEnumerator SpawnOutSequence()
	{
		yield return new WaitForSeconds(0.5f);
	}

	public void FireAtTargets()
	{
		trackingActive = true;
	}
}
