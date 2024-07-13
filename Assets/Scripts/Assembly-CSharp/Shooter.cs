using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Shooter : MonoBehaviour
{
	public class ShootEventArgs : CancellableEventArgs
	{
		public int ComboSize
		{
			get
			{
				return Targets.Length;
			}
		}

		public GameObject[] Targets { get; private set; }

		public ShootEventArgs(GameObject[] targets)
		{
			Targets = targets.ToArray();
		}
	}

	public class ComboCompletedEventArgs : EventArgs
	{
		public int Number { get; private set; }

		public ComboCompletedEventArgs(int number)
		{
			Number = number;
		}
	}

	public static GameObject currentTarget;

	public static Vector3 lastTargetWorldPosition;

	public int damage = 10;

	public DamageTypes attackDamageType;

	public TargetQueue targetQueue;

	public GameObject tracer;

	public GameObject muzzleFlash;

	public float rateOfFire = 0.1f;

	public float bulletTravelTime = 0.1f;

	public float windUpTime = 0.2f;

	public GameObject comboCoin;

	public ShipVisual weaponVisual;

	public int comboCount;

	public bool isShooting;

	public SoundEventData fireSound;

	public SoundEventData fireMaxComboSound;

	private int _shotNumber;

	private int _shotSequenceNumber;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(Shooter), LogLevel.Error);

	private int numTargetsHit;

	private AimModifier _aimMod;

	private GameObject mostRecentlyHitTarget;

	private Vector3 mostRecentlyHitTargetPosition;

	private AimModifier aimMod
	{
		get
		{
			if (_aimMod == null && weaponVisual != null)
			{
				_aimMod = ShipManager.instance.shipVisual.StaticSkylander.GetComponent<AimModifier>();
			}
			return _aimMod;
		}
	}

	public static event EventHandler<ShootEventArgs> Shooting;

	public static event EventHandler<ComboCompletedEventArgs> ComboCompleted;

	private void Awake()
	{
		targetQueue = GetComponent<TargetQueue>();
	}

	private void Start()
	{
		CharacterAudioResources audioResources = StartGameSettings.Instance.activeSkylander.AudioResources;
		if (audioResources == null)
		{
			_log.LogError("No audio resource found for {0}!", StartGameSettings.Instance.activeSkylander.charName);
		}
		else if (audioResources.shootOverrideSfx != null)
		{
			fireSound = audioResources.shootOverrideSfx;
		}
	}

	public void StartCombo()
	{
		comboCount = 0;
	}

	public virtual void FireAtTargets()
	{
		if (targetQueue.Count <= 0)
		{
			return;
		}
		int num = 0;
		GameObject[] targets = targetQueue.Targets;
		foreach (GameObject gameObject in targets)
		{
			if (gameObject != null && gameObject.CompareTag("Projectile"))
			{
				num++;
			}
		}
		if (num >= 4)
		{
			AchievementManager.Instance.SetStep(Achievements.ComboMissiles, 1);
		}
		ShootEventArgs shootEventArgs = new ShootEventArgs(targetQueue.Targets);
		OnShooting(shootEventArgs);
		if (shootEventArgs.IsCancelled)
		{
			targetQueue.Clear();
			return;
		}
		StartCombo();
		_shotNumber = 0;
		StartCoroutine(AutoFire());
	}

	private void OnShooting(ShootEventArgs args)
	{
		if (Shooter.Shooting != null)
		{
			Shooter.Shooting(this, args);
		}
	}

	public void GhostSwordsShotEvent(GameObject troll)
	{
		OnShooting(new ShootEventArgs(new GameObject[1] { troll }));
	}

	public virtual void StopAutoFire()
	{
		StopCoroutine("AutoFire");
		isShooting = false;
	}

	private IEnumerator AutoFire()
	{
		_shotSequenceNumber++;
		isShooting = true;
		if (aimMod != null)
		{
			aimMod.ResetMuzzle();
		}
		yield return new WaitForSeconds(windUpTime);
		if (weaponVisual != null)
		{
			weaponVisual.Recoil();
		}
		while (targetQueue.Count > 0)
		{
			StartCoroutine(FireAtNextTarget());
			yield return new WaitForSeconds(rateOfFire);
		}
		yield return new WaitForSeconds(bulletTravelTime);
		targetQueue.OnTargetsCleared();
		FinishCombo(mostRecentlyHitTargetPosition);
		if (weaponVisual != null)
		{
			ShipManager.instance.shipVisual.ResetLookatTarget(mostRecentlyHitTargetPosition, 0f);
		}
		isShooting = false;
	}

	public IEnumerator FireAtNextTarget()
	{
		GameObject nextTarget = (currentTarget = targetQueue.GetNextTarget());
		if (nextTarget == null)
		{
			DebugScreen.Log("FireAtNextTarget Finish");
			StopAutoFire();
			FinishCombo(mostRecentlyHitTargetPosition);
			yield break;
		}
		Health nextTargetHealth = nextTarget.GetComponent<Health>();
		if (weaponVisual != null)
		{
			ShipManager.instance.shipVisual.ChangeLookatTarget(nextTarget.transform, 0.2f);
		}
		_shotNumber++;
		if (aimMod != null && aimMod.GetMuzzleParent() != null && muzzleFlash != null)
		{
			muzzleFlash.transform.position = aimMod.GetMuzzleParent().position;
			aimMod.SwitchMuzzle();
		}
		GameObject tmpTracerGO = null;
		Tracer tmpTracer = null;
		if (weaponVisual != null && muzzleFlash != null)
		{
			PlayFireSound();
			muzzleFlash.GetComponent<Animation>().Rewind();
			muzzleFlash.GetComponent<Animation>().Play(PlayMode.StopAll);
			if ((bool)muzzleFlash.GetComponent<HideWhenNotAnimating>())
			{
				muzzleFlash.GetComponent<HideWhenNotAnimating>().Show();
			}
			StartCoroutine(StartGameSettings.Instance.activeSkylander.PlayFXOverlay(CharacterData.FXOverlayType.MuzzleFlash, muzzleFlash.transform.position, muzzleFlash.transform.rotation));
			tmpTracerGO = UnityEngine.Object.Instantiate(tracer) as GameObject;
			tmpTracer = tmpTracerGO.GetComponent<Tracer>();
			tmpTracer.color = StartGameSettings.Instance.activeSkylander.elementData.gunTracerColor;
			tmpTracer.startPoint = muzzleFlash.transform.position;
			tmpTracer.endPoint = nextTarget.GetComponent<Collider>().bounds.center;
			tmpTracer.SetFXOverlay(StartGameSettings.Instance.activeSkylander.GetFXOverlayPrefab(CharacterData.FXOverlayType.Tracer));
			weaponVisual.BarrelRecoil();
		}
		yield return new WaitForSeconds(bulletTravelTime);
		if (nextTarget != null)
		{
			DamageInfo tempDI = new DamageInfo
			{
				damageAmount = damage,
				damageType = attackDamageType,
				comboNum = comboCount
			};
			DebugScreen.Log("FireAtNextTarget TakeHit");
			if (nextTargetHealth.TakeHit(tempDI))
			{
				lastTargetWorldPosition = nextTarget.GetComponent<Collider>().bounds.center;
				mostRecentlyHitTargetPosition = nextTarget.GetComponent<Collider>().bounds.center;
				comboCount++;
				if (HiddenTreasure.IsActive)
				{
					SpawnComboCoin(mostRecentlyHitTargetPosition);
				}
				numTargetsHit++;
			}
			else if (nextTargetHealth.isDeflecting || (nextTargetHealth.isForceFielded && weaponVisual != null))
			{
				tmpTracerGO = UnityEngine.Object.Instantiate(tracer) as GameObject;
				tmpTracer = tmpTracerGO.GetComponent<Tracer>();
				tmpTracer.color = StartGameSettings.Instance.activeSkylander.elementData.gunTracerColor;
				tmpTracer.startPoint = nextTarget.GetComponent<Collider>().bounds.center;
				tmpTracer.endPoint = nextTarget.GetComponent<Collider>().bounds.center + Vector3.Scale(UnityEngine.Random.onUnitSphere, new Vector3(3f, 3f, 3f));
			}
		}
		currentTarget = null;
	}

	private void SpawnComboCoin(Vector3 location)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(comboCoin, location, Quaternion.identity);
		ComboCoin component = gameObject.GetComponent<ComboCoin>();
		component.number = comboCount;
		if (component.number == 1)
		{
			component.isSheep = true;
			component.number = HiddenTreasure.ModifyComboCount(comboCount);
		}
	}

	public void FinishCombo(Vector3 positionOfLastHitEnemy)
	{
		if (comboCount > 1)
		{
			if (!HiddenTreasure.IsActive)
			{
				SpawnComboCoin(positionOfLastHitEnemy);
			}
			if (comboCount > 1)
			{
				float num = UnityEngine.Random.value * (float)comboCount;
				if ((num > 1f || comboCount == GameManager.gunSlotCount) && !StartGameSettings.Instance.activeSkylander.isGiant && weaponVisual != null)
				{
					weaponVisual.StartCoroutine(weaponVisual.VictorySequence());
				}
			}
		}
		else if (numTargetsHit > 0)
		{
			TapComboUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<TapComboUpgrade>();
			if (passiveUpgradeOrDefault != null)
			{
				comboCount = (int)passiveUpgradeOrDefault.comboOnTap;
				SpawnComboCoin(positionOfLastHitEnemy);
			}
		}
		numTargetsHit = 0;
		OnComboCompleted();
	}

	public void Explode(Vector3 position, float radius)
	{
		Collider[] array = Physics.OverlapSphere(position, radius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.gameObject.layer == Layers.Enemies && (bool)collider.GetComponent<Rigidbody>())
			{
				collider.GetComponent<Rigidbody>().isKinematic = false;
				collider.GetComponent<Rigidbody>().useGravity = true;
				collider.GetComponent<Rigidbody>().AddExplosionForce(1000f, position, radius);
			}
		}
	}

	public void PlayFireSound()
	{
		if (targetQueue.Count == GameManager.gunSlotCount)
		{
			SoundEventManager.Instance.Play(fireMaxComboSound, muzzleFlash);
		}
		else
		{
			SoundEventManager.Instance.Play(fireSound, muzzleFlash);
		}
	}

	protected void OnComboCompleted()
	{
		if (Shooter.ComboCompleted != null)
		{
			Shooter.ComboCompleted(this, new ComboCompletedEventArgs(comboCount));
		}
	}
}
