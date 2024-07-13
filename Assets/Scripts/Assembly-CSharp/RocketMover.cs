using UnityEngine;

public class RocketMover : MonoBehaviour
{
	private const float _accelerationTime = 3f;

	private Vector3 _maxSpeed = new Vector3(-30f, 0f, 0f);

	public bool stopAtArrival;

	private Vector3 _loadedRoomPosition;

	private float _startTime;

	private bool _moving = true;

	private bool _raisedEventForCurrentLevel;

	private float RoomWidth = 9f;

	private void Start()
	{
		_startTime = Time.time;
		if (PlatformUtils.IsLowQualityPlatform)
		{
			Debug.Log("Slowing down rocket... We're on a crappy platform.");
			_maxSpeed = new Vector3(-14f, 0f, 0f);
		}
	}

	private void Update()
	{
		if (_moving)
		{
			float num = (Time.time - _startTime) / 3f;
			if (num > 1f)
			{
				num = 1f;
			}
			Vector3 vector = Vector3.Lerp(Vector3.zero, _maxSpeed, num);
			base.transform.Translate(vector * Time.deltaTime);
			if (stopAtArrival)
			{
				iTween.MoveTo(base.gameObject, iTween.Hash("position", _loadedRoomPosition, "time", 2f, "oncompletetarget", base.gameObject, "oncomplete", "FinishMove"));
				_moving = false;
			}
			else if (base.transform.position.x < _loadedRoomPosition.x && !_raisedEventForCurrentLevel)
			{
				_raisedEventForCurrentLevel = true;
				MoverWithSpeed.OnMoveCompleteHack(this);
			}
		}
	}

	public void RoomFinishedLoading(LevelManager.NextRoomEventArgs args)
	{
		if (base.transform.position.x < args.CameraPosition.x + RoomWidth)
		{
			base.transform.position = new Vector3(args.CameraPosition.x + RoomWidth, base.transform.position.y, base.transform.position.z);
		}
		_loadedRoomPosition = args.CameraPosition;
		_raisedEventForCurrentLevel = false;
	}

	public void FinishMove()
	{
		MoverWithSpeed.OnMoveCompleteHack(this);
		Object.Destroy(this);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(_loadedRoomPosition, RoomWidth);
	}
}
