using System.Collections;
using UnityEngine;

public class EnemySuperDodger : SafeMonoBehaviour
{
	public float timeDodging = 0.5f;

	public float disappearTime = 0.15f;

	public int ammoCount = 2;

	private WizardStunUpgrade _wizardStun;

	public Transform teleportLocation;

	public Transform dodgeVFX;

	public SoundEventData dodgeSFX;

	public SoundEventData dodgeVO;

	public SoundEventData reappearSFX;

	public SoundEventData dizzyVO;

	private Vector3 startPos;

	private float timer;

	private AnimationStates myAnim;

	private Accessory[] accessories;

	public Transform projectile;

	public Transform dizzyFX;

	private bool vulnerable;

	private bool targetted;

	private bool tracking = true;

	private GameObject target;

	private float yaw;

	private GameObject myDizzy;

	private TargetQueue mTargetQueue;

	public float DizzyTime
	{
		get
		{
			if (_wizardStun != null)
			{
				return DifficultyManager.Instance.WizardDizzyTime + _wizardStun.additonalStunDelay;
			}
			return DifficultyManager.Instance.WizardDizzyTime;
		}
	}

	private void OnEnable()
	{
		mTargetQueue = null;
		myAnim = GetComponent<AnimationStates>();
		if (myAnim != null)
		{
			myAnim.Jester_Idle();
			myAnim.Offset(Random.Range(0f, 0.5f));
		}
		startPos = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		accessories = GetComponents<Accessory>();
		tracking = true;
		target = GameObject.FindGameObjectWithTag("ProjectileTarget");
		_wizardStun = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<WizardStunUpgrade>();
	}

	private void OnDisable()
	{
		CancelInvoke();
		StopAllCoroutines();
		tracking = false;
	}

	private void Update()
	{
		if (targetted && !vulnerable)
		{
			targetted = false;
			myAnim.Jester_Disappear();
			StartCoroutine(Teleport());
		}
		if (tracking && target != null)
		{
			yaw = 57.29578f * Mathf.Atan((0f - target.transform.position.x + base.transform.position.x) / (0f - target.transform.position.z + base.transform.position.z));
			base.transform.rotation = Quaternion.Euler(new Vector3(0f, yaw, 0f));
		}
	}

	public void Selected(TargetQueue i_TargetQueue)
	{
		mTargetQueue = i_TargetQueue;
		targetted = true;
	}

	private IEnumerator Teleport()
	{
		if (dodgeSFX != null)
		{
			SoundEventManager.Instance.Play(dodgeSFX, base.gameObject);
		}
		if (dodgeVO != null)
		{
			SoundEventManager.Instance.Play(dodgeVO, base.gameObject);
		}
		yield return new WaitForSeconds(disappearTime);
		if (mTargetQueue != null)
		{
			mTargetQueue.RemoveGameObject(base.gameObject);
			mTargetQueue = null;
		}
		base.gameObject.layer = Layers.EnemiesDontTarget;
		if (!(base.gameObject != Shooter.currentTarget))
		{
			yield break;
		}
		if (projectile != null && ammoCount > 0)
		{
			Transform proj = (Transform)Object.Instantiate(projectile, base.transform.position + new Vector3(0f, 0f, 0.3f), base.transform.rotation);
			proj.parent = base.transform.parent;
			if ((bool)proj.GetComponent<ArcProjectile>())
			{
				proj.GetComponent<ArcProjectile>().SetShooter(base.gameObject.name);
			}
			ammoCount--;
		}
		if (dodgeVFX != null)
		{
			Object.Instantiate(dodgeVFX, base.transform.position + new Vector3(0f, base.GetComponent<Collider>().bounds.extents.y, 0f), base.transform.rotation);
		}
		Accessory[] array = accessories;
		foreach (Accessory acc in array)
		{
			acc.HideAccessory();
		}
		GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		yield return new WaitForSeconds(timeDodging);
		if (teleportLocation != null)
		{
			if (base.transform.position == teleportLocation.position)
			{
				iTween.MoveTo(base.gameObject, startPos, timeDodging);
			}
			else
			{
				iTween.MoveTo(base.gameObject, teleportLocation.position, timeDodging);
			}
		}
		Reappear();
	}

	private void Reappear()
	{
	}

	private void BecomeTargettable()
	{
		vulnerable = true;
		base.gameObject.layer = Layers.Enemies;
	}

	private void Idle()
	{
		vulnerable = false;
		myAnim.Jester_Idle();
		myAnim.Offset(Random.Range(0f, 0.5f));
	}

	public void Disable()
	{
		if (myDizzy != null)
		{
			Object.Destroy(myDizzy);
		}
		tracking = false;
		StopAllCoroutines();
		CancelInvoke();
	}

	public void VictoryDance()
	{
		if (myDizzy != null)
		{
			Object.Destroy(myDizzy);
		}
		tracking = false;
		StopAllCoroutines();
		CancelInvoke();
	}

	public void SpawnPoof()
	{
		if (dodgeVFX != null)
		{
			Object.Instantiate(dodgeVFX, base.transform.position + new Vector3(0f, base.GetComponent<Collider>().bounds.extents.y, 0f), base.transform.rotation);
		}
	}

	private void OnDestroy()
	{
		if (!SafeMonoBehaviour.IsShuttingDown && SoundEventManager.Instance != null)
		{
			SoundEventManager.Instance.Stop(dizzyVO, base.gameObject);
		}
	}
}
