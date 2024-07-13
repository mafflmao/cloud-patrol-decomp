using UnityEngine;

public class GhostSword : MonoBehaviour
{
	private const float frameRate = 60f;

	[HideInInspector]
	public bool isReversed;

	public float speed = 30f;

	public static Vector3 rotation;

	public float scale = 0.5f;

	public bool affectY = true;

	public bool affectZ = true;

	private bool _rotate;

	private DragMultiTarget _dragTarget;

	private static float _ySpeed;

	private static float _zSpeed;

	protected int m_DragMultiTargetIndex;

	public int DragMultiTargetIndex
	{
		set
		{
			m_DragMultiTargetIndex = value;
		}
	}

	private void Start()
	{
		SetVisible(false);
		_dragTarget = ShipManager.instance.dragMultiTarget[m_DragMultiTargetIndex];
		base.transform.parent = _dragTarget.transform;
		base.transform.localScale = base.transform.localScale * scale;
		SetSpeed();
	}

	private void LateUpdate()
	{
		_dragTarget = ShipManager.instance.dragMultiTarget[m_DragMultiTargetIndex];
		if (_rotate)
		{
			rotation = new Vector3(speed, _ySpeed, _zSpeed);
			base.transform.RotateAround(_dragTarget.transform.position, Vector3.forward, rotation.x * Time.deltaTime * 60f);
			base.transform.RotateAround(_dragTarget.transform.position, Vector3.up, rotation.y * Time.deltaTime * 60f);
			base.transform.RotateAround(_dragTarget.transform.position, Vector3.left, rotation.z * Time.deltaTime * 60f);
		}
	}

	public void StartRotating(float aSpeed)
	{
		speed = aSpeed;
		SetSpeed();
		if (isReversed)
		{
			base.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 180f));
			base.transform.localPosition = new Vector3(5f, 0f, 0f);
		}
		else
		{
			base.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
			base.transform.localPosition = new Vector3(-5f, 0f, 0f);
		}
		_rotate = true;
		SetVisible(true);
	}

	private void SetSpeed()
	{
		if (affectY)
		{
			_ySpeed = Random.Range(speed * 0.125f, speed * 0.25f);
		}
		if (affectZ)
		{
			_zSpeed = Random.Range(speed * 0.125f, speed * 0.25f);
		}
	}

	public void StopRotating()
	{
		_rotate = false;
		SetVisible(false);
	}

	public void Detach()
	{
		Transform transform = base.transform;
		StopRotating();
		transform.parent = null;
		SetVisible(true);
		base.gameObject.AddComponent<SphereCollider>();
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		rigidbody.detectCollisions = false;
		rigidbody.centerOfMass = Vector3.zero;
		rigidbody.AddTorque((Random.value * 10f + 25f) * Vector3.forward);
		rigidbody.AddForce((transform.position - _dragTarget.transform.position) * 1000f, ForceMode.Force);
		Object.Destroy(base.gameObject, 2f);
	}

	private void SetVisible(bool isVisible)
	{
		if (base.GetComponent<Renderer>() != null)
		{
			base.GetComponent<Renderer>().enabled = isVisible;
		}
		else
		{
			GetComponentInChildren<MeshRenderer>().enabled = isVisible;
		}
	}
}
