using System.Collections;
using UnityEngine;

public class MoverBounce : MonoBehaviour
{
	public float stationaryDelay = 0.2f;

	public float rejumpDelay = 0.1f;

	public Vector3 forceWhenAtRest = Vector3.one;

	private int health;

	private Health myHealth;

	public GameObject sheepWool;

	public Color woolColor;

	public Color burnColor;

	private ParticleSystem smokeFX;

	private float lastJumpTime;

	private float stationaryTime;

	private AnimationStates anim;

	public bool jumpOnStart;

	public SoundEventData jumpSFX;

	public bool sheepMover;

	private Mover sheepMoving;

	private float mySpeed;

	private void Start()
	{
		myHealth = GetComponent<Health>();
		if (myHealth != null)
		{
			health = myHealth.hitPoints;
		}
		anim = GetComponentInChildren<AnimationStates>();
		if (anim == null)
		{
		}
		if (jumpOnStart)
		{
			base.GetComponent<Rigidbody>().AddForce(forceWhenAtRest, ForceMode.Impulse);
			stationaryTime = Time.time + stationaryDelay;
			if (jumpSFX != null)
			{
				SoundEventManager.Instance.Play(jumpSFX, base.gameObject);
			}
			if (anim != null)
			{
				StartCoroutine(anim.Sheep_Jump());
			}
		}
		if (sheepMover)
		{
			sheepMoving = base.gameObject.GetComponent<Mover>();
			mySpeed = sheepMoving.speed;
		}
		stationaryTime = Time.time + 1f;
	}

	private void Update()
	{
		if (myHealth != null)
		{
			if (health > myHealth.hitPoints)
			{
				health = myHealth.hitPoints;
				if (anim != null)
				{
					anim.TakeHit();
				}
				base.GetComponent<Rigidbody>().AddForce(forceWhenAtRest / 2f, ForceMode.Impulse);
			}
			else if (health <= 0)
			{
				BurnSheep();
			}
		}
		if (base.GetComponent<Rigidbody>().velocity.y >= 0.02f || base.GetComponent<Rigidbody>().velocity.y <= -0.02f)
		{
			stationaryTime = Time.time + stationaryDelay;
			if (sheepMover)
			{
				sheepMoving.speed = mySpeed;
			}
		}
		else if (stationaryTime <= Time.time)
		{
			StartCoroutine(Bounce());
			if (jumpSFX != null && forceWhenAtRest != new Vector3(0f, 0f, 0f))
			{
				SoundEventManager.Instance.Play(jumpSFX, base.gameObject);
			}
			if (anim != null)
			{
				StartCoroutine(anim.Sheep_Jump());
			}
		}
		if (sheepMover && base.GetComponent<Rigidbody>().velocity.y <= 0.02f && base.GetComponent<Rigidbody>().velocity.y >= -0.02f)
		{
			sheepMoving.speed = 0f;
		}
	}

	private IEnumerator Bounce()
	{
		if (lastJumpTime <= Time.time)
		{
			lastJumpTime = Time.time + rejumpDelay;
			base.GetComponent<Rigidbody>().AddForce(forceWhenAtRest, ForceMode.Impulse);
		}
		yield return 0;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (anim != null)
		{
			anim.Idle();
		}
	}

	private void BurnSheep()
	{
		anim.TakeHit();
		Material[] materials = sheepWool.GetComponent<Renderer>().materials;
		foreach (Material material in materials)
		{
			material.SetColor("_Color", burnColor);
		}
	}
}
