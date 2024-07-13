using System;
using System.Linq;
using UnityEngine;

public class TrackingCrosshair : MonoBehaviour
{
	public Texture normalTexture;

	public Texture maxTexture;

	public Texture lockTexture;

	public Animation anim;

	public MeshRenderer meshRenderer;

	public float baseSize = 7.358263f;

	private Collider targetCollider;

	private MonoBehaviour[] _components;

	public TargetQueue Owner { get; set; }

	public static event EventHandler ReadyForRecycle;

	private void Awake()
	{
		_components = base.gameObject.GetComponentsInChildren<MonoBehaviour>().ToArray();
	}

	private void Start()
	{
		ResetStateAndDisable();
	}

	public void Update()
	{
		UpdatePositionBasedOnCameraAndTarget();
	}

	private void UpdatePositionBasedOnCameraAndTarget()
	{
		if (targetCollider != null)
		{
			base.transform.position = targetCollider.bounds.center;
		}
		base.transform.LookAt(Camera.main.transform, Vector3.down);
		float num = Vector3.Distance(Camera.main.transform.position, base.transform.position);
		float num2 = num / baseSize;
		base.transform.localScale = Vector3.Scale(Vector3.one, new Vector3(num2, num2, num2));
	}

	public void BeginTarget(GameObject target)
	{
		MonoBehaviour[] components = _components;
		foreach (MonoBehaviour monoBehaviour in components)
		{
			monoBehaviour.enabled = true;
		}
		meshRenderer.enabled = true;
		UpdatePositionBasedOnCameraAndTarget();
		anim.Play("TrackingCrosshairBegin", PlayMode.StopAll);
		if (target != null)
		{
			targetCollider = target.GetComponent<Collider>();
		}
	}

	public void MaxOut()
	{
		anim.gameObject.GetComponent<Renderer>().material.mainTexture = maxTexture;
		anim.Stop();
		anim.Play("TrackingCrosshairMaxout", PlayMode.StopAll);
	}

	public void UnMaxOut()
	{
		anim.gameObject.GetComponent<Renderer>().material.mainTexture = normalTexture;
	}

	public void RemoveTarget()
	{
		Debug.Log("RemoveTarget");
		anim.Play("TrackingCrosshairShot", PlayMode.StopAll);
		base.transform.parent = null;
		InvokeHelper.InvokeSafe(OnReadyForRecycle, 0.5f, this);
	}

	public void TargetLock()
	{
		anim.Play("TrackingCrosshairShot", PlayMode.StopAll);
		anim.gameObject.GetComponent<Renderer>().material.mainTexture = lockTexture;
		base.transform.parent = null;
		InvokeHelper.InvokeSafe(OnReadyForRecycle, 0.5f, this);
	}

	private void ResetStateAndDisable()
	{
		anim.Stop();
		anim.gameObject.GetComponent<Renderer>().material.mainTexture = normalTexture;
		targetCollider = null;
		meshRenderer.enabled = false;
		MonoBehaviour[] components = _components;
		foreach (MonoBehaviour monoBehaviour in components)
		{
			monoBehaviour.enabled = false;
		}
	}

	private void OnReadyForRecycle()
	{
		ResetStateAndDisable();
		base.transform.parent = null;
		if (TrackingCrosshair.ReadyForRecycle != null)
		{
			TrackingCrosshair.ReadyForRecycle(this, new EventArgs());
		}
	}
}
