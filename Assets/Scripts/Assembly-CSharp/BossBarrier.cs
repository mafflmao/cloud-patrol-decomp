using UnityEngine;

public class BossBarrier : MonoBehaviour
{
	public Transform[] bossRubble;

	public Transform[] bossDebris;

	public Transform explosionFX;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Explode()
	{
		if ((bool)explosionFX)
		{
			Object.Instantiate(explosionFX, base.transform.position + new Vector3(0f, 0.5f, 0.5f), Quaternion.Euler(270f, 0f, 0f));
		}
		if (bossRubble.Length > 0)
		{
			int num = Random.Range(0, bossRubble.Length);
			Transform transform = (Transform)Object.Instantiate(bossRubble[num], base.transform.position, Quaternion.Euler(270f, 0f, 0f));
			transform.transform.parent = base.transform.parent;
		}
		if (bossDebris.Length > 0)
		{
			for (int i = 0; i < bossDebris.Length; i++)
			{
				Transform transform2 = (Transform)Object.Instantiate(bossDebris[i], base.transform.position, Quaternion.identity);
				transform2.transform.parent = base.transform.parent;
				BoxCollider component = transform2.GetComponent<BoxCollider>();
				if (component == null)
				{
					transform2.gameObject.AddComponent<BoxCollider>();
					component = transform2.GetComponent<BoxCollider>();
				}
				if (transform2.gameObject.GetComponent<Rigidbody>() == null)
				{
					transform2.gameObject.AddComponent<Rigidbody>();
				}
				transform2.GetComponent<Rigidbody>().mass = 2f;
				transform2.GetComponent<Rigidbody>().useGravity = true;
				transform2.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 800f);
				transform2.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 4225f);
			}
		}
		Object.Destroy(base.gameObject);
	}
}
