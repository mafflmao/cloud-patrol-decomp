using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoldAnvilRain : Powerup
{
	public GameObject anvil;

	public GameObject cloudPrefab;

	public static bool IsActive;

	private GameObject _cloudInstance;

	private GameObject _cloud;

	private PowerupCutscene powerupCutscene;

	private DamageInfo damageInfo;

	private GameObject _anvil;

	private List<Health> _enemies;

	public SoundEventData thunderSFX;

	protected void Start()
	{
		IsActive = true;
		_enemies = new List<Health>();
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
	}

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		StartCoroutine(TriggerPowerCoroutine());
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		CleanupAnvil();
	}

	private void CleanupAnvil()
	{
		foreach (Health enemy in _enemies)
		{
			DelayKill(enemy, 0f);
		}
		if (_anvil != null)
		{
			Object.Destroy(_anvil);
		}
		_enemies.Clear();
	}

	private IEnumerator TriggerPowerCoroutine()
	{
		_enemies.Clear();
		SoundEventManager.Instance.Play(thunderSFX, base.gameObject);
		_enemies.AddRange(Object.FindObjectsOfType(typeof(Health)).Cast<Health>());
		for (int i = _enemies.Count - 1; i >= 0; i--)
		{
			Health health2 = _enemies[i];
			if (health2 != null && health2.isEnemy)
			{
				StopAndCowerTroll(health2);
			}
			else
			{
				_enemies.RemoveAt(i);
			}
		}
		GameManager.KillAllProjectiles();
		GameManager.ExplodeBombs();
		yield return new WaitForSeconds(0.25f);
		SpawnBigAnvil();
		foreach (Health health in _enemies)
		{
			StartCoroutine(DelayKillTroll(health));
		}
		yield return new WaitForSeconds(2f);
		CleanupAnvil();
		AnimationUtils.PlayClip(_cloud.GetComponent<Animation>(), "Anvil_Clouds_Outro");
		yield return new WaitForSeconds(_cloud.GetComponent<Animation>()["Anvil_Clouds_Outro"].length);
		Object.Destroy(_cloudInstance);
		IsActive = false;
		yield return new WaitForSeconds(0.25f);
		DestroyAndFinish(true);
	}

	private void StopAndCowerTroll(Health health)
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
		gameObject.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);
		AnimationStates component = gameObject.GetComponent<AnimationStates>();
		if (component != null)
		{
			component.Cover();
		}
	}

	private void SpawnBigAnvil()
	{
		Vector3 euler = new Vector3(270f, 90f, 0f);
		if (Random.value > 0.5f)
		{
			euler.y += 180f;
		}
		_anvil = Object.Instantiate(anvil, Camera.main.transform.position, Quaternion.Euler(euler)) as GameObject;
		_anvil.transform.parent = Camera.main.transform;
		_anvil.transform.localPosition = new Vector3(0f, 0f, 5f);
	}

	private IEnumerator DelayKillTroll(Health health)
	{
		yield return new WaitForSeconds(Random.Range(0f, 0.5f));
		if (health != null)
		{
			health.SendMessage("Disable", SendMessageOptions.DontRequireReceiver);
			StartCoroutine(DelayKill(health, 0.5f));
		}
	}

	private IEnumerator DelayKill(Health aHealth, float delay)
	{
		float killTime = Time.time + delay;
		if (aHealth != null && aHealth.isEnemy)
		{
			while (killTime >= Time.time)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (aHealth != null)
			{
				aHealth.isDeflecting = false;
				aHealth.isForceFielded = false;
				aHealth.TakeHit(damageInfo);
			}
		}
	}
}
