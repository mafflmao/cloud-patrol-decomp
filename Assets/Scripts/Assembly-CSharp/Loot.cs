using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
	public Transform killEffect;

	public bool amIMoney;

	public bool amIGem;

	private static int moneyAmount = 1;

	public SoundEventData Fuel_SFX_Pickup;

	public SoundEventData Fuel_SFX_Collect;

	public float takeHitTimeDelay = 0.2f;

	private float takeHitTime;

	public float aliveTime = 3f;

	public float burstHorzSpeed = 50f;

	public float burstUpForce = 80f;

	public bool autoCollect = true;

	public float autoCollectTime = 0.2f;

	public Animation childAnimation;

	public float maxDistance;

	public float maxSpeed;

	private Vector3 manualMovementDestination;

	private float mySpeed;

	private float randomNum;

	public int numGems = 1;

	private float _autoCollectTimeRemaining;

	public float collectionTravelTime;

	public Transform destinationPoint;

	private Vector3 startingPosition;

	private bool doCollectionMovement;

	public bool IsCollected
	{
		get
		{
			return doCollectionMovement;
		}
	}

	public event EventHandler Looted;

	public static event EventHandler<CancellableEventArgs> Timeout;

	public static event EventHandler Collected;

	private void Awake()
	{
		if (aliveTime > 0f)
		{
			InvokeHelper.InvokeSafe(DestroyAfterTimeout, aliveTime, this);
		}
	}

	private void Start()
	{
		destinationPoint = ShipManager.instance.moneyDestinationPoint;
		if (aliveTime > 0f)
		{
			InvokeHelper.InvokeSafe(DestroyAfterTimeout, aliveTime, this);
		}
		randomNum = UnityEngine.Random.Range(0f - burstHorzSpeed, burstHorzSpeed);
		if (amIMoney)
		{
			base.transform.Rotate(UnityEngine.Random.onUnitSphere * 360f);
			GameManager.sessionStats.singleCoinsSpawned++;
		}
		takeHitTime = Time.time + takeHitTimeDelay;
		if (base.GetComponent<Rigidbody>() != null)
		{
			float num = UnityEngine.Random.Range(1f, 1.4f);
			Vector3 vector = Vector3.up * burstUpForce * num;
			Vector3 vector2 = Vector3.left * randomNum;
			Vector3 vector3 = Vector3.forward * (randomNum / 10f);
			Vector3 force = vector + vector2 + vector3;
			base.transform.position += base.GetComponent<Collider>().bounds.extents.y * Vector3.up;
			base.GetComponent<Rigidbody>().isKinematic = false;
			base.GetComponent<Rigidbody>().useGravity = true;
			base.GetComponent<Rigidbody>().AddForce(force);
		}
		else
		{
			manualMovementDestination = base.transform.position + Vector3.Scale(UnityEngine.Random.onUnitSphere, new Vector3(maxDistance, maxDistance, maxDistance));
			mySpeed = Mathf.Clamp(UnityEngine.Random.value * maxSpeed, maxSpeed / 1.5f, maxSpeed);
		}
		if (autoCollect)
		{
			InvokeHelper.InvokeSafe(Collect, autoCollectTime, this);
		}
		if (childAnimation != null)
		{
			InvokeHelper.InvokeSafe(PlayChildAnimation, UnityEngine.Random.value * 0.2f, this);
		}
	}

	private void PlayChildAnimation()
	{
		childAnimation.Play();
	}

	private void LateUpdate()
	{
		if (doCollectionMovement)
		{
			if (_autoCollectTimeRemaining >= 0f)
			{
				float t = Mathf.Clamp01(_autoCollectTimeRemaining / collectionTravelTime);
				base.transform.position = Vector3.Lerp(destinationPoint.position, startingPosition, t);
				_autoCollectTimeRemaining -= Time.deltaTime;
			}
			else
			{
				Kill();
			}
		}
		else if (base.GetComponent<Rigidbody>() == null)
		{
			manualMovementDestination = Vector3.MoveTowards(manualMovementDestination, destinationPoint.position, maxSpeed * Time.deltaTime * 60f);
			base.transform.position = Vector3.MoveTowards(base.transform.position, manualMovementDestination, mySpeed * Time.deltaTime * 60f);
		}
	}

	public void TakeHit(DamageInfo myDamageInfo)
	{
		if (!doCollectionMovement && Time.time >= takeHitTime)
		{
			Collect();
		}
	}

	public void TakeHit()
	{
		if (!doCollectionMovement && Time.time >= takeHitTime)
		{
			Collect();
		}
	}

	public void Collect()
	{
		if (base.GetComponent<Rigidbody>() != null)
		{
			base.GetComponent<Rigidbody>().isKinematic = true;
			base.GetComponent<Rigidbody>().useGravity = false;
			base.GetComponent<Rigidbody>().detectCollisions = false;
		}
		if (amIMoney)
		{
			destinationPoint = ShipManager.instance.moneyDestinationPoint;
			TrailRenderer component = GetComponent<TrailRenderer>();
			component.enabled = true;
			_autoCollectTimeRemaining = collectionTravelTime;
			doCollectionMovement = true;
			GameManager.sessionStats.singleCoinsCollected++;
		}
		else if (amIGem)
		{
			doCollectionMovement = false;
			Kill();
		}
		startingPosition = base.transform.position;
		if (!autoCollect)
		{
			SoundEventManager.Instance.Play(Fuel_SFX_Pickup, base.gameObject);
		}
		OnLooted();
		OnCollected();
	}

	public void Kill()
	{
		if (killEffect != null)
		{
			Transform transform = UnityEngine.Object.Instantiate(killEffect, base.transform.position, Quaternion.identity) as Transform;
			transform.parent = ShipManager.instance.enemyProjectileTarget;
		}
		if (amIMoney)
		{
			GameManager.GotMoney(moneyAmount);
			SoundEventManager.Instance.Play(Fuel_SFX_Collect, destinationPoint.gameObject);
		}
		if (amIGem)
		{
			if (!RankDataManager.Instance.TryAwardGameplayGem(numGems))
			{
				Debug.LogError("Player attempted to collect more gems than is allowed this run. Gem collection prevented.");
			}
			SoundEventManager.Instance.Play(Fuel_SFX_Collect, base.gameObject);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void DestroyAfterTimeout()
	{
		CancellableEventArgs cancellableEventArgs = new CancellableEventArgs();
		OnTimeout(cancellableEventArgs);
		if (!cancellableEventArgs.IsCancelled)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnLooted()
	{
		if (this.Looted != null)
		{
			this.Looted(this, new EventArgs());
		}
	}

	private void OnTimeout(CancellableEventArgs args)
	{
		if (Loot.Timeout != null)
		{
			Loot.Timeout(this, args);
		}
	}

	private void OnCollected()
	{
		if (Loot.Collected != null)
		{
			Loot.Collected(this, new EventArgs());
		}
	}
}
