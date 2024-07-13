using System.Collections;
using UnityEngine;

public class EnemyWizard : MonoBehaviour
{
	public float timeBeforeDodge = 2f;

	public float timeDodging = 0.5f;

	public Transform teleportLocation;

	private bool warpedYet;

	public Transform dodgeVFX;

	public SoundEventData dodgeSFX;

	public SoundEventData dodgeVO;

	public SoundEventData reappearSFX;

	private Vector3 startPos;

	private bool reappear;

	private float timer;

	private AnimationStates myAnim;

	private Accessory[] accessories;

	public Transform projectile;

	private TargetQueue mTargetQueue;

	private void OnEnable()
	{
		mTargetQueue = null;
		myAnim = GetComponent<AnimationStates>();
		if (myAnim != null)
		{
			myAnim.Wizard_Idle();
			myAnim.Offset(Random.Range(0f, 0.5f));
		}
		startPos = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		base.gameObject.layer = Layers.EnemiesDontTarget;
		accessories = GetComponents<Accessory>();
		Accessory[] array = accessories;
		foreach (Accessory accessory in array)
		{
			accessory.HideAccessory();
			Debug.Log("Hiding Accessory");
		}
		timer = Time.time + timeBeforeDodge;
	}

	private void OnDisable()
	{
		CancelInvoke();
		StopAllCoroutines();
	}

	private void Update()
	{
		if ((timer < Time.time) & !reappear)
		{
			reappear = true;
			Reappear();
		}
	}

	public void Selected(TargetQueue i_TargetQueue)
	{
		mTargetQueue = i_TargetQueue;
		myAnim.Wizard_Ready();
		StartCoroutine(Teleport());
	}

	private IEnumerator Teleport()
	{
		if (dodgeSFX != null)
		{
			SoundEventManager.Instance.Play(dodgeSFX, base.gameObject);
		}
		if (dodgeVO != null)
		{
			SoundEventManager.Instance.Play(dodgeVO, base.gameObject);
		}
		if (teleportLocation == null || warpedYet)
		{
			yield return new WaitForSeconds(1f);
		}
		else
		{
			yield return new WaitForSeconds(0.2f);
			if (projectile != null)
			{
				Transform proj = (Transform)Object.Instantiate(projectile, base.transform.position, base.transform.rotation);
				proj.parent = base.transform.parent;
				if ((bool)proj.GetComponent<ArcProjectile>())
				{
					proj.GetComponent<ArcProjectile>().SetShooter(base.gameObject.name);
				}
			}
		}
		warpedYet = true;
		if (mTargetQueue != null)
		{
			mTargetQueue.RemoveGameObject(base.gameObject);
			mTargetQueue = null;
		}
		base.gameObject.layer = Layers.EnemiesDontTarget;
		if (dodgeVFX != null)
		{
			Object.Instantiate(dodgeVFX, base.transform.position + new Vector3(0f, base.GetComponent<Collider>().bounds.extents.y, 0f), base.transform.rotation);
		}
		Accessory[] array = accessories;
		foreach (Accessory acc in array)
		{
			acc.HideAccessory();
			Debug.Log("Hiding Accessory");
		}
		GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		yield return new WaitForSeconds(timeDodging);
		if (teleportLocation != null)
		{
			if (base.transform.position == teleportLocation.position)
			{
				base.transform.position = startPos;
			}
			else
			{
				base.transform.position = teleportLocation.position;
			}
		}
		Reappear();
	}

	private void Reappear()
	{
		base.gameObject.layer = Layers.Enemies;
		GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
		if (dodgeVFX != null)
		{
			Object.Instantiate(dodgeVFX, base.transform.position + new Vector3(0f, base.GetComponent<Collider>().bounds.extents.y, 0f), base.transform.rotation);
		}
		Accessory[] array = accessories;
		foreach (Accessory accessory in array)
		{
			accessory.ShowAccessory();
			Debug.Log("Showing Accessory");
		}
		StartCoroutine(myAnim.Wizard_Appear());
		SoundEventManager.Instance.Play(reappearSFX, base.gameObject);
	}

	public void Disable()
	{
		StopAllCoroutines();
		CancelInvoke();
	}

	public void VictoryDance()
	{
		StopAllCoroutines();
		CancelInvoke();
	}

	public void SpawnPoof()
	{
		if (dodgeVFX != null)
		{
			Object.Instantiate(dodgeVFX, base.transform.position + new Vector3(0f, base.GetComponent<Collider>().bounds.extents.y, 0f), base.transform.rotation);
		}
	}
}
