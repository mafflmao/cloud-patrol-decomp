using System;
using System.Collections;
using UnityEngine;

public class EnemyDodger : SafeMonoBehaviour
{
	public GameObject hardHat;

	public bool forceHard;

	public float initialDizzyTime = 3f;

	public float initialDizzyTimeHard = 0.75f;

	public float hardDodgerDifficulty = 2f;

	private WizardStunUpgrade _wizardStun;

	private WizardExplosionUpgrade _wizardExplode;

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

	private bool _isDizzy;

	private bool targetted;

	private TargetQueue mTargetQueue;

	private bool tracking = true;

	private static GameObject _target;

	private float yaw;

	private GameObject myDizzy;

	public int numOfShots = 3;

	private int numOfShotsFired;

	public float autoTeleportTime = 3f;

	private float autoTimer;

	private bool disappearedOnce;

	public float DizzyTime
	{
		get
		{
			float num = 0f;
			num = ((!forceHard) ? DifficultyManager.Instance.WizardDizzyTime : DifficultyManager.Instance.WizardHardDizzyTime);
			if (_wizardStun != null)
			{
				return num + _wizardStun.additonalStunDelay;
			}
			return num;
		}
	}

	public float DodgeTime
	{
		get
		{
			if (forceHard)
			{
				return DifficultyManager.Instance.WizardHardDodgeTime;
			}
			return DifficultyManager.Instance.WizardDodgeTime;
		}
	}

	private void OnEnable()
	{
		mTargetQueue = null;
		myAnim = GetComponent<AnimationStates>();
		if (myAnim != null)
		{
			Idle();
		}
		startPos = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		accessories = GetComponents<Accessory>();
		tracking = true;
		if (_target == null)
		{
			_target = GameObject.FindGameObjectWithTag("ProjectileTarget");
		}
		_wizardStun = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<WizardStunUpgrade>();
		_wizardExplode = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<WizardExplosionUpgrade>();
		if (_wizardStun != null && _wizardStun.stunOnSpawn)
		{
			ForceDizzy();
		}
		Health.Killed += OnKilled;
	}

	private void Start()
	{
		if (DifficultyManager.ShouldSpawnRedWizard || forceHard)
		{
			TurnRed();
		}
	}

	private void TurnRed()
	{
		forceHard = true;
		if (hardHat != null)
		{
			GetComponent<Accessory>().SwapAccessory(hardHat);
		}
	}

	private void OnDisable()
	{
		disappearedOnce = true;
		CancelInvoke();
		StopAllCoroutines();
		tracking = false;
		Health.Killed -= OnKilled;
	}

	private void OnKilled(object sender, EventArgs args)
	{
		if (!((Health)sender != GetComponent<Health>()) && _wizardExplode != null && ((forceHard && _wizardExplode.onlyRedWizards) || !_wizardExplode.onlyRedWizards))
		{
			_wizardExplode.SpawnExplosion(base.transform.position);
		}
	}

	private void Update()
	{
		if (!_isDizzy)
		{
			if (targetted && !_isDizzy)
			{
				targetted = false;
				myAnim.Jester_Disappear();
				StartCoroutine(Teleport());
			}
			else if (autoTimer < Time.time && !disappearedOnce)
			{
				targetted = false;
				myAnim.Jester_Disappear();
				StartCoroutine(Teleport());
			}
			if (tracking && _target != null)
			{
				yaw = 57.29578f * Mathf.Atan((0f - _target.transform.position.x + base.transform.position.x) / (0f - _target.transform.position.z + base.transform.position.z));
				base.transform.rotation = Quaternion.Euler(new Vector3(0f, yaw, 0f));
			}
		}
	}

	public void Selected(TargetQueue i_TargetQueue)
	{
		mTargetQueue = i_TargetQueue;
		targetted = true;
	}

	private IEnumerator Teleport()
	{
		disappearedOnce = true;
		if (TimeTwister.IsActive)
		{
			yield return new WaitForSeconds(1f);
		}
		base.gameObject.layer = Layers.EnemiesDontTarget;
		if (mTargetQueue != null)
		{
			mTargetQueue.RemoveGameObject(base.gameObject);
			mTargetQueue = null;
		}
		if (dodgeSFX != null)
		{
			SoundEventManager.Instance.Play(dodgeSFX, base.gameObject);
		}
		if (dodgeVO != null)
		{
			SoundEventManager.Instance.Play(dodgeVO, base.gameObject);
		}
		yield return new WaitForSeconds(0.1f);
		if (projectile != null && numOfShotsFired < numOfShots)
		{
			Transform proj = (Transform)UnityEngine.Object.Instantiate(projectile, base.transform.position + new Vector3(0f, 0.3f, 0.3f), base.transform.rotation);
			proj.parent = base.transform.parent;
			if ((bool)proj.GetComponent<ArcProjectile>())
			{
				proj.GetComponent<ArcProjectile>().SetShooter(base.gameObject.name);
			}
			numOfShotsFired++;
		}
		if (dodgeVFX != null)
		{
			UnityEngine.Object.Instantiate(dodgeVFX, base.transform.position + new Vector3(0f, base.GetComponent<Collider>().bounds.extents.y, 0f), base.transform.rotation);
		}
		Accessory[] array = accessories;
		foreach (Accessory acc in array)
		{
			acc.HideAccessory();
		}
		GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		yield return new WaitForSeconds(DodgeTime);
		if (teleportLocation != null)
		{
			if (base.transform.position == teleportLocation.position)
			{
				base.transform.position = startPos;
			}
			else
			{
				base.transform.position = teleportLocation.position;
			}
		}
		Reappear();
	}

	private void ForceDizzy()
	{
	}

	private void Reappear()
	{

	}

	private void BecomeTargettable()
	{
		_isDizzy = true;
		base.gameObject.layer = Layers.Enemies;
	}

	private void Idle()
	{
		_isDizzy = false;
		disappearedOnce = false;
		StopDizzyFX();
		autoTimer = Time.time + autoTeleportTime;
		myAnim.Jester_Idle();
		myAnim.Offset(UnityEngine.Random.Range(0f, 0.5f));
	}

	public void Disable()
	{
		StopDizzyFX();
		disappearedOnce = true;
		tracking = false;
		StopAllCoroutines();
		CancelInvoke();
	}

	public void VictoryDance()
	{
		StopDizzyFX();
		disappearedOnce = true;
		tracking = false;
		StopAllCoroutines();
		CancelInvoke();
	}

	private void StopDizzyFX()
	{
		if (myDizzy != null)
		{
			UnityEngine.Object.Destroy(myDizzy);
		}
	}

	public void SpawnPoof()
	{
		if (dodgeVFX != null)
		{
			UnityEngine.Object.Instantiate(dodgeVFX, base.transform.position + new Vector3(0f, base.GetComponent<Collider>().bounds.extents.y, 0f), base.transform.rotation);
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
