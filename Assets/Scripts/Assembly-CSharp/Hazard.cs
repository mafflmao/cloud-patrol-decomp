using System;
using UnityEngine;

public class Hazard : MonoBehaviour
{
	public float damage = 1f;

	public float damageRate = 0.5f;

	public bool selfDestructOnDamage;

	public ScoreData.ScoreType scoreType;

	[HideInInspector]
	public string originatingGameObject = string.Empty;

	public Transform poofFX;

	public Transform explodeFX;

	public bool _isActive = true;

	public bool _canDamage = true;

	public Vector3 lootOffset = new Vector3(0f, 0f, 0.2f);

	public GameObject fuelDrop;

	public SoundEventData sfxExplode;

	public bool isActive
	{
		get
		{
			return _isActive;
		}
	}

	public static event EventHandler<EventArgs> HazardHurtPlayer;

	public static event EventHandler HazardDefused;

	public virtual void ApplyDamage()
	{
		if (!_isActive || !_canDamage)
		{
			return;
		}
		_canDamage = false;
		if (SkyIronShield.ActiveShield != null)
		{
			SkyIronShield.ActiveShield.SpawnShield(base.transform.position, true);
			DefusedHazard(true);
		}
		else
		{
			if (GameManager.sessionStats.deathAI == "None")
			{
				if (originatingGameObject != string.Empty)
				{
					GameManager.sessionStats.deathAI = originatingGameObject;
				}
				else
				{
					GameManager.sessionStats.deathAI = base.gameObject.name;
				}
				GameManager.sessionStats.deathType = "Hazard";
				GameManager.sessionStats.deathScreenLocation = Camera.main.WorldToScreenPoint(base.transform.position);
			}
			OnHazardHurtPlayer();
			if (base.gameObject.name.Contains("Projectile_Explosion"))
			{
				GameManager.projsHitInVoyage++;
				HealthBar.Instance.TriggerEvent(HealthBar.HealthBarEvent.ProjHit);
				ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.PROJECTILEHIT, base.transform.position, true);
			}
			else
			{
				GameManager.redBombsHitInVoyage++;
				HealthBar.Instance.TriggerEvent(HealthBar.HealthBarEvent.BombHit);
				ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.BOMBMALUS, base.transform.position, true);
			}
			GameManager.bombsHitInVoyage++;
			GameManager.HurtPlayer(damage);
			AchievementManager.Instance.IncrementStep(Achievements.DieBombs);
		}
		if (selfDestructOnDamage)
		{
			Explode();
		}
		else
		{
			InvokeHelper.InvokeSafe(HazardCanDamage, damageRate, this);
		}
	}

	protected void OnHazardHurtPlayer()
	{
		if (Hazard.HazardHurtPlayer != null)
		{
			Hazard.HazardHurtPlayer(this, new EventArgs());
		}
	}

	public void DefusedHazard(bool useMultiplierForScore)
	{
		ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.BOMB, base.transform.position, useMultiplierForScore);
		if (Hazard.HazardDefused != null)
		{
			Hazard.HazardDefused(this, new EventArgs());
		}
	}

	public void DropLoot()
	{
		if ((bool)poofFX && (bool)base.gameObject)
		{
			UnityEngine.Object.Instantiate(poofFX, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
		}
		if ((bool)fuelDrop && (bool)base.gameObject)
		{
			UnityEngine.Object.Instantiate(fuelDrop, base.transform.position + lootOffset, Quaternion.Euler(90f, 0f, 0f));
		}
	}

	private void HazardCanDamage()
	{
		_canDamage = true;
	}

	public void SetActive(bool active)
	{
		_isActive = active;
	}

	public void Explode()
	{
		Explode(false);
	}

	public void Explode(bool destroy)
	{
		if ((bool)sfxExplode)
		{
			SoundEventManager.Instance.Play(sfxExplode, base.gameObject);
		}
		if ((bool)explodeFX)
		{
			UnityEngine.Object.Instantiate(explodeFX, base.transform.position, Quaternion.identity);
		}
		if (destroy)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Cleanup()
	{
		SpawnPoof();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SpawnPoof()
	{
		if ((bool)poofFX)
		{
			UnityEngine.Object.Instantiate(poofFX, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
		}
	}
}
