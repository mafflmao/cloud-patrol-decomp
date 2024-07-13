using System;
using System.Collections;
using UnityEngine;

public class EnemyShooterShielded : MonoBehaviour
{
	private float timer;

	public int state;

	public float initWaitTime = 2f;

	public float fireDelay = 2f;

	public float chargeTime = 1f;

	public float shieldDownTime = 2f;

	public float shieldUpTime = 2f;

	private float _invulnerableDelay = 0.8f;

	private float _vulnerableDelay = 0.15f;

	public int ammoCount = 5;

	public SoundEventData SFX_Telegraph_Shoot_Fire;

	public SoundEventData VO_Taunt;

	public SoundEventData SFX_Attack_Shoot_Fire;

	public SoundEventData SFX_ShieldUp;

	public SoundEventData SFX_ShieldDown;

	public Flipbook fireFX;

	private bool doOnce;

	public bool fireThruMessage;

	public bool fireNow;

	private AnimationStates myAnim;

	public bool hazardShield;

	private HazardBombProxy spikeHazard;

	private Health myHealth;

	private bool hazardProxyTriggered;

	public Transform projectile;

	private Transform spawnPoint;

	private float yaw;

	private GameObject gun;

	private bool tracking = true;

	private bool block;

	private bool _blockNullified;

	private bool _isRed;

	private void OnEnable()
	{
		ShootThroughShieldsUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<ShootThroughShieldsUpgrade>();
		_blockNullified = passiveUpgradeOrDefault != null;
		gun = GameObject.FindGameObjectWithTag("ProjectileTarget");
		tracking = true;
		myAnim = GetComponent<AnimationStates>();
		if ((bool)myAnim)
		{
			myAnim.speed = 1f;
			myAnim.PlayAnim("Shield_ready");
			myAnim.Offset(myAnim.CurrentClipLength);
		}
		myHealth = GetComponent<Health>();
		if (myHealth != null)
		{
			myHealth.isDeflecting = true && !_blockNullified;
		}
		if (hazardShield)
		{
			spikeHazard = base.gameObject.GetComponent<HazardBombProxy>();
			if ((bool)spikeHazard)
			{
				spikeHazard.SetActive(false);
			}
		}
		spawnPoint = TransformUtil.FindRecursive(base.transform, "bRHand");
		state = 0;
		if (hazardShield && (bool)spikeHazard)
		{
			spikeHazard.SetActive(true && !_blockNullified);
			Hazard.HazardHurtPlayer += HandleHazardHazardHurtPlayer;
			HazardBombProxy.HazardProxyTriggered += HandleHazardBombProxyTriggered;
		}
		if (GetComponent<TripleShooterModifier>() != null || DifficultyManager.ShouldSpawnRedTrolls)
		{
			TurnRed();
		}
		shieldUpTime = DifficultyManager.Instance.ShieldUpTime;
		shieldDownTime = DifficultyManager.Instance.ShieldDownTime;
		fireDelay = DifficultyManager.Instance.ShieldFireTime;
		chargeTime = DifficultyManager.Instance.ShieldChargeTime;
	}

	protected void TurnRed()
	{
		_isRed = true;
		if (GetComponent<TripleShooterModifier>() == null)
		{
			base.gameObject.AddComponent<TripleShooterModifier>();
		}
		Transform transform = TransformUtil.FindRecursive(myAnim.anim.transform, "TrollWarrior");
		if (transform != null)
		{
			transform.GetComponent<Renderer>().material.color = TripleShooterModifier.TrollColor;
		}
		if (myHealth != null)
		{
			myHealth.scoreType = ScoreData.ScoreType.ENEMY_SHOOTER_RED;
		}
	}

	private void HandleHazardBombProxyTriggered(object sender, EventArgs e)
	{
		if (sender == spikeHazard)
		{
			if (block)
			{
				hazardProxyTriggered = true;
			}
			if (SkyIronShield.ActiveShield != null)
			{
				KnockTroll();
			}
		}
	}

	private void OnDisable()
	{
		tracking = false;
		StopAllCoroutines();
		CancelInvoke();
		if (hazardShield)
		{
			Hazard.HazardHurtPlayer -= HandleHazardHazardHurtPlayer;
			HazardBombProxy.HazardProxyTriggered -= HandleHazardBombProxyTriggered;
		}
		Vulnerable();
	}

	private void HandleHazardHazardHurtPlayer(object sender, EventArgs e)
	{
		if (spikeHazard.spawnedExplosion != null && (bool)spikeHazard && sender == spikeHazard.spawnedExplosion.GetComponent<Hazard>() && spikeHazard.spawnedExplosion != null)
		{
			spikeHazard.spawnedExplosion.GetComponent<Hazard>().originatingGameObject = base.gameObject.name;
			KnockTroll();
		}
	}

