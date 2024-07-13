using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int hitPoints = 30;

	public bool noKill;

	private bool wasKilled;

	public Transform takeHitEffect;

	public Transform killEffect;

	public Transform poofEffect;

	public Transform dropSpawn;

	public ScoreData.ScoreType scoreType;

	public int coinsSpawnedOnHit;

	public float destroyTimeAfterHit = 2.5f;

	public SoundEventData deathSFXSoundEventData;

	public SoundEventData lastEnemyDeathSFXSoundEventData;

	public SoundEventData takeHitSoundEventData;

	public SoundEventData takeHitVOSoundEventData;

	public SoundEventData deflectSFX;

	public bool isDeflecting;

	public bool isForceFielded;

	public bool isDead;

	private GameObject moneyDrop;

	public bool isEnemy = true;

	public Vector3 trajectory = new Vector3(0f, 75f, -100f);

	private Vector3 newPos;

	private float invulnerabilityTimeAfterHit = 0.25f;

	private bool temporarilyInvulnerable;

	public int hitWithComboNumber;

	public static event EventHandler<EventArgs> TookHit;

	public static event EventHandler<EventArgs> Deflected;

	public static event EventHandler<EventArgs> Killed;

	private void Start()
	{
		moneyDrop = ShipManager.instance.moneyDrop;
	}

	private void MakeInvulnerableFor(float timeInSeconds)
	{
		temporarilyInvulnerable = true;
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(MakeVulnerableIn(timeInSeconds));
		}
	}

	private IEnumerator MakeVulnerableIn(float timeInSeconds)
	{
		yield return new WaitForSeconds(timeInSeconds);
		temporarilyInvulnerable = false;
	}

	public bool TakeHit(DamageInfo damageInfo)
	{
		if (temporarilyInvulnerable)
		{
			DebugScreen.Log("TakeHit Invulnerable");
			return false;
		}
		MakeInvulnerableFor(invulnerabilityTimeAfterHit);
		if (isDeflecting)
		{
			DebugScreen.Log("TakeHit IsDeflecting");
			if ((bool)deflectSFX)
			{
				SoundEventManager.Instance.Play(deflectSFX, base.gameObject);
			}
			OnDeflect();
			return false;
		}
		if (isForceFielded)
		{
			DebugScreen.Log("TakeHit IsForceFielded");
			OnDeflect();
			return false;
		}
		hitWithComboNumber = damageInfo.comboNum;
		hitPoints -= damageInfo.damageAmount;
		TextureStateController component = GetComponent<TextureStateController>();
		if (component != null)
		{
			component.SendMessage("SetDamageState", hitPoints);
		}
		DamageStates component2 = GetComponent<DamageStates>();
		if (component2 != null)
		{
			component2.SendMessage("SetDamageState", hitPoints);
		}
		if (takeHitSoundEventData != null && damageInfo.damageType != DamageTypes.anvil)
		{
			SoundEventManager.Instance.Play(takeHitSoundEventData, base.gameObject);
		}
		if (takeHitVOSoundEventData != null)
		{
			SoundEventManager.Instance.Play(takeHitVOSoundEventData, base.gameObject);
		}
		OnTookHit();
		if (hitPoints <= 0)
		{
			DropLoot();
			bool flingTrolls = damageInfo.damageType != DamageTypes.explosive;
			Kill(flingTrolls);
			DebugScreen.Log("TakeHit Dead");
		}
		else
		{
			if (coinsSpawnedOnHit > 0)
			{
				DropLoot();
			}
			if (takeHitEffect != null)
			{
				UnityEngine.Object.Instantiate(takeHitEffect, base.GetComponent<Collider>().bounds.center, Quaternion.identity);
				StartCoroutine(StartGameSettings.Instance.activeSkylander.PlayFXOverlay(CharacterData.FXOverlayType.HitEffect, base.GetComponent<Collider>().bounds.center, Quaternion.identity));
			}
		}
		return true;
	}

	private bool IsLastEnemyInRoom()
	{
		if (!isEnemy)
		{
			return false;
		}
		Transform parent = base.transform.parent;
		if (parent == null)
		{
			return false;
		}
		ScreenManager component = parent.GetComponent<ScreenManager>();
		if (component == null)
		{
			return false;
		}
		int childCount = parent.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = parent.transform.GetChild(i).gameObject;
			if (gameObject != base.gameObject)
			{
				Health component2 = gameObject.GetComponent<Health>();
				if (component2 != null && component2.isEnemy)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void Kill()
	{
		Kill(true);
	}

	public void Kill(bool flingTrolls)
	{
		if (noKill)
		{
			return;
		}
		OnKilled();
		isDead = true;
		base.gameObject.layer = Layers.EnemiesDontTarget;
		ShipManager.instance.RemoveTarget(base.gameObject);
		if (deathSFXSoundEventData != null || lastEnemyDeathSFXSoundEventData != null)
		{
			bool flag = IsLastEnemyInRoom();
			if (lastEnemyDeathSFXSoundEventData != null && flag)
			{
				SoundEventManager.Instance.Play(lastEnemyDeathSFXSoundEventData, base.gameObject);
			}
			else if (deathSFXSoundEventData != null)
			{
				SoundEventManager.Instance.Play(deathSFXSoundEventData, base.gameObject);
			}
		}
		if (killEffect != null && !isEnemy)
		{
			UnityEngine.Object.Instantiate(killEffect, base.GetComponent<Collider>().bounds.center, Quaternion.identity);
		}
		else
		{
			GameObject deathFxPrefab = GlobalFXData.Instance.DeathFxPrefab;
			if (deathFxPrefab != null)
			{
				UnityEngine.Object.Instantiate(deathFxPrefab, base.GetComponent<Collider>().bounds.center, Quaternion.identity);
			}
		}
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(StartGameSettings.Instance.activeSkylander.PlayFXOverlay(CharacterData.FXOverlayType.KillEffect, base.GetComponent<Collider>().bounds.center, Quaternion.identity));
		}
		if (deathSFXSoundEventData == null && killEffect == null)
		{
			Debug.LogWarning("There was no death FX or SFX for " + base.gameObject.name);
		}
		if (isEnemy && !wasKilled)
		{
			wasKilled = true;
			GameManager.EnemyKilled();
		}
		base.gameObject.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);
		AnimationStates component = GetComponent<AnimationStates>();
		Hat component2 = base.gameObject.GetComponent<Hat>();
		if (component != null || component2 != null)
		{
			if (flingTrolls)
			{
				if (base.gameObject.GetComponent<Rigidbody>() == null)
				{
					base.gameObject.AddComponent<Rigidbody>();
				}
				base.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
				base.gameObject.GetComponent<Rigidbody>().angularDrag = 0f;
				if (base.transform.position.x > Camera.main.transform.position.x)
				{
					CalculateTrajectory(true);
				}
				else
				{
					CalculateTrajectory(false);
				}
				float num = 1f;
				if (hitWithComboNumber == GameManager.gunSlotCount)
				{
					num = 2f;
				}
				base.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(trajectory.x * num, trajectory.y * num, trajectory.z * num));
				base.gameObject.GetComponent<Rigidbody>().AddTorque(UnityEngine.Random.insideUnitSphere * 20f * num);
				base.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
			}
			if ((bool)component)
			{
				component.Death(destroyTimeAfterHit);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject, 0.5f);
			}
			if ((bool)base.transform.parent && (bool)base.transform.parent.parent && !component2)
			{
				base.transform.parent = base.transform.parent.parent;
			}
			else
			{
				base.transform.parent = null;
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void CalculateTrajectory(bool clockwise)
	{
		float distance = 275f;
		if (CheckDirection(Vector3.back, distance))
		{
			return;
		}
		if (clockwise)
		{
			if (!CheckDirection(Vector3.right, distance) && !CheckDirection(Vector3.left, distance))
			{
				CheckDirection(Vector3.up, distance);
			}
		}
		else if (!CheckDirection(Vector3.left, distance) && !CheckDirection(Vector3.right, distance))
		{
			CheckDirection(Vector3.up, distance);
		}
	}

	private bool CheckDirection(Vector3 dir, float distance)
	{
		Vector3 center = base.transform.GetComponent<Collider>().bounds.center;
		RaycastHit[] array = Physics.RaycastAll(center, Vector3.Normalize(dir), distance);
		if (array.Length == 0)
		{
			Vector3 up = Vector3.up;
			RaycastHit[] array2 = Physics.RaycastAll(center, Vector3.Normalize(dir + up), distance);
			if (array2.Length == 0)
			{
				trajectory = Vector3.Normalize(dir + up) * distance;
				return true;
			}
			return true;
		}
		return false;
	}

	public void DropLoot()
	{
		if (coinsSpawnedOnHit > 0 && hitPoints > 0)
		{
			for (int i = 0; i < coinsSpawnedOnHit; i++)
			{
				if ((bool)moneyDrop && (bool)base.gameObject)
				{
					UnityEngine.Object.Instantiate(moneyDrop, base.GetComponent<Collider>().bounds.center, Quaternion.identity);
				}
			}
			SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Coin_SFX_Burst_large, base.gameObject);
		}
		else
		{
			for (int j = 0; j < hitWithComboNumber; j++)
			{
				if ((bool)moneyDrop && (bool)base.gameObject)
				{
					UnityEngine.Object.Instantiate(moneyDrop, base.GetComponent<Collider>().bounds.center, Quaternion.identity);
				}
			}
			if (hitWithComboNumber >= 1 && hitWithComboNumber <= 2)
			{
				SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Coin_SFX_Burst_small, base.gameObject);
			}
			else if (hitWithComboNumber <= 4 && hitWithComboNumber > 2)
			{
				SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Coin_SFX_Burst_medium, base.gameObject);
			}
			else if (hitWithComboNumber == 5)
			{
				SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Coin_SFX_Burst_large, base.gameObject);
			}
		}
		if (!(dropSpawn != null))
		{
			return;
		}
		Transform transform = (Transform)UnityEngine.Object.Instantiate(dropSpawn, base.transform.position, Quaternion.identity);
		transform.transform.parent = base.transform.parent;
		MoverBounce component = transform.GetComponent<MoverBounce>();
		if (component != null && component.sheepMover)
		{
			component.jumpOnStart = true;
			Mover component2 = transform.GetComponent<Mover>();
			if ((bool)component2)
			{
				component2.direction = new Vector3(0f, 0f, 1f);
				component2.speed = 1f;
			}
		}
	}

	public void SpawnPoof()
	{
		if ((bool)poofEffect)
		{
			UnityEngine.Object.Instantiate(poofEffect, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
		}
	}

	private void OnTookHit()
	{
		if (Health.TookHit != null)
		{
			Health.TookHit(this, new EventArgs());
		}
	}

	private void OnDeflect()
	{
		if (Health.Deflected != null)
		{
			Health.Deflected(this, new EventArgs());
		}
	}

	public void OnKilled()
	{
		if (LevelManager.Instance.FinishedTutorials)
		{
			if (IsDestructibleModifier.IsHealthAttachedToDestructible(this))
			{
				AchievementManager.Instance.IncrementStep(Achievements.DestroyObjects);
			}
			else if (IsSheepModifier.IsHealthAttachedToSheep(this))
			{
				AchievementManager.Instance.IncrementStep(Achievements.ShootSheep);
			}
			else if (IsProjectileModifier.IsHealthAttachedToProjectile(this))
			{
				AchievementManager.Instance.IncrementStep(Achievements.ShootMissiles);
				Achievements.AchievementData achievement = AchievementManager.Instance.GetAchievement(Achievements.ShootMissiles);
				Debug.Log("ShootMissiles is " + achievement.step + "/" + achievement.stepCount);
			}
		}
		if (Health.Killed != null)
		{
			Health.Killed(this, new EventArgs());
		}
	}

	public void Disable()
	{
		StopAllCoroutines();
		CancelInvoke();
	}

	public void OnDestroy()
	{
		ShipManager.instance.RemoveTarget(base.gameObject);
	}
}
