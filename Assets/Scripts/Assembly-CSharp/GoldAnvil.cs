using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoldAnvil : MonoBehaviour
{
	public SoundEventData anvilCollisionSound;

	private DamageInfo _damageInfo;

	private List<Health> _enemies;

	private void OnEnable()
	{
		_damageInfo = new DamageInfo();
		_damageInfo.comboNum = 10;
		_damageInfo.damageAmount = 100;
		_damageInfo.damageType = DamageTypes.anvil;
		_enemies = new List<Health>();
		_enemies.AddRange(Object.FindObjectsOfType(typeof(Health)).Cast<Health>());
	}

	private void OnDisable()
	{
		_enemies.Clear();
	}

	private void Impact()
	{
		GameManager.CameraShake();
		PlayCollideSFX();
		Object.Destroy(base.gameObject, 1f);
	}

	public void PlayCollideSFX()
	{
		SoundEventManager.Instance.Play(anvilCollisionSound, base.gameObject);
	}

	private void Update()
	{
		foreach (Health enemy in _enemies)
		{
			if (!(enemy == null) && base.GetComponent<Collider>().bounds.Contains(enemy.GetComponent<Collider>().bounds.max))
			{
				Smash(enemy);
			}
		}
	}

	private void Smash(Health aHealth)
	{
		if (aHealth != null && aHealth.isEnemy)
		{
			Debug.Log("Smash " + aHealth.name);
			aHealth.isDeflecting = false;
			aHealth.isForceFielded = false;
			aHealth.TakeHit(_damageInfo);
		}
	}
}
