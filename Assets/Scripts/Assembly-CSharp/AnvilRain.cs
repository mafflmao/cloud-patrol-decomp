using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnvilRain : Powerup
{
	public GameObject anvil;

	public GameObject cloudPrefab;

	private int _numTrollsHit;

	private int _numTrollsToHit;

	public static bool IsActive;

	private GameObject _cloudInstance;

	private GameObject _cloud;

	private DamageInfo damageInfo;

	private List<Anvil> _anvils;

	private List<Health> _enemies;

	public SoundEventData thunderSFX;

	protected void Start()
	{
		_anvils = new List<Anvil>();
		_enemies = new List<Health>();
		Powerup powerup = null;
		GoldAnvilUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<GoldAnvilUpgrade>();
		if (passiveUpgradeOrDefault != null)
		{
			powerup = passiveUpgradeOrDefault.goldAnvilRainPrefab;
		}
		else
		{
			LegendaryGoldAnvilUpgrade passiveUpgradeOrDefault2 = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<LegendaryGoldAnvilUpgrade>();
			if (passiveUpgradeOrDefault2 != null)
			{
				powerup = passiveUpgradeOrDefault2.goldAnvilRainPrefab;
			}
		}
		if (powerup != null)
		{
			Powerup component = ((GameObject)Object.Instantiate(powerup.gameObject)).GetComponent<Powerup>();
			if (component != null)
			{
				component.PowerupData = base.PowerupData;
				component.Holder = base.Holder;
				List<Powerup> list = new List<Powerup>();
				list.Add(component);
				component.Holder.OverridePowerup = list;
				component.SetLevel(base.Level, lifeTimeInSeconds);
			}
			Object.Destroy(base.gameObject);
			MagicItemManager.IsMagicItemActive = true;
			return;
		}
		base.gameObject.GetComponent<PowerupCutscene>().enabled = true;
		damageInfo = new DamageInfo();
		damageInfo.comboNum = 1;
		damageInfo.damageType = DamageTypes.anvil;
		damageInfo.damageAmount = 100;
		if (!_cloudInstance)
		{
			_cloudInstance = new GameObject("cloudParent");
			_cloud = Object.Instantiate(cloudPrefab) as GameObject;
			_cloud.transform.parent = _cloudInstance.transform;
			_cloudInstance.transform.parent = Camera.main.transform;
			_cloudInstance.transform.position = Camera.main.transform.position + new Vector3(-0.06f, -0.12f, -3.5f);
		}
		IsActive = true;
	}

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		_numTrollsHit = 0;
		StartCoroutine(TriggerPowerCoroutine());
	}

	public override void SetLevel(int newLevel, float newValue)
	{
		base.Level = newLevel;
		_numTrollsToHit = Mathf.RoundToInt(newValue);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		CleanupAnvils();
	}

	private void CleanupAnvils()
	{
		foreach (Health enemy in _enemies)
		{
			damageInfo.comboNum = 10;
			if (enemy != null && enemy.isEnemy && _numTrollsHit < _numTrollsToHit)
			{
				enemy.TakeHit(damageInfo);
				_numTrollsHit++;
			}
		}
		foreach (Anvil anvil in _anvils)
		{
			if ((bool)anvil)
			{
				Object.Destroy(anvil.gameObject);
			}
		}
		_anvils.Clear();
		_enemies.Clear();
	}

	private IEnumerator TriggerPowerCoroutine()
	{
		_anvils.Clear();
		_enemies.Clear();
		SoundEventManager.Instance.Play(thunderSFX, base.gameObject);
		_enemies.AddRange(Object.FindObjectsOfType(typeof(Health)).Cast<Health>());
		for (int i = _enemies.Count - 1; i >= 0; i--)
		{
			Health health2 = _enemies[i];
			if (health2 != null && health2.isEnemy && _numTrollsHit < _numTrollsToHit)
			{
				StopTroll(health2);
				_numTrollsHit++;
			}
			else
			{
				_enemies.RemoveAt(i);
			}
		}
		GameManager.KillAllProjectiles();
		StartCoroutine(SpawnDummyAnvils(_numTrollsToHit / 2, 10f));
		StartCoroutine(SpawnDummyAnvils(_numTrollsToHit, 5f));
		StartCoroutine(SpawnDummyAnvils(_numTrollsToHit / 2, 2f));
		yield return new WaitForSeconds(0.25f);
		GameManager.KillAllProjectiles();
		foreach (Health health in _enemies)
		{
			StartCoroutine(DelayKillTroll(health));
		}
		yield return new WaitForSeconds(2f);
		CleanupAnvils();
		AnimationUtils.PlayClip(_cloud.GetComponent<Animation>(), "Anvil_Clouds_Outro");
		yield return new WaitForSeconds(_cloud.GetComponent<Animation>()["Anvil_Clouds_Outro"].length);
		Object.Destroy(_cloudInstance);
		IsActive = false;
		yield return new WaitForSeconds(0.25f);
		DestroyAndFinish(true);
	}

	private void StopTroll(Health health)
	{
		GameObject gameObject = health.gameObject;
		if (gameObject.GetComponent<Mover>() != null)
		{
			gameObject.GetComponent<Mover>().StopMoving();
		}
		if (gameObject.GetComponent<MoverPingPong>() != null)
		{
			gameObject.GetComponent<MoverPingPong>().StopMoving();
		}
		if (gameObject.GetComponent<MoverOrbit>() != null)
		{
			gameObject.GetComponent<MoverOrbit>().StopMoving();
		}
	}

	private IEnumerator DelayKillTroll(Health health)
	{
		yield return new WaitForSeconds(Random.Range(0f, 0.5f));
		if (health != null)
		{
			Vector3 anvilRot = new Vector3(270f, 90f, 0f);
			if (Random.value > 0.5f)
			{
				anvilRot.y += 180f;
			}
			Anvil tempAnvil = (Object.Instantiate(anvil, health.transform.position + new Vector3(0f, 4f, 0f), Quaternion.Euler(anvilRot)) as GameObject).GetComponent<Anvil>();
			tempAnvil.GetComponent<TrailRenderer>().enabled = true;
			_anvils.Add(tempAnvil);
			health.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);
			StartCoroutine(DelayKill(health, tempAnvil));
		}
	}

	private IEnumerator SpawnDummyAnvils(int count, float distanceFromCamera)
	{
		for (int i = 0; i < count; i++)
		{
			yield return new WaitForSeconds(Random.Range(0f, 0.5f));
			Vector3 newPosition = Camera.main.transform.position + new Vector3(0f, 5f, 0f - distanceFromCamera);
			newPosition.x += Random.Range(-3f, 3f);
			Vector3 anvilRot = new Vector3(270f, 90f, 0f);
			if (Random.value > 0.5f)
			{
				anvilRot.y += 180f;
			}
			Anvil anvy = (Object.Instantiate(anvil, newPosition, Quaternion.Euler(anvilRot)) as GameObject).GetComponent<Anvil>();
			anvy.GetComponent<Collider>().enabled = false;
			_anvils.Add(anvy);
		}
	}

	private IEnumerator DelayKill(Health aHealth, Anvil aAnvil)
	{
		float killTime = Time.time + 0.5f;
		if (aHealth != null && aHealth.isEnemy)
		{
			while (!aAnvil.hasBounced && killTime >= Time.time)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (aHealth != null)
			{
				aHealth.TakeHit(damageInfo);
				if (!aAnvil.hasBounced)
				{
					aAnvil.PlayCollideSFX();
				}
			}
		}
		else
		{
			Debug.Log("health is " + aHealth);
			Debug.Log("enemy is " + aHealth.isEnemy);
		}
	}
}
