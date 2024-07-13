using UnityEngine;

public class OneFingerGestureSample : SampleBase
{
	public GameObject longPressObject;

	public GameObject tapObject;

	public GameObject swipeObject;

	public GameObject dragObject;

	public int requiredTapCount = 2;

	protected override string GetHelpText()
	{
		return "This sample demonstrates some of the supported single-finger gestures:\r\n\r\n- Drag: press the red sphere and move your finger to drag it around  \r\n\r\n- LongPress: keep your finger pressed on the cyan sphere for at least " + FingerGestures.Instance.longPressDuration + " seconds\r\n\r\n- Tap: rapidly press & release the purple sphere " + requiredTapCount + " times\r\n\r\n- Swipe: press the yellow sphere and move your finger in one of the four cardinal directions, then release. The speed of the motion is taken into account.";
	}

	private void OnEnable()
	{
		Debug.Log("Registering finger gesture events from C# script");
		FingerGestures.OnLongPress += FingerGestures_OnLongPress;
		FingerGestures.OnTap += FingerGestures_OnTap;
		FingerGestures.OnSwipe += FingerGestures_OnSwipe;
		FingerGestures.OnDragBegin += FingerGestures_OnDragBegin;
		FingerGestures.OnDragMove += FingerGestures_OnDragMove;
		FingerGestures.OnDragEnd += FingerGestures_OnDragEnd;
	}

	private void OnDisable()
	{
		FingerGestures.OnLongPress -= FingerGestures_OnLongPress;
		FingerGestures.OnTap -= FingerGestures_OnTap;
		FingerGestures.OnSwipe -= FingerGestures_OnSwipe;
		FingerGestures.OnDragBegin -= FingerGestures_OnDragBegin;
		FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
		FingerGestures.OnDragEnd -= FingerGestures_OnDragEnd;
	}

	private void FingerGestures_OnLongPress(Vector2 fingerPos)
	{
		if (CheckSpawnParticles(fingerPos, longPressObject))
		{
			base.UI.StatusText = "Performed a long-press with finger ";
		}
	}

	private void FingerGestures_OnTap(Vector2 fingerPos)
	{
		if (CheckSpawnParticles(fingerPos, tapObject))
		{
			base.UI.StatusText = "Tapped " + requiredTapCount + " times with finger ";
		}
	}

	private void FingerGestures_OnSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
		GameObject gameObject = SampleBase.PickObject(startPos);
		if (gameObject == swipeObject)
		{
			base.UI.StatusText = string.Concat("Swiped ", direction, " with finger ");
			Vector3 forward;
			switch (direction)
			{
			case FingerGestures.SwipeDirection.Up:
				forward = Vector3.up;
				break;
			case FingerGestures.SwipeDirection.Down:
				forward = Vector3.down;
				break;
			case FingerGestures.SwipeDirection.Right:
				forward = Vector3.right;
				break;
			default:
				forward = Vector3.left;
				break;
			}
		}
	}

	private void FingerGestures_OnDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
		GameObject gameObject = SampleBase.PickObject(startPos);
		if (gameObject == dragObject)
		{
			base.UI.StatusText = "Started dragging with finger ";
			SpawnParticles(gameObject);
		}
	}

	private void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
	{
		dragObject.transform.position = SampleBase.GetWorldPos(fingerPos);
	}

	private void FingerGestures_OnDragEnd(Vector2 fingerPos)
	{
		base.UI.StatusText = "Stopped dragging with finger ";
		SpawnParticles(dragObject);
	}

	private bool CheckSpawnParticles(Vector2 fingerPos, GameObject requiredObject)
	{
		GameObject gameObject = SampleBase.PickObject(fingerPos);
		if (!gameObject || gameObject != requiredObject)
		{
			return false;
		}
		SpawnParticles(gameObject);
		return true;
	}

	private void SpawnParticles(GameObject obj)
	{

	}
}
