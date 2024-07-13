using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotmachineBox : MonoBehaviour
{
	public enum SlotFace
	{
		coins = 0,
		blast = 1,
		elemental_ticket = 2
	}

	private const string animOpen = "slotbox_Open";

	private const string animDisappear = "slotbox_Disappear";

	public Animation boxAnimation;

	public Flipbook slotFlipbook;

	public List<ParticleSystem> confetti;

	public GameObject coinSpawn;

	public int numCoins = 5;

	public GameObject bulletSpawn;

	public GameObject elementalSpawn;

	[HideInInspector]
	public SlotFace slotFace;

	private GameObject _gem;

	private GameObject _bullet;

	private Vector3 _spawnLoc;

	private bool _notHit = true;

	private void OnEnable()
	{
		Health.TookHit += TookHitHandler;
		LevelManager.MovingToNextRoom += LeavingRoom;
		_spawnLoc = base.GetComponent<Collider>().bounds.center + new Vector3(0f, 0.5f, 0f);
	}

	private void OnDisable()
	{
		CleanupSlotbox();
	}

	private void OnDestroy()
	{
		CleanupSlotbox();
	}

	private void LeavingRoom(object obj, EventArgs args)
	{
		if ((bool)_bullet)
		{
			_bullet.GetComponent<ArcProjectile>().Explode();
		}
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(Disappear(0f));
		}
	}

	private void CleanupSlotbox()
	{
		Health.TookHit -= TookHitHandler;
		LevelManager.MovingToNextRoom -= LeavingRoom;
		if ((bool)_gem)
		{
			_gem.GetComponent<Loot>().Looted -= GemCollectedHandler;
			UnityEngine.Object.Destroy(_gem);
		}
		CancelInvoke();
		StopAllCoroutines();
	}

	public void TookHitHandler(object obj, EventArgs args)
	{
		if (obj == base.gameObject.GetComponent<Health>())
		{
			Health.TookHit -= TookHitHandler;
			LevelManager.MovingToNextRoom -= LeavingRoom;
			if (_notHit)
			{
				_notHit = false;
				base.gameObject.layer = Layers.EnemiesDontTarget;
				slotFlipbook.Stop();
				slotFace = (SlotFace)slotFlipbook.index;
				StartCoroutine(RewardSequence());
			}
		}
	}

	public IEnumerator RewardSequence()
	{
		boxAnimation.Play("slotbox_Open");
		yield return new WaitForSeconds(boxAnimation["slotbox_Open"].clip.length);
		if (slotFace == SlotFace.coins)
		{
			SpawnCoins();
		}
		else if (slotFace == SlotFace.blast)
		{
			SpawnBullets();
		}
		else if (slotFace == SlotFace.elemental_ticket)
		{
			SpawnGem();
		}
	}

	private IEnumerator Disappear(float delay)
	{
		yield return new WaitForSeconds(delay);
		boxAnimation.Play("slotbox_Disappear");
		yield return new WaitForSeconds(boxAnimation["slotbox_Disappear"].clip.length);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SpawnCoins()
	{
		for (int i = 0; i < numCoins; i++)
		{
			UnityEngine.Object.Instantiate(coinSpawn, _spawnLoc, Quaternion.identity);
		}
		StartCoroutine(Disappear(0.5f));
	}

	public void SpawnBullets()
	{
		_bullet = UnityEngine.Object.Instantiate(bulletSpawn, _spawnLoc + new Vector3(0f, 0f, 0.4f), Quaternion.identity) as GameObject;
		ScreenManager componentInChildren = LevelManager.Instance.currentScreenRoot.GetComponentInChildren<ScreenManager>();
		if ((bool)componentInChildren)
		{
			_bullet.transform.parent = componentInChildren.transform;
		}
		StartCoroutine(Disappear(0.5f));
	}

	private void SpawnGem()
	{
		_gem = UnityEngine.Object.Instantiate(elementalSpawn, _spawnLoc + new Vector3(0f, 0.25f, 0f), Quaternion.identity) as GameObject;
		_gem.transform.parent = base.transform;
		_gem.GetComponent<Loot>().Looted += GemCollectedHandler;
	}

	public void GemCollectedHandler(object obj, EventArgs args)
	{
		if ((bool)_gem && obj == _gem.GetComponent<Loot>())
		{
			_gem.GetComponent<Loot>().Looted -= GemCollectedHandler;
		}
		StartCoroutine(Disappear(0f));
	}
}
