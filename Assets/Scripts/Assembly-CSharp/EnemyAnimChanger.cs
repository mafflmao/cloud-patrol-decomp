using UnityEngine;

public class EnemyAnimChanger : MonoBehaviour
{
	public Texture[] myTextures;

	private float timer;

	public int state;

	public float initWaitTime = 2f;

	public SoundEventData SFX_Telegraph_Shoot_Fire;

	public SoundEventData VO_Taunt;

	public SoundEventData SFX_Attack_Shoot_Fire;

	public Flipbook fireFX;

	private float peekOutDistance = 0.4f;

	private bool doOnce;

	public bool peekABoo;

	public bool isTurtle;

	private bool inPosition;

	private int state0;

	private int state1;

	private int state2;

	private int state0_5;

	private float tempTime;

	private float oldPos;

	private void Start()
	{
		base.GetComponent<Renderer>().material.mainTexture = myTextures[0];
		if (peekABoo)
		{
			base.gameObject.layer = Layers.EnemiesDontTarget;
		}
		if (!isTurtle)
		{
			state0 = 0;
			state1 = 1;
			state2 = 2;
		}
		else
		{
			state0 = 0;
			state0_5 = 1;
			state1 = 2;
			state2 = 3;
			base.gameObject.layer = Layers.EnemiesDontTarget;
		}
	}

	private void OnEnable()
	{
		state = 0;
	}

	private void Update()
	{
		if (!(timer <= Time.time))
		{
			return;
		}
		if (state == 0)
		{
			base.GetComponent<Renderer>().material.mainTexture = myTextures[state0];
			timer = Time.time + initWaitTime;
			if (!isTurtle)
			{
				state = 1;
				return;
			}
			base.gameObject.layer = Layers.EnemiesDontTarget;
			state = -1;
		}
		else if (state == 1)
		{
			if (peekABoo && !inPosition)
			{
				if (!doOnce)
				{
					oldPos = base.transform.position.y;
					tempTime = 0f;
					doOnce = true;
				}
				MoveUp();
				return;
			}
			SoundEventManager.Instance.Play(SFX_Telegraph_Shoot_Fire, base.gameObject);
			SoundEventManager.Instance.Play(VO_Taunt, base.gameObject);
			base.GetComponent<Renderer>().material.mainTexture = myTextures[state1];
			timer = Time.time + 1.5f;
			if (fireFX != null)
			{
				Flipbook component = fireFX.GetComponent<Flipbook>();
				component.Play();
			}
			state = 2;
		}
		else if (state == 2)
		{
			SoundEventManager.Instance.Play(SFX_Attack_Shoot_Fire, base.gameObject);
			GameManager.HurtPlayer(2f);
			GameManager.CameraShake();
			base.GetComponent<Renderer>().material.mainTexture = myTextures[state2];
			timer = Time.time + 0.5f;
			state = 3;
		}
		else if (state == 3)
		{
			base.GetComponent<Renderer>().material.mainTexture = myTextures[state1];
			timer = Time.time + 0.5f;
			state = 4;
		}
		else if (state == 4)
		{
			base.GetComponent<Renderer>().material.mainTexture = myTextures[state0];
			if (peekABoo && inPosition)
			{
				if (!doOnce)
				{
					oldPos = base.transform.position.y;
					tempTime = 0f;
					doOnce = true;
				}
				MoveDown();
			}
			else if (isTurtle)
			{
				state = -2;
			}
			else
			{
				state = 0;
			}
		}
		else if (state == -1)
		{
			base.GetComponent<Renderer>().material.mainTexture = myTextures[state0_5];
			timer = Time.time + 0.2f;
			base.gameObject.layer = Layers.Enemies;
			state = 1;
		}
		else if (state == -2)
		{
			base.GetComponent<Renderer>().material.mainTexture = myTextures[state0_5];
			timer = Time.time + 0.2f;
			state = 0;
		}
	}

	private void MoveUp()
	{
		if (base.transform.position.y < oldPos + peekOutDistance)
		{
			if (Time.time > tempTime)
			{
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.08f, base.transform.position.z);
				tempTime = Time.time + 0.001f;
			}
		}
		else
		{
			base.gameObject.layer = Layers.Enemies;
			inPosition = true;
			doOnce = false;
			timer = Time.time + 0.5f;
		}
	}

	private void MoveDown()
	{
		if (base.transform.position.y > oldPos - peekOutDistance)
		{
			if (Time.time > tempTime)
			{
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 0.08f, base.transform.position.z);
				tempTime = Time.time + 0.001f;
			}
		}
		else
		{
			base.gameObject.layer = Layers.EnemiesDontTarget;
			inPosition = false;
			doOnce = false;
			state = 0;
		}
	}
}
