using UnityEngine;

public class SheepCopter : MonoBehaviour
{
	private Health myHealth;

	private Mover myMove;

	private MoverPingPong myPingPong;

	private AnimationStates myAnim;

	private float _liftAmount;

	public float _liftAmountLow = 1f;

	public float _liftAmountHigh = 2f;

	private float _initialTimeOffset;

	private float _frequencyFactor;

	private float _amplitudeFactor;

	private int previousHP;

	private int currentHP;

	public bool copterDude = true;

	public GameObject sheepWool;

	public Color woolColor;

	public Color burnColor;

	private ParticleSystem smokeFX;

	public SoundEventData SFX_copter;

	public GameObject flyingAccessory;

	private Health accessoryHealth;

	private bool ableToFall;

	private void Start()
	{
		Object.Destroy(base.gameObject);
		myHealth = GetComponent<Health>();
		myAnim = GetComponent<AnimationStates>();
		previousHP = myHealth.hitPoints;
		currentHP = myHealth.hitPoints;
		smokeFX = GetComponentInChildren<ParticleSystem>();
		if (myAnim != null)
		{
			myAnim.Fly();
		}
		if (copterDude)
		{
			SoundEventManager.Instance.Play(SFX_copter, base.gameObject, 1f);
		}
		_amplitudeFactor = Random.Range(0.001f, 0.01f);
		_frequencyFactor = Random.Range(1f, 1.5f);
		_initialTimeOffset = 0f;
		Random.Range(0f, 1f);
		_liftAmount = Random.Range(_liftAmountLow, _liftAmountHigh);
		if (flyingAccessory != null)
		{
			ableToFall = true;
		}
	}

	private void Update()
	{
		if (ableToFall && flyingAccessory == null)
		{
			FallingSheep();
		}
		else
		{
			float x = Mathf.Sin((Time.time + _initialTimeOffset) * _frequencyFactor) * _amplitudeFactor;
			float y = _liftAmount * Time.deltaTime;
			Vector3 vector = new Vector3(x, y, 0f);
			base.GetComponent<Rigidbody>().MovePosition(base.transform.position + vector);
		}
		previousHP = myHealth.hitPoints;
		if (previousHP != currentHP)
		{
			currentHP = previousHP;
		}
		else if (currentHP <= 0)
		{
			BurnSheep();
		}
	}

	private void FallingSheep()
	{
		base.GetComponent<Rigidbody>().useGravity = true;
		myAnim.Fall();
	}

	private void BurnSheep()
	{
		Material[] materials = sheepWool.GetComponent<Renderer>().materials;
		foreach (Material material in materials)
		{
			material.SetColor("_Color", burnColor);
		}
	}

	public void Disable()
	{
		CancelInvoke();
		StopAllCoroutines();
		if (ableToFall)
		{
			InvokeHelper.InvokeSafe(PopBalloon, 0.2f, this);
		}
	}

	private void PopBalloon()
	{
		if (flyingAccessory != null)
		{
			Health component = flyingAccessory.GetComponent<Health>();
			if (component != null)
			{
				component.Kill();
			}
		}
	}
}
