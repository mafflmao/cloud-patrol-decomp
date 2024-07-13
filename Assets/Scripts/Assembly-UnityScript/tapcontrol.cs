using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class tapcontrol : MonoBehaviour
{
	public GameObject cameraObject;

	public Transform cameraPivot;

	public Texture jumpButton;

	public float speed;

	public float jumpSpeed;

	public float inAirMultiplier;

	public float minimumDistanceToMove;

	public float minimumTimeUntilMove;

	public bool zoomEnabled;

	public float zoomEpsilon;

	public float zoomRate;

	public bool rotateEnabled;

	public float rotateEpsilon;

	private ZoomCamera zoomCamera;

	private Camera cam;

	private Transform thisTransform;

	private CharacterController character;

	private Vector3 targetLocation;

	private bool moving;

	private float rotationTarget;

	private float rotationVelocity;

	private Vector3 velocity;

	private ControlState state;

	private int[] fingerDown;

	private Vector2[] fingerDownPosition;

	private int[] fingerDownFrame;

	private float firstTouchTime;

	public tapcontrol()
	{
		inAirMultiplier = 0.25f;
		minimumDistanceToMove = 1f;
		minimumTimeUntilMove = 0.25f;
		rotateEpsilon = 1f;
		state = ControlState.WaitingForFirstTouch;
		fingerDown = new int[2];
		fingerDownPosition = new Vector2[2];
		fingerDownFrame = new int[2];
	}

	public virtual void Start()
	{
		thisTransform = transform;
		zoomCamera = (ZoomCamera)cameraObject.GetComponent(typeof(ZoomCamera));
		cam = cameraObject.GetComponent<Camera>();
		character = (CharacterController)GetComponent(typeof(CharacterController));
		ResetControlState();
		GameObject gameObject = GameObject.Find("PlayerSpawn");
		if ((bool)gameObject)
		{
			thisTransform.position = gameObject.transform.position;
		}
	}

	public virtual void OnEndGame()
	{
		enabled = false;
	}

	public virtual void FaceMovementDirection()
	{
		Vector3 vector = character.velocity;
		vector.y = 0f;
		if (!(vector.magnitude <= 0.1f))
		{
			thisTransform.forward = vector.normalized;
		}
	}

	public virtual void CameraControl(Touch touch0, Touch touch1)
	{
		if (rotateEnabled && state == ControlState.RotatingCamera)
		{
			Vector2 vector = touch1.position - touch0.position;
			Vector2 lhs = vector / vector.magnitude;
			Vector2 vector2 = touch1.position - touch1.deltaPosition - (touch0.position - touch0.deltaPosition);
			Vector2 rhs = vector2 / vector2.magnitude;
			float num = Vector2.Dot(lhs, rhs);
			if (!(num >= 1f))
			{
				Vector3 lhs2 = new Vector3(vector.x, vector.y);
				Vector3 rhs2 = new Vector3(vector2.x, vector2.y);
				float z = Vector3.Cross(lhs2, rhs2).normalized.z;
				float num2 = Mathf.Acos(num);
				rotationTarget += num2 * 57.29578f * z;
				if (!(rotationTarget >= 0f))
				{
					rotationTarget += 360f;
				}
				else if (!(rotationTarget < 360f))
				{
					rotationTarget -= 360f;
				}
			}
		}
		else if (zoomEnabled && state == ControlState.ZoomingCamera)
		{
			float magnitude = (touch1.position - touch0.position).magnitude;
			float magnitude2 = (touch1.position - touch1.deltaPosition - (touch0.position - touch0.deltaPosition)).magnitude;
			float num3 = magnitude - magnitude2;
			zoomCamera.zoom += num3 * zoomRate * Time.deltaTime;
		}
	}

	public virtual void CharacterControl()
	{

	}

	public virtual void ResetControlState()
	{
		state = ControlState.WaitingForFirstTouch;
		fingerDown[0] = -1;
		fingerDown[1] = -1;
	}

	public virtual void Update()
	{
		int touchCount = Input.touchCount;
		if (touchCount == 0)
		{
			ResetControlState();
		}
		else
		{
			int num = default(int);
			Touch touch = default(Touch);
			Touch[] touches = Input.touches;
			Touch touch2 = default(Touch);
			Touch touch3 = default(Touch);
			bool flag = false;
			bool flag2 = false;
			if (state == ControlState.WaitingForFirstTouch)
			{
				for (num = 0; num < touchCount; num++)
				{
					touch = touches[num];
					if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
					{
						state = ControlState.WaitingForSecondTouch;
						firstTouchTime = Time.time;
						fingerDown[0] = touch.fingerId;
						fingerDownPosition[0] = touch.position;
						fingerDownFrame[0] = Time.frameCount;
						break;
					}
				}
			}
			if (state == ControlState.WaitingForSecondTouch)
			{
				for (num = 0; num < touchCount; num++)
				{
					touch = touches[num];
					if (touch.phase == TouchPhase.Canceled)
					{
						continue;
					}
					if (touchCount >= 2 && touch.fingerId != fingerDown[0])
					{
						state = ControlState.WaitingForMovement;
						fingerDown[1] = touch.fingerId;
						fingerDownPosition[1] = touch.position;
						fingerDownFrame[1] = Time.frameCount;
						break;
					}
					if (touchCount == 1)
					{
						Vector2 vector = touch.position - fingerDownPosition[0];
						if (touch.fingerId == fingerDown[0] && (Time.time > firstTouchTime + minimumTimeUntilMove || touch.phase == TouchPhase.Ended))
						{
							state = ControlState.MovingCharacter;
							break;
						}
					}
				}
			}
			if (state == ControlState.WaitingForMovement)
			{
				for (num = 0; num < touchCount; num++)
				{
					touch = touches[num];
					if (touch.phase == TouchPhase.Began)
					{
						if (touch.fingerId == fingerDown[0] && fingerDownFrame[0] == Time.frameCount)
						{
							touch2 = touch;
							flag = true;
						}
						else if (touch.fingerId != fingerDown[0] && touch.fingerId != fingerDown[1])
						{
							fingerDown[1] = touch.fingerId;
							touch3 = touch;
							flag2 = true;
						}
					}
					if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended)
					{
						if (touch.fingerId == fingerDown[0])
						{
							touch2 = touch;
							flag = true;
						}
						else if (touch.fingerId == fingerDown[1])
						{
							touch3 = touch;
							flag2 = true;
						}
					}
				}
				if (flag)
				{
					if (flag2)
					{
						Vector2 vector2 = fingerDownPosition[1] - fingerDownPosition[0];
						Vector2 vector3 = touch3.position - touch2.position;
						Vector2 lhs = vector2 / vector2.magnitude;
						Vector2 rhs = vector3 / vector3.magnitude;
						float num2 = Vector2.Dot(lhs, rhs);
						if (!(num2 >= 1f))
						{
							float num3 = Mathf.Acos(num2);
							if (!(num3 <= rotateEpsilon * ((float)Math.PI / 180f)))
							{
								state = ControlState.RotatingCamera;
							}
						}
						if (state == ControlState.WaitingForMovement)
						{
							float f = vector2.magnitude - vector3.magnitude;
							if (!(Mathf.Abs(f) <= zoomEpsilon))
							{
								state = ControlState.ZoomingCamera;
							}
						}
					}
				}
				else
				{
					state = ControlState.WaitingForNoFingers;
				}
			}
			if (state == ControlState.RotatingCamera || state == ControlState.ZoomingCamera)
			{
				for (num = 0; num < touchCount; num++)
				{
					touch = touches[num];
					if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended)
					{
						if (touch.fingerId == fingerDown[0])
						{
							touch2 = touch;
							flag = true;
						}
						else if (touch.fingerId == fingerDown[1])
						{
							touch3 = touch;
							flag2 = true;
						}
					}
				}
				if (flag)
				{
					if (flag2)
					{
						CameraControl(touch2, touch3);
					}
				}
				else
				{
					state = ControlState.WaitingForNoFingers;
				}
			}
		}
		CharacterControl();
	}

	public virtual void LateUpdate()
	{
		float y = Mathf.SmoothDampAngle(cameraPivot.eulerAngles.y, rotationTarget, ref rotationVelocity, 0.3f);
		Vector3 eulerAngles = cameraPivot.eulerAngles;
		float num = (eulerAngles.y = y);
		Vector3 vector2 = (cameraPivot.eulerAngles = eulerAngles);
	}

	public virtual void Main()
	{
	}
}
