using UnityEngine;

public class PlayerMoveAroundScene : MonoBehaviour
{
	private Vector3 oldPos;

	private Vector3 spot;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
	}

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		MoveToFingerPosition(fingerPos);
	}

	public void MoveToFingerPosition(Vector2 aFingerPos)
	{
		Vector3 worldPos = GetWorldPos(aFingerPos);
		worldPos.x += 0.2f;
		Vector3 position = new Vector3(worldPos.x, -4f, Camera.main.transform.position.z - 3f);
		base.transform.position = position;
	}

	public static Vector3 GetWorldPos(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		float distance = (0f - ray.origin.z) / ray.direction.z;
		return ray.GetPoint(distance);
	}
}