	private void KnockTroll()
	{
		block = false;
		if (myHealth != null)
		{
			myHealth.isDeflecting = false;
			DamageInfo damageInfo = new DamageInfo();
			damageInfo.damageAmount = 100;
			damageInfo.damageType = DamageTypes.normal;
			damageInfo.comboNum = 0;
			myHealth.TakeHit(damageInfo);
		}
		ShipManager.instance.dragMultiTarget[0].targetQueue.RemoveGameObject(base.gameObject);
		base.gameObject.layer = Layers.EnemiesDontTarget;
	}

	private void Update()
	{
		if (!fireThruMessage)
		{
			FireSequence();
		}
		else if (fireNow)
		{
			FireSequence();
		}
		if (tracking && gun != null)
		{
			yaw = 57.29578f * Mathf.Atan((0f - gun.transform.position.x + base.transform.position.x) / (0f - gun.transform.position.z + base.transform.position.z));
			base.transform.rotation = Quaternion.Euler(new Vector3(0f, yaw, 0f));
		}
	}

	private void FireSequence()
	{
		if (!(timer <= Time.time))
		{
			return;
		}
		if (state == 0)
		{
			myAnim.speed = 1f;
			float num = myAnim.Shield_Ready();
			InvokeHelper.InvokeSafe(Invulnerable, _invulnerableDelay, this);
			if (!doOnce)
			{
				timer = Time.time + num + TripleShooterModifier.GetTime(initWaitTime / DifficultyManager.Instance.ShieldAnimationSpeed, _isRed);
				doOnce = true;
			}
			else
			{
				SoundEventManager.Instance.Play(SFX_ShieldDown, base.gameObject, 0.7f);
				timer = Time.time + num + TripleShooterModifier.GetTime(shieldDownTime, _isRed);
			}
			if (ammoCount > 0)
			{
				ammoCount--;
				state = 1;
			}
			else if (doOnce)
			{
				state = 4;
				SoundEventManager.Instance.Play(SFX_ShieldDown, base.gameObject, 0.7f);
				timer = Time.time + num + TripleShooterModifier.GetTime(shieldUpTime, _isRed);
			}
		}
		else if (state == 1)
		{
			if (hazardProxyTriggered)
			{
				if ((bool)SkyIronShield.ActiveShield)
				{
					KnockTroll();
				}
				return;
			}
			InvokeHelper.InvokeSafe(Vulnerable, _vulnerableDelay, this);
			SoundEventManager.Instance.Play(SFX_Telegraph_Shoot_Fire, base.gameObject, 0.5f / DifficultyManager.Instance.ShieldAnimationSpeed);
			SoundEventManager.Instance.Play(VO_Taunt, base.gameObject);
			SoundEventManager.Instance.Play(SFX_ShieldUp, base.gameObject);
			myAnim.speed = DifficultyManager.Instance.ShieldAnimationSpeed;
			StartCoroutine(myAnim.Shield_Gunner_Ready());
			timer = Time.time + TripleShooterModifier.GetTime(fireDelay, _isRed);
			if (fireFX != null)
			{
				InvokeHelper.InvokeSafe(fireFX.Play, chargeTime, fireFX);
			}
			state = 2;
		}
		else if (state == 2)
		{
			myAnim.speed = DifficultyManager.Instance.ShieldAnimationSpeed;
			myAnim.Shield_Gunner_Shoot();
			SoundEventManager.Instance.Play(SFX_Attack_Shoot_Fire, base.gameObject);
			Transform transform = (Transform)UnityEngine.Object.Instantiate(projectile, spawnPoint.position, base.transform.rotation);
			transform.parent = base.transform.parent;
			if ((bool)transform.GetComponent<ArcProjectile>())
			{
				transform.GetComponent<ArcProjectile>().SetShooter(base.gameObject.name);
			}
			if (_isRed)
			{
				transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				transform.GetComponent<Renderer>().material.color = TripleShooterModifier.BulletColor;
				transform.GetComponent<Health>().scoreType = ScoreData.ScoreType.PROJECTILE_CORKSCREW;
				InvokeHelper.InvokeSafe(ShootProjectile, 0.1f, this);
				InvokeHelper.InvokeSafe(ShootProjectile, 0.2f, this);
			}
			timer = Time.time + TripleShooterModifier.GetTime(0.6f / DifficultyManager.Instance.ShieldAnimationSpeed, _isRed);
			state = 3;
		}
		else if (state == 3)
		{
			if (!fireThruMessage)
			{
				state = 0;
				return;
			}
			state = 0;
			fireNow = false;
		}
		else if (state == 4)
		{
			myAnim.speed = 1f;
			float num2 = myAnim.PlayAnim("Shield_reload");
			InvokeHelper.InvokeSafe(Vulnerable, _vulnerableDelay, this);
			SoundEventManager.Instance.Play(SFX_ShieldUp, base.gameObject);
			state = 5;
			timer = Time.time + num2;
		}
		else if (state == 5)
		{
			myAnim.speed = 1f;
			myAnim.PlayAnim("Shield_idle");
			timer = Time.time + TripleShooterModifier.GetTime(fireDelay, _isRed);
			state = 6;
		}
		else if (state == 6)
		{
			myAnim.speed = DifficultyManager.Instance.ShieldAnimationSpeed;
			float num3 = myAnim.Shield_Ready();
			InvokeHelper.InvokeSafe(Invulnerable, _invulnerableDelay, this);
			SoundEventManager.Instance.Play(SFX_ShieldDown, base.gameObject, 0.7f / DifficultyManager.Instance.ShieldAnimationSpeed);
			state = 4;
			timer = Time.time + num3 + TripleShooterModifier.GetTime(fireDelay, _isRed);
		}
	}

