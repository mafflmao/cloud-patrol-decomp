using UnityEngine;

public class Spin : MonoBehaviour
{
	public float rotSpeed = 250f;

	private float rot;

	public float momentum = 0.95f;

	private float lastrot;

	public bool doSpin;

	public Camera myCamera;

	private Vector2 fingerPosition;

	private void OnEnable()
	{
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
		FingerGestures.OnDragMove += FingerGestures_OnDragMove;
	}

	private void OnDisable()
	{
		FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
	}

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		Ray ray = myCamera.ScreenPointToRay(fingerPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider == base.GetComponent<Collider>())
		{
			fingerPosition = fingerPos;
			lastrot = fingerPos.x;
			doSpin = true;
		}
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float fingerDownTime)
	{
		doSpin = false;
	}

	private void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
	{
		fingerPosition = fingerPos;
	}

	public void LateUpdate()
	{
		if (doSpin)
		{
			rot = (fingerPosition.x - lastrot) * rotSpeed;
		}
		else
		{
			rot *= momentum;
		}
		lastrot = fingerPosition.x;
		base.transform.Rotate(base.transform.up, 0f - rot);
	}
}
