using UnityEngine;

public class MoverPingPong : MonoBehaviour
{
	public int X;

	public int Y;

	public int Z;

	private Vector3 direction;

	public float speed = 2f;

	public float xRangeHalved = 2f;

	private float maxXpos;

	private float minXpos;

	public float yRangeHalved = 2f;

	private float maxYpos;

	private float minYpos;

	public float pauseTime = 1f;

	public float shootingPauseTime = 3f;

	private bool pause;

	private float timer;

	private int goUp = 1;

	private bool doOnce;

	private EnemyStationaryShooter shooter;

	public bool fireThruMessage;

	private void Start()
	{
		maxXpos = base.transform.position.x + xRangeHalved;
		minXpos = base.transform.position.x - xRangeHalved;
		maxYpos = base.transform.position.y;
		minYpos = base.transform.position.y - 2f * yRangeHalved;
		direction = new Vector3(X, Y, Z);
		if (fireThruMessage)
		{
			shooter = GetComponent<EnemyStationaryShooter>();
		}
	}

	private void Update()
	{
		if (!doOnce)
		{
			maxXpos = base.transform.position.x + xRangeHalved;
			minXpos = base.transform.position.x - xRangeHalved;
			maxYpos = base.transform.position.y;
			minYpos = base.transform.position.y - 2f * yRangeHalved;
			direction = new Vector3(X, Y, Z);
			doOnce = true;
		}
		move();
	}

	private void move()
	{
		if (!pause)
		{
			Vector3 vector = new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, direction.z * speed * Time.deltaTime);
			base.transform.position += vector;
		}
		if (speed != 0f)
		{
			checkOnScreen();
		}
	}

	public void StopMoving()
	{
		speed = 0f;
	}

	private void checkOnScreen()
	{
		if (direction.y != 0f)
		{
			if (goUp == 1 && base.transform.position.y >= maxYpos && !pause)
			{
				pause = true;
				timer = Time.time + pauseTime;
				goUp = 0;
			}
			if (goUp == 0 && base.transform.position.y <= minYpos && !pause)
			{
				pause = true;
				timer = Time.time + shootingPauseTime;
				if (fireThruMessage && !GameManager.Instance.IsGameOver && !HealingElixirScreen.IsActive)
				{
					shooter.FireProjectileNow();
				}
				goUp = 1;
			}
			if (pause)
			{
				waitY();
			}
		}
		if (direction.x != 0f)
		{
			if (goUp == 1 && base.transform.position.x >= maxXpos && !pause)
			{
				pause = true;
				timer = Time.time + pauseTime;
				goUp = 0;
			}
			if (goUp == 0 && base.transform.position.x <= minXpos && !pause)
			{
				pause = true;
				timer = Time.time + pauseTime;
				goUp = 1;
			}
			if (pause)
			{
				waitX();
			}
		}
	}

	private void waitY()
	{
		if (Time.time >= timer && !GameManager.Instance.IsGameOver && !HealingElixirScreen.IsActive)
		{
			direction.y = 0f - direction.y;
			pause = false;
			if (fireThruMessage && goUp == 0)
			{
				SoundEventManager.Instance.Play(shooter.SFX_Copter, base.gameObject);
			}
		}
	}

	private void waitX()
	{
		if (Time.time >= timer && !GameManager.Instance.IsGameOver && !HealingElixirScreen.IsActive)
		{
			direction.x = 0f - direction.x;
			pause = false;
		}
	}

	public void Disable()
	{
		StopMoving();
		CancelInvoke();
		StopAllCoroutines();
	}
}