	private void ShootProjectile()
	{
		SoundEventManager.Instance.Play(SFX_Attack_Shoot_Fire, base.gameObject);
		Transform transform = (Transform)UnityEngine.Object.Instantiate(projectile, spawnPoint.position, base.transform.rotation);
		transform.parent = base.transform.parent;
		transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		ArcProjectile component = transform.GetComponent<ArcProjectile>();
		if ((bool)component)
		{
			component.SetShooter(base.gameObject.name);
		}
		transform.GetComponent<Health>().scoreType = ScoreData.ScoreType.PROJECTILE_CORKSCREW;
		transform.GetComponent<Renderer>().material.color = TripleShooterModifier.BulletColor;
	}

	private void Invulnerable()
	{
		myHealth.isDeflecting = true && !_blockNullified;
		block = true && !_blockNullified;
		if (hazardShield && (bool)spikeHazard && !_blockNullified)
		{
			spikeHazard.SetActive(true);
		}
	}

	private void Vulnerable()
	{
		myHealth.isDeflecting = false;
		block = false;
		if (hazardShield && (bool)spikeHazard)
		{
			spikeHazard.SetActive(false);
		}
	}

	public void Disable()
	{
		fireNow = false;
		tracking = false;
		state = 100;
		StopAllCoroutines();
		CancelInvoke();
		Vulnerable();
		if (fireFX != null && fireFX.gameObject != null)
		{
			UnityEngine.Object.Destroy(fireFX.gameObject);
		}
		InvokeHelper.InvokeSafe(DetachShield, 0.1f, this);
	}

	private void DetachShield()
	{
		Accessory[] components = GetComponents<Accessory>();
		Accessory[] array = components;
		foreach (Accessory accessory in array)
		{
			accessory.Detach();
		}
	}

	private IEnumerator VictoryDance()
	{
		fireThruMessage = true;
		fireNow = false;
		int rand = UnityEngine.Random.Range(1, 4);
		float randWait = UnityEngine.Random.Range(0.4f, 0.8f);
		yield return new WaitForSeconds(randWait);
		CancelInvoke();
		myAnim.StopCoroutine("Shield_Reload");
		if (rand <= 2)
		{
			myAnim.StopCoroutine("Shield_Reload");
			Vulnerable();
			if (block)
			{
				SoundEventManager.Instance.Play(SFX_ShieldUp, base.gameObject);
				myAnim.speed = DifficultyManager.Instance.ShieldAnimationSpeed;
				myAnim.PlayAnim("Shield_reload");
				yield return new WaitForSeconds(myAnim.anim.clip.length);
				myAnim.Shield_Victory();
			}
			else
			{
				myAnim.Shield_Victory();
			}
			SoundEventManager.Instance.Play(SFX_ShieldUp, base.gameObject, 0.5f);
			yield return new WaitForSeconds(myAnim.anim.clip.length);
			myAnim.PlayAnim("Shield_reload");
			myAnim.Offset(0.2f);
			yield return new WaitForSeconds(myAnim.anim.clip.length);
			myAnim.PlayAnim("Shield_idle");
		}
		else
		{
			myAnim.PlayAnim("Shield_idle");
			yield return 0;
		}
	}

	public void QuitGame()
	{
		fireThruMessage = true;
		fireNow = false;
		CancelInvoke();
		myAnim.StopCoroutine("Shield_Reload");
		myAnim.PlayAnim("Shield_idle");
	}
}
