using UnityEngine;

public class Accessory : SafeMonoBehaviour
{
	public Transform accessory;

	public string accessoryBone = "bLHand";

	public Vector3 accessoryOffset = new Vector3(0f, 0f, 0f);

	public Vector3 pivotOffset = new Vector3(0f, 0f, 0f);

	public float PercentChanceToSpawn = 1f;

	public GameObject poofPrefab

	public bool useRotate;

	public Vector3 accessoryRotation = new Vector3(0f, 0f, 0f);

	public bool usedForFlight;

	public void SwapAccessory(GameObject prefab)
	{
		GameObject gameObject = Object.Instantiate(prefab) as GameObject;
		gameObject.transform.parent = accessory.transform.parent;
		gameObject.transform.localPosition = accessory.transform.localPosition;
		gameObject.transform.localRotation = accessory.transform.localRotation;
		gameObject.transform.localScale = accessory.transform.localScale;
		Transform transform = accessory;
		accessory = gameObject.transform;
		Object.Destroy(transform.gameObject);
	}

	private void OnEnable()
	{
		if ((bool)accessory && Random.value <= PercentChanceToSpawn)
		{
			Transform transform = TransformUtil.FindRecursive(base.transform, accessoryBone);
			if ((bool)transform)
			{
				accessory.parent = transform;
				accessory.localPosition = accessoryOffset;
				if (useRotate)
				{
					accessory.localRotation = Quaternion.Euler(accessoryRotation);
				}
			}
			else
			{
				Debug.LogError("No bone '" + accessoryBone + "' found!");
			}
		}
		else if ((bool)accessory)
		{
			Object.Destroy(accessory.gameObject);
		}
	}

	public void ShowAccessory()
	{
		accessory.GetComponent<Renderer>().enabled = true;
	}

	public void HideAccessory()
	{
		accessory.GetComponent<Renderer>().enabled = false;
	}

	public void Detach()
	{
		if (base.transform.parent != null && base.transform.parent.parent != null && (bool)accessory)
		{
			accessory.parent = base.transform.parent.parent;
			if (accessory.gameObject.GetComponent<Rigidbody>() == null)
			{
				accessory.gameObject.AddComponent<Rigidbody>();
			}
			if (accessory.gameObject.GetComponent<Collider>() == null)
			{
				BoxCollider boxCollider = accessory.gameObject.AddComponent<BoxCollider>();
				boxCollider.center += pivotOffset;
				boxCollider.enabled = false;
			}
			else
			{
				accessory.gameObject.GetComponent<Collider>().enabled = false;
			}
			accessory.GetComponent<Rigidbody>().AddForce(new Vector3(100f, 150f, 150f));
			InvokeHelper.InvokeSafe(SpinAccessory, 0.1f, this);
			InvokeHelper.InvokeSafe(DestroyAccessory, 2f, this);
		}
	}

	private void SpinAccessory()
	{
		if (accessory != null)
		{
			accessory.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 100f);
		}
	}

	private void OnDestroy()
	{
		CancelInvoke();
		StopAllCoroutines();
		DestroyAccessory();
	}

	private void DestroyAccessory()
	{
		if (!accessory)
		{
			return;
		}
		if (!SafeMonoBehaviour.IsShuttingDown)
		{
			if ((bool)poofPrefab && accessory.GetComponent<Collider>() != null)
			{
				Object.Instantiate(poofPrefab, accessory.GetComponent<Collider>().bounds.center, accessory.rotation);
			}
			SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.EnemyPoof, base.gameObject);
		}
		Object.Destroy(accessory.gameObject);
	}
}
