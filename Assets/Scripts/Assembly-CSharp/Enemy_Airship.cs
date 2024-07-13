using UnityEngine;

public class Enemy_Airship : MonoBehaviour
{
	public Transform killEffect;

	public Vector3 direction = new Vector3(1f, 0f, 0f).normalized;

	public float speed = 2f;

	public Transform cannonBall;

	public int numCannonBalls = 2;

	public float fireTimer = 0.5f;

	public float waitTimer = 1f;

	public Transform[] cannons;

	private float timer;

	private bool isMoving;

	private bool cannonActive;

	private int activeCannonBalls;

	private void Start()
	{
	}

	private void Update()
	{
		if ((double)base.transform.position.x >= 0.1 && !isMoving)
		{
			Idle();
		}
		else
		{
			MoveShip();
		}
	}

	private void MoveShip()
	{
		Vector3 vector = new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, direction.z * speed * Time.deltaTime);
		base.transform.position += vector;
		timer = Time.realtimeSinceStartup;
	}

	private void Idle()
	{
		if (activeCannonBalls != numCannonBalls)
		{
			speed = 0f;
			cannonActive = true;
			if (Time.realtimeSinceStartup >= timer + waitTimer && cannonActive)
			{
				CannonIdle();
			}
			return;
		}
		cannonActive = false;
		if (Time.realtimeSinceStartup >= timer + waitTimer)
		{
			isMoving = true;
			speed = 2f;
			MoveShip();
		}
	}

	private void CannonIdle()
	{
		if (Time.realtimeSinceStartup >= timer + fireTimer)
		{
			FireCannon();
		}
		else
		{
			Idle();
		}
	}

	private void FireCannon()
	{
		Transform transform = Object.Instantiate(killEffect, cannons[activeCannonBalls].transform.position, Quaternion.identity) as Transform;
		Object.Instantiate(cannonBall, cannons[activeCannonBalls].transform.position, Quaternion.identity);
		Object.Destroy(transform.gameObject, 0.5f);
		activeCannonBalls++;
		timer = Time.realtimeSinceStartup;
	}

	private void TakeHit(float damage)
	{
		Kill();
	}

	private void Kill()
	{
		Transform transform = Object.Instantiate(killEffect, base.transform.position, Quaternion.identity) as Transform;
		Object.Destroy(base.gameObject);
		Object.Destroy(transform.gameObject, 1f);
		GameManager.EnemyKilled();
	}
}
