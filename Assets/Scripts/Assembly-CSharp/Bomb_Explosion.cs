using UnityEngine;

public class Bomb_Explosion : MonoBehaviour
{
	public float Force = 5f;

	public Vector3 Spin = Vector3.up;

	public bool ExplodeOnStart;

	public bool ExplodeOnEnable = true;

	public Transform[] transforms;

	private Vector3[] positions;

	private Quaternion[] rotations;

	public ParticleSystem particles;

	public bool sendApplyDamageMessage;

	private void Awake()
	{
		if (transforms.Length > 0)
		{
			positions = new Vector3[transforms.Length];
			rotations = new Quaternion[transforms.Length];
			for (int i = 0; i < transforms.Length; i++)
			{
				positions[i] = transforms[i].localPosition;
				rotations[i] = transforms[i].localRotation;
			}
		}
	}

	private void Start()
	{
		if (ExplodeOnStart)
		{
			BombExplode();
		}
	}

	private void OnEnable()
	{
		if (ExplodeOnEnable)
		{
			BombExplode();
		}
	}

	private void BombExplode()
	{
		if (sendApplyDamageMessage)
		{
			SendMessageUpwards("ApplyDamage", SendMessageOptions.DontRequireReceiver);
		}
		if (particles != null)
		{
			particles.Play();
        }
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < transforms.Length; i++)
		{
			Transform transform = transforms[i];
			if (transform.GetComponent<Rigidbody>() == null)
			{
				transform.gameObject.AddComponent<Rigidbody>();
			}
			if (transform.GetComponent<Collider>() == null)
			{
				transform.gameObject.AddComponent<SphereCollider>();
			}
			if (transform.GetComponent<Renderer>() != null)
			{
				transform.GetComponent<Renderer>().enabled = true;
			}
			if (transform != base.transform && transform.GetComponent<Rigidbody>() != null)
			{
				transform.localPosition = positions[i];
				transform.localRotation = rotations[i];
				zero = ((!(transform.GetComponent<Collider>() != null)) ? (transform.transform.position - base.transform.position) : (transform.GetComponent<Collider>().bounds.center - base.transform.position));
				transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
				transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				transform.GetComponent<Rigidbody>().detectCollisions = false;
				transform.GetComponent<Rigidbody>().AddForce(zero.normalized * Force);
				transform.GetComponent<Rigidbody>().AddTorque(Spin);
			}
		}
	}
}
