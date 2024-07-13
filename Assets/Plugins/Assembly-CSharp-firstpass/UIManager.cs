using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("EZ GUI/Management/UI Manager")]
public class UIManager : MonoBehaviour
{
	public enum POINTER_TYPE
	{
		MOUSE = 0,
		TOUCHPAD = 1,
		AUTO_TOUCHPAD = 2,
		RAY = 3,
		MOUSE_AND_RAY = 4,
		TOUCHPAD_AND_RAY = 5
	}

	public enum RAY_ACTIVE_STATE
	{
		Inactive = 0,
		Momentary = 1,
		Constant = 2
	}

	public enum OUTSIDE_VIEWPORT
	{
		Process_All = 0,
		Ignore = 1,
		Move_Off = 2
	}

	public struct NonUIHitInfo
	{
		public int ptrIndex;

		public int camIndex;

		public NonUIHitInfo(int pIndex, int cIndex)
		{
			ptrIndex = pIndex;
			camIndex = cIndex;
		}
	}

	public class BlockInputChangingEventArgs : EventArgs
	{
		public bool IsCancelled { get; private set; }

		public void Cancel()
		{
			IsCancelled = true;
		}
	}

	public delegate void PointerPollerDelegate();

	public delegate void PointerInfoDelegate(POINTER_INFO ptr);

	private static UIManager s_Instance;

	public POINTER_TYPE pointerType = POINTER_TYPE.AUTO_TOUCHPAD;

	public float dragThreshold = 8f;

	public float rayDragThreshold = 2f;

	public float rayDepth = float.PositiveInfinity;

	public LayerMask rayMask = -1;

	public bool focusWithRay;

	public string actionAxis = "Fire1";

	public OUTSIDE_VIEWPORT inputOutsideViewport = OUTSIDE_VIEWPORT.Move_Off;

	public bool warnOnNonUiHits = true;

	protected Transform raycastingTransform;

	public EZCameraSettings[] uiCameras = new EZCameraSettings[0];

	public Camera rayCamera;

	private bool _blockInput;

	public float defaultDragOffset = 1f;

	public EZAnimation.EASING_TYPE cancelDragEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	public float cancelDragDuration = 1f;

	public TextAsset defaultFont;

	public Material defaultFontMaterial;

	public bool autoRotateKeyboardPortrait = true;

	public bool autoRotateKeyboardPortraitUpsideDown = true;

	public bool autoRotateKeyboardLandscapeLeft = true;

	public bool autoRotateKeyboardLandscapeRight = true;

	protected bool rayActive;

	protected RAY_ACTIVE_STATE rayState;

	protected POINTER_INFO[,] pointers;

	protected NonUIHitInfo[] nonUIHits;

	protected bool[] usedPointers;

	protected bool[] usedNonUIHits;

	protected bool rayIsNonUIHit;

	protected int numPointers;

	protected int numTouchPointers;

	protected int[] activePointers;

	protected int numActivePointers;

	protected int numNonUIHits;

	protected POINTER_INFO rayPtr;

	protected PointerPollerDelegate pointerPoller;

	protected PointerInfoDelegate informNonUIHit;

	protected PointerInfoDelegate mouseTouchListeners;

	protected PointerInfoDelegate rayListeners;

	protected IUIObject focusObj;

	protected string controlText;

	protected int insert;

	private KEYBOARD_INFO kbInfo = default(KEYBOARD_INFO);

	protected int inputLockCount;

	protected bool m_started;

	protected bool m_awake;

	private float time;

	private float startTime;

	private float realtimeDelta;

	private int lastUpdateFrame;

	private int curActionID;

	private int numTouches;

	protected RaycastHit hit;

	protected Vector3 tempVec;

	private bool down;

	private IUIObject tempObj;

	private POINTER_INFO tempPtr;

	private StringBuilder sb = new StringBuilder();

	public List<EZCameraSettings> m_ListCameraSettings = new List<EZCameraSettings>();

	public static UIManager instance
	{
		get
		{
			if (s_Instance == null)
			{
				UIManager uIManager = UnityEngine.Object.FindObjectOfType(typeof(UIManager)) as UIManager;
				if (uIManager != null)
				{
					uIManager.Awake();
				}
				s_Instance = uIManager;
			}
			return s_Instance;
		}
	}

	public bool blockInput
	{
		get
		{
			return _blockInput;
		}
		set
		{
			if (value != _blockInput)
			{
				BlockInputChangingEventArgs blockInputChangingEventArgs = new BlockInputChangingEventArgs();
				OnBlockInputChanging(blockInputChangingEventArgs);
				if (!blockInputChangingEventArgs.IsCancelled)
				{
					_blockInput = value;
				}
			}
		}
	}

	public RAY_ACTIVE_STATE RayActive
	{
		get
		{
			return rayState;
		}
		set
		{
			rayState = value;
		}
	}

	public IUIObject FocusObject
	{
		get
		{
			return focusObj;
		}
		set
		{
			IUIObject iUIObject = ((value == null || !value.GotFocus()) ? null : value);
			if (focusObj != null && focusObj is IKeyFocusable)
			{
				((IKeyFocusable)focusObj).LostFocus();
			}
			focusObj = iUIObject;
			if (focusObj != null)
			{
				controlText = ((IKeyFocusable)focusObj).GetInputText(ref kbInfo);
				if (controlText == null)
				{
					controlText = string.Empty;
				}
				insert = kbInfo.insert;
				if (sb.Length > 0)
				{
					sb.Replace(sb.ToString(), controlText);
				}
				else
				{
					sb.Append(controlText);
				}
			}
		}
	}

	public int InsertionPoint
	{
		get
		{
			return insert;
		}
		set
		{
			insert = value;
		}
	}

	public int PointerCount
	{
		get
		{
			return numPointers;
		}
	}

	public static event EventHandler<BlockInputChangingEventArgs> BlockInputChanging;

	public static bool Exists()
	{
		if (s_Instance == null)
		{
			UIManager uIManager = UnityEngine.Object.FindObjectOfType(typeof(UIManager)) as UIManager;
			if (uIManager != null)
			{
				uIManager.Awake();
			}
			s_Instance = uIManager;
		}
		return s_Instance != null;
	}

	public void OnDestroy()
	{
		s_Instance = null;
		m_ListCameraSettings.Clear();
	}

	private void Awake()
	{
		Cursor.visible = false;
		if (m_awake)
		{
			return;
		}
		m_awake = true;
		if (s_Instance != null)
		{
			Debug.LogError("You can only have one instance of this singleton object in existence.");
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		s_Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (pointerType == POINTER_TYPE.TOUCHPAD || pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
		{
			numTouches = 11;
		}
		else if (pointerType == POINTER_TYPE.AUTO_TOUCHPAD)
		{
			numTouches = 12;
		}
		else if (pointerType == POINTER_TYPE.MOUSE_AND_RAY)
		{
			numTouches = 1;
		}
		else
		{
			numTouches = 1;
		}
		if (pointerType == POINTER_TYPE.AUTO_TOUCHPAD || pointerType == POINTER_TYPE.MOUSE || pointerType == POINTER_TYPE.MOUSE_AND_RAY)
		{
			numTouchPointers = numTouches - 1;
		}
		else
		{
			numTouchPointers = numTouches;
		}
		if (rayCamera == null && uiCameras.Length > 0)
		{
			rayCamera = uiCameras[0].camera;
		}
	}

	private void Start()
	{
		if (!m_started)
		{
			m_started = true;
			numPointers = numTouches;
			activePointers = new int[numTouches];
			usedPointers = new bool[numPointers];
			nonUIHits = new NonUIHitInfo[numTouches];
			usedNonUIHits = new bool[numPointers];
			numNonUIHits = 0;
			SetupPointers();
		}
	}

	protected void SetupPointers()
	{
		Start();
		pointers = new POINTER_INFO[uiCameras.Length, numTouches];
		if (uiCameras.Length == 0)
		{
			return;
		}
		if (rayCamera == null)
		{
			if (Camera.main != null)
			{
				raycastingTransform = Camera.main.transform;
			}
		}
		else
		{
			raycastingTransform = rayCamera.transform;
		}
		switch (pointerType)
		{
		case POINTER_TYPE.MOUSE:
		{
			pointerPoller = PollMouse;
			activePointers[0] = 0;
			numActivePointers = 1;
			for (int n = 0; n < uiCameras.Length; n++)
			{
				pointers[n, 0].id = 0;
				pointers[n, 0].rayDepth = uiCameras[n].rayDepth;
				pointers[n, 0].layerMask = uiCameras[n].mask;
				pointers[n, 0].camera = uiCameras[n].camera;
				pointers[n, 0].type = POINTER_INFO.POINTER_TYPE.MOUSE;
			}
			break;
		}
		case POINTER_TYPE.TOUCHPAD:
		{
			pointerPoller = PollTouchpad;
			for (int num = 0; num < uiCameras.Length; num++)
			{
				for (int num2 = 0; num2 < numPointers; num2++)
				{
					pointers[num, num2].id = num2;
					pointers[num, num2].rayDepth = uiCameras[num].rayDepth;
					pointers[num, num2].layerMask = uiCameras[num].mask;
					pointers[num, num2].camera = uiCameras[num].camera;
					pointers[num, num2].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
				}
			}
			break;
		}
		case POINTER_TYPE.AUTO_TOUCHPAD:
		{
			pointerPoller = PollMouseAndTouchpad;
			for (int k = 0; k < uiCameras.Length; k++)
			{
				for (int l = 0; l < numPointers; l++)
				{
					pointers[k, l].id = l;
					pointers[k, l].rayDepth = uiCameras[k].rayDepth;
					pointers[k, l].layerMask = uiCameras[k].mask;
					pointers[k, l].camera = uiCameras[k].camera;
					pointers[k, l].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
				}
				pointers[k, numPointers - 1].type = POINTER_INFO.POINTER_TYPE.MOUSE;
			}
			break;
		}
		case POINTER_TYPE.RAY:
			pointerPoller = PollRay;
			numActivePointers = 0;
			rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
			rayPtr.id = -1;
			rayPtr.rayDepth = rayDepth;
			rayPtr.layerMask = rayMask;
			rayPtr.camera = rayCamera;
			break;
		case POINTER_TYPE.MOUSE_AND_RAY:
		{
			pointerPoller = PollMouseRay;
			activePointers[0] = 0;
			numActivePointers = 1;
			for (int m = 0; m < uiCameras.Length; m++)
			{
				pointers[m, 0].id = 0;
				pointers[m, 0].rayDepth = uiCameras[m].rayDepth;
				pointers[m, 0].layerMask = uiCameras[m].mask;
				pointers[m, 0].camera = uiCameras[m].camera;
				pointers[m, 0].type = POINTER_INFO.POINTER_TYPE.MOUSE;
			}
			rayPtr.id = -1;
			rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
			rayPtr.rayDepth = rayDepth;
			rayPtr.layerMask = rayMask;
			rayPtr.camera = rayCamera;
			break;
		}
		case POINTER_TYPE.TOUCHPAD_AND_RAY:
		{
			pointerPoller = PollTouchpadRay;
			for (int i = 0; i < uiCameras.Length; i++)
			{
				for (int j = 0; j < numPointers; j++)
				{
					pointers[i, j].id = j;
					pointers[i, j].rayDepth = uiCameras[i].rayDepth;
					pointers[i, j].layerMask = uiCameras[i].mask;
					pointers[i, j].camera = uiCameras[i].camera;
					pointers[i, j].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
				}
			}
			rayPtr.id = -1;
			rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
			rayPtr.rayDepth = rayDepth;
			rayPtr.layerMask = rayMask;
			rayPtr.camera = rayCamera;
			break;
		}
		default:
			Debug.LogError("ERROR: Invalid pointer type selected!");
			break;
		}
	}

	public void SetNonUIHitDelegate(PointerInfoDelegate del)
	{
		informNonUIHit = del;
	}

	public void AddNonUIHitDelegate(PointerInfoDelegate del)
	{
		informNonUIHit = (PointerInfoDelegate)Delegate.Combine(informNonUIHit, del);
	}

	public void RemoveNonUIHitDelegate(PointerInfoDelegate del)
	{
		informNonUIHit = (PointerInfoDelegate)Delegate.Remove(informNonUIHit, del);
	}

	public void AddMouseTouchPtrListener(PointerInfoDelegate del)
	{
		mouseTouchListeners = (PointerInfoDelegate)Delegate.Combine(mouseTouchListeners, del);
	}

	public void AddRayPtrListener(PointerInfoDelegate del)
	{
		rayListeners = (PointerInfoDelegate)Delegate.Combine(rayListeners, del);
	}

	public void RemoveMouseTouchPtrListener(PointerInfoDelegate del)
	{
		mouseTouchListeners = (PointerInfoDelegate)Delegate.Remove(mouseTouchListeners, del);
	}

	public void RemoveRayPtrListener(PointerInfoDelegate del)
	{
		rayListeners = (PointerInfoDelegate)Delegate.Remove(rayListeners, del);
	}

	protected void AddNonUIHit(int ptrIndex, int camIndex)
	{
		if (informNonUIHit != null)
		{
			if (camIndex == -1)
			{
				rayIsNonUIHit = true;
			}
			else if (!usedPointers[ptrIndex] && !usedNonUIHits[ptrIndex])
			{
				usedNonUIHits[ptrIndex] = true;
				nonUIHits[numNonUIHits] = new NonUIHitInfo(ptrIndex, camIndex);
				numNonUIHits++;
			}
		}
	}

	protected void CallNonUIHitDelegate()
	{
		if (informNonUIHit == null)
		{
			return;
		}
		for (int i = 0; i < numNonUIHits; i++)
		{
			NonUIHitInfo nonUIHitInfo = nonUIHits[i];
			usedNonUIHits[nonUIHitInfo.ptrIndex] = false;
			if (!usedPointers[nonUIHitInfo.ptrIndex])
			{
				informNonUIHit(pointers[nonUIHitInfo.camIndex, nonUIHitInfo.ptrIndex]);
			}
		}
		if (rayIsNonUIHit)
		{
			informNonUIHit(rayPtr);
		}
	}

	public bool DidPointerHitUI(int id)
	{
		if (lastUpdateFrame != Time.frameCount)
		{
			Update();
		}
		if (id == -1)
		{
			return rayPtr.targetObj != null;
		}
		Mathf.Clamp(id, 0, usedPointers.Length - 1);
		return usedPointers[id];
	}

	public bool DidAnyPointerHitUI()
	{
		if (lastUpdateFrame != Time.frameCount)
		{
			Update();
		}
		if (rayPtr.targetObj != null)
		{
			return true;
		}
		for (int i = 0; i < usedPointers.Length; i++)
		{
			if (usedPointers[i])
			{
				return true;
			}
		}
		return false;
	}

	public void AddCamera(Camera cam, LayerMask mask, float depth)
	{
		AddCamera(cam, mask, depth, -1);
	}

	public void AddCamera(Camera cam, LayerMask mask, float depth, int index)
	{
		EZCameraSettings[] array = new EZCameraSettings[uiCameras.Length + 1];
		if (index == -1)
		{
			index = uiCameras.Length - 1;
		}
		index = Mathf.Clamp(index, 0, uiCameras.Length + 1);
		int i = 0;
		int num = 0;
		for (; i < array.Length; i++)
		{
			if (i == index)
			{
				array[i] = new EZCameraSettings();
				array[i].camera = cam;
				array[i].mask = mask;
				array[i].rayDepth = depth;
			}
			else
			{
				array[i] = uiCameras[num++];
			}
		}
		uiCameras = array;
		SetupPointers();
	}

	public void RemoveCamera(int index)
	{
		EZCameraSettings[] array = new EZCameraSettings[uiCameras.Length - 1];
		index = Mathf.Clamp(index, 0, uiCameras.Length);
		int num = 0;
		for (int i = 0; i < uiCameras.Length; i++)
		{
			if (i != index)
			{
				array[num] = uiCameras[i];
				num++;
			}
		}
		uiCameras = array;
		SetupPointers();
	}

	public bool RemoveCamera(ref Camera ao_Camera)
	{
		Camera tempCamera = ao_Camera;
		EZCameraSettings eZCameraSettings = m_ListCameraSettings.Find((EZCameraSettings cam) => cam.camera == tempCamera);
		if (eZCameraSettings != null)
		{
			m_ListCameraSettings.Remove(eZCameraSettings);
		}
		int cameraIndex = GetCameraIndex(ref ao_Camera);
		if (cameraIndex != -1)
		{
			RemoveCamera(cameraIndex);
			return true;
		}
		return false;
	}

	public int GetCameraIndex(ref Camera ao_Camera)
	{
		for (int i = 0; i < uiCameras.Length; i++)
		{
			if (uiCameras[i].camera == ao_Camera)
			{
				return i;
			}
		}
		return -1;
	}

	public void ReplaceCamera(int index, Camera cam)
	{
		if (index > uiCameras.Length || index < 0)
		{
			Debug.LogError("Can't replace camera because index does not exist.");
			return;
		}
		uiCameras[index].camera = cam;
		SetupPointers();
	}

	public void OnLevelWasLoaded(int level)
	{
		Camera ao_Camera = null;
		while (RemoveCamera(ref ao_Camera))
		{
		}
		if (rayCamera == null)
		{
			rayCamera = Camera.main;
		}
		if (focusObj == null)
		{
			FocusObject = null;
		}
		if (m_started)
		{
			SetupPointers();
		}
	}

	protected void BeginDrag(ref POINTER_INFO curPtr)
	{
		curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Begin, curPtr.targetObj, curPtr));
		curPtr.targetObj.DragUpdatePosition(curPtr);
	}

	protected void DoDragUpdate(POINTER_INFO curPtr)
	{
		IUIObject targetObj = curPtr.targetObj;
		targetObj.DragUpdatePosition(curPtr);
		RaycastHit[] array = Physics.RaycastAll(curPtr.ray, curPtr.rayDepth, curPtr.layerMask & (int)targetObj.DropMask);
		if (array.Length == 0 || (array.Length == 1 && array[0].transform == targetObj.transform))
		{
			for (int i = 0; i < uiCameras.Length; i++)
			{
				if (!(uiCameras[i].camera == curPtr.camera))
				{
					POINTER_INFO pOINTER_INFO = pointers[i, curPtr.id];
					array = Physics.RaycastAll(pOINTER_INFO.ray, pOINTER_INFO.rayDepth, pOINTER_INFO.layerMask & (int)targetObj.DropMask);
					if (array.Length != 0 && (array.Length != 1 || !(array[0].transform == targetObj.transform)))
					{
						break;
					}
				}
			}
		}
		RaycastHit raycastHit = default(RaycastHit);
		raycastHit.distance = float.PositiveInfinity;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].transform != targetObj.transform && array[j].distance < raycastHit.distance)
			{
				raycastHit = array[j];
			}
		}
		targetObj.DropTarget = ((!raycastHit.transform) ? null : raycastHit.transform.gameObject);
		switch (curPtr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
		case POINTER_INFO.INPUT_EVENT.DRAG:
			curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Update, targetObj, curPtr));
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
			curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Dropped, targetObj, curPtr));
			break;
		}
	}

	public virtual void Update()
	{
		time = Time.realtimeSinceStartup;
		realtimeDelta = time - startTime;
		startTime = time;
		if (lastUpdateFrame == Time.frameCount)
		{
			return;
		}
		lastUpdateFrame = Time.frameCount;
		if (uiCameras.Length != 0)
		{
			if (pointerPoller != null)
			{
				pointerPoller();
			}
			if (focusObj != null)
			{
				PollKeyboard();
			}
			DispatchInput();
		}
	}

	protected void DispatchInput()
	{
		numNonUIHits = 0;
		rayIsNonUIHit = false;
		for (int i = 0; i < usedPointers.Length; i++)
		{
			usedPointers[i] = false;
		}
		if (mouseTouchListeners != null)
		{
			for (int j = 0; j < numActivePointers; j++)
			{
				for (int k = 0; k < uiCameras.Length; k++)
				{
					if (uiCameras[k].camera.gameObject.activeSelf)
					{
						DispatchHelper(ref pointers[k, activePointers[j]], k);
						if (mouseTouchListeners != null)
						{
							mouseTouchListeners(pointers[k, activePointers[j]]);
						}
						if (usedPointers[activePointers[j]])
						{
							break;
						}
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < numActivePointers; l++)
			{
				for (int m = 0; m < uiCameras.Length; m++)
				{
					if (uiCameras[m].camera != null && uiCameras[m].camera.gameObject.activeSelf)
					{
						DispatchHelper(ref pointers[m, activePointers[l]], m);
						if (usedPointers[activePointers[l]])
						{
							break;
						}
					}
				}
			}
		}
		if (pointerType == POINTER_TYPE.RAY || pointerType == POINTER_TYPE.MOUSE_AND_RAY || pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
		{
			DispatchHelper(ref rayPtr, -1);
			if (rayListeners != null)
			{
				rayListeners(rayPtr);
			}
		}
		CallNonUIHitDelegate();
	}

	protected void DispatchHelper(ref POINTER_INFO curPtr, int camIndex)
	{
		if (inputOutsideViewport != 0 && curPtr.type != POINTER_INFO.POINTER_TYPE.RAY && !curPtr.camera.pixelRect.Contains(curPtr.devicePos))
		{
			if (inputOutsideViewport != OUTSIDE_VIEWPORT.Ignore && curPtr.targetObj != null)
			{
				tempPtr.Copy(curPtr);
				if (curPtr.active)
				{
					tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
					curPtr.targetObj.OnInput(tempPtr);
				}
				else
				{
					tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
					tempPtr.targetObj.OnInput(tempPtr);
				}
				if (curPtr.targetObj.IsDragging)
				{
					curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Dropped, curPtr.targetObj, curPtr));
				}
				curPtr.targetObj = null;
			}
			return;
		}
		if (curPtr.targetObj != null && curPtr.targetObj.IsDragging)
		{
			DoDragUpdate(curPtr);
		}
		else
		{
			switch (curPtr.evt)
			{
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if (curPtr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || curPtr.evt == POINTER_INFO.INPUT_EVENT.TAP)
				{
					tempObj = null;
					if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
					{
						tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
						curPtr.hitInfo = hit;
						if (tempObj != null)
						{
							tempObj = tempObj.GetControl(ref curPtr);
						}
						if (tempObj == null)
						{
							AddNonUIHit(curPtr.id, camIndex);
						}
					}
					else
					{
						curPtr.hitInfo = default(RaycastHit);
					}
					if (tempObj != curPtr.targetObj)
					{
						if (curPtr.targetObj != null)
						{
							tempPtr.Copy(curPtr);
							if (curPtr.evt == POINTER_INFO.INPUT_EVENT.RELEASE)
							{
								tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
							}
							else
							{
								tempPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
							}
							curPtr.targetObj.OnInput(tempPtr);
						}
						if (curPtr.id >= 0)
						{
							usedPointers[curPtr.id] = true;
						}
						if (!blockInput)
						{
							curPtr.targetObj = tempObj;
						}
						if (tempObj != null && curPtr.evt != POINTER_INFO.INPUT_EVENT.TAP && curPtr.evt != POINTER_INFO.INPUT_EVENT.RELEASE && !blockInput)
						{
							tempObj.OnInput(curPtr);
						}
					}
					else if (curPtr.targetObj != null)
					{
						curPtr.targetObj.OnInput(curPtr);
						if (curPtr.id >= 0)
						{
							usedPointers[curPtr.id] = true;
						}
					}
					if (curPtr.type == POINTER_INFO.POINTER_TYPE.TOUCHPAD)
					{
						curPtr.targetObj = null;
					}
					break;
				}
				if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
				{
					curPtr.hitInfo = hit;
					if (curPtr.targetObj == null)
					{
						AddNonUIHit(curPtr.id, camIndex);
					}
				}
				else
				{
					curPtr.hitInfo = default(RaycastHit);
				}
				if (curPtr.targetObj != null && !blockInput)
				{
					curPtr.targetObj.OnInput(curPtr);
					if (curPtr.targetObj.IsDraggable && !curPtr.isTap && curPtr.targetObj.controlIsEnabled)
					{
						BeginDrag(ref curPtr);
					}
				}
				break;
			case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			case POINTER_INFO.INPUT_EVENT.MOVE:
				tempObj = null;
				if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
				{
					tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
					curPtr.hitInfo = hit;
					if (tempObj != null)
					{
						tempObj = tempObj.GetControl(ref curPtr);
					}
					if (tempObj == null)
					{
						AddNonUIHit(curPtr.id, camIndex);
						if (warnOnNonUiHits)
						{
							LogNonUIObjErr(hit.collider.gameObject);
						}
					}
					if (!curPtr.active)
					{
						if (curPtr.targetObj != tempObj && curPtr.targetObj != null)
						{
							tempPtr.Copy(curPtr);
							tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
							if (!blockInput)
							{
								curPtr.targetObj.OnInput(tempPtr);
							}
						}
						if (!blockInput)
						{
							curPtr.targetObj = tempObj;
							if (tempObj != null)
							{
								curPtr.targetObj.OnInput(curPtr);
							}
						}
					}
					else if (curPtr.targetObj != null && !blockInput)
					{
						curPtr.targetObj.OnInput(curPtr);
					}
				}
				else
				{
					curPtr.hitInfo = default(RaycastHit);
					if (curPtr.targetObj != null && !curPtr.active)
					{
						curPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
						curPtr.targetObj.OnInput(curPtr);
					}
					if (!curPtr.active)
					{
						curPtr.targetObj = null;
					}
				}
				break;
			case POINTER_INFO.INPUT_EVENT.PRESS:
				if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
				{
					tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
					curPtr.hitInfo = hit;
					if (tempObj != null)
					{
						tempObj = tempObj.GetControl(ref curPtr);
					}
					if (tempObj == null)
					{
						AddNonUIHit(curPtr.id, camIndex);
						if (warnOnNonUiHits)
						{
							LogNonUIObjErr(hit.collider.gameObject);
						}
					}
					if (tempObj != curPtr.targetObj && curPtr.targetObj != null)
					{
						tempPtr.Copy(curPtr);
						tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
						if (!blockInput)
						{
							curPtr.targetObj.OnInput(tempPtr);
						}
					}
					if (!blockInput)
					{
						curPtr.targetObj = tempObj;
					}
					else
					{
						if (curPtr.targetObj != null)
						{
							tempPtr.Copy(curPtr);
							tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
							curPtr.targetObj.OnInput(tempPtr);
						}
						curPtr.targetObj = null;
					}
					if (curPtr.targetObj != null)
					{
						if (!blockInput)
						{
							curPtr.targetObj.OnInput(curPtr);
						}
						if (curPtr.targetObj != focusObj && ((curPtr.type == POINTER_INFO.POINTER_TYPE.RAY && focusWithRay) || curPtr.type != POINTER_INFO.POINTER_TYPE.RAY))
						{
							FocusObject = curPtr.targetObj;
						}
					}
					else if (curPtr.type == POINTER_INFO.POINTER_TYPE.RAY == focusWithRay)
					{
						FocusObject = null;
					}
				}
				else
				{
					curPtr.hitInfo = default(RaycastHit);
					if (blockInput && curPtr.targetObj != null)
					{
						tempPtr.Copy(curPtr);
						tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
						curPtr.targetObj.OnInput(tempPtr);
					}
					curPtr.targetObj = null;
					if (curPtr.type == POINTER_INFO.POINTER_TYPE.RAY == focusWithRay)
					{
						FocusObject = null;
					}
				}
				break;
			}
		}
		if (curPtr.targetObj != null && curPtr.id >= 0)
		{
			usedPointers[curPtr.id] = true;
		}
	}

	protected void PollMouse()
	{
		PollMouse(ref pointers[0, 0]);
		for (int i = 1; i < uiCameras.Length; i++)
		{
			if (uiCameras[i].camera == null)
			{
				RemoveCamera(i);
			}
			else if (uiCameras[i].camera.gameObject.activeSelf)
			{
				pointers[i, 0].Reuse(pointers[0, 0]);
				pointers[i, 0].prevRay = pointers[i, 0].ray;
				pointers[i, 0].ray = uiCameras[i].camera.ScreenPointToRay(pointers[i, 0].devicePos);
			}
		}
	}

	protected void PollMouseAndTouchpad()
	{
		numActivePointers = 1;
		int num = numTouches - 1;
		activePointers[numActivePointers - 1] = num;
		PollMouse(ref pointers[0, num]);
		for (int i = 1; i < uiCameras.Length; i++)
		{
			if (uiCameras[i].camera == null)
			{
				RemoveCamera(i);
			}
			else if (uiCameras[i].camera.gameObject.activeSelf)
			{
				pointers[i, num].Reuse(pointers[0, num]);
				pointers[i, num].prevRay = pointers[i, num].ray;
				pointers[i, num].ray = uiCameras[i].camera.ScreenPointToRay(pointers[i, num].devicePos);
			}
		}
	}

	protected void PollMouse(ref POINTER_INFO curPtr)
	{
		down = FingerGestures.InputFinger.IsDown;
		float axis = Input.GetAxis("Mouse ScrollWheel");
		axis *= realtimeDelta;
		bool flag = axis != 0f;
		if (down && curPtr.active)
		{
			if (FingerGestures.mousePosition != curPtr.devicePos)
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.DRAG;
				curPtr.inputDelta = FingerGestures.mousePosition - curPtr.devicePos;
				curPtr.devicePos = FingerGestures.mousePosition;
				if (curPtr.isTap)
				{
					tempVec = curPtr.origPos - curPtr.devicePos;
					if (Mathf.Abs(tempVec.x) > dragThreshold || Mathf.Abs(tempVec.y) > dragThreshold)
					{
						curPtr.isTap = false;
					}
				}
			}
			else
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				curPtr.inputDelta = Vector3.zero;
			}
		}
		else if (down && !curPtr.active)
		{
			curPtr.Reset(curActionID++);
			curPtr.evt = POINTER_INFO.INPUT_EVENT.PRESS;
			curPtr.active = true;
			curPtr.inputDelta = FingerGestures.mousePosition - curPtr.devicePos;
			curPtr.origPos = FingerGestures.mousePosition;
			curPtr.isTap = true;
			curPtr.activeTime = Time.time;
			curPtr.targetObj = null;
		}
		else if (!down && curPtr.active)
		{
			curPtr.inputDelta = FingerGestures.mousePosition - curPtr.devicePos;
			curPtr.devicePos = FingerGestures.mousePosition;
			if (curPtr.isTap)
			{
				tempVec = curPtr.origPos - curPtr.devicePos;
				if (Mathf.Abs(tempVec.x) > dragThreshold || Mathf.Abs(tempVec.y) > dragThreshold)
				{
					curPtr.isTap = false;
				}
			}
			if (curPtr.isTap)
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
			}
			else
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
			}
			curPtr.active = false;
			curPtr.activeTime = 0f;
		}
		else if (!down && FingerGestures.mousePosition != curPtr.devicePos)
		{
			curPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
			curPtr.inputDelta = FingerGestures.mousePosition - curPtr.devicePos;
			curPtr.devicePos = FingerGestures.mousePosition;
		}
		else
		{
			curPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			curPtr.inputDelta = Vector3.zero;
		}
		if (flag)
		{
			curPtr.inputDelta.z = axis;
		}
		curPtr.devicePos = FingerGestures.mousePosition;
		curPtr.prevRay = curPtr.ray;
		if (uiCameras[0].camera != null)
		{
			curPtr.ray = uiCameras[0].camera.ScreenPointToRay(curPtr.devicePos);
		}
	}

	protected void PollTouchpad()
	{
	}

	protected void PollRay()
	{
		if (actionAxis.Length != 0)
		{
			rayActive = Input.GetButton(actionAxis);
		}
		else
		{
			rayActive = rayState != RAY_ACTIVE_STATE.Inactive;
			if (rayState == RAY_ACTIVE_STATE.Momentary)
			{
				rayState = RAY_ACTIVE_STATE.Inactive;
			}
		}
		if (rayActive && rayPtr.active)
		{
			if (raycastingTransform.forward != rayPtr.ray.direction || raycastingTransform.position != rayPtr.ray.origin)
			{
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.DRAG;
				tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
				rayPtr.inputDelta = tempVec - rayPtr.devicePos;
				rayPtr.devicePos = tempVec;
				if (rayPtr.isTap)
				{
					tempVec = rayPtr.origPos - rayPtr.devicePos;
					if (tempVec.sqrMagnitude > rayDragThreshold * rayDragThreshold)
					{
						rayPtr.isTap = false;
					}
				}
			}
			else
			{
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				rayPtr.inputDelta = Vector3.zero;
			}
		}
		else if (rayActive && !rayPtr.active)
		{
			rayPtr.Reset(curActionID++);
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.PRESS;
			rayPtr.active = true;
			rayPtr.origPos = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = rayPtr.origPos - rayPtr.devicePos;
			rayPtr.devicePos = rayPtr.origPos;
			rayPtr.isTap = true;
			rayPtr.activeTime = Time.time;
			rayPtr.targetObj = null;
		}
		else if (!rayActive && rayPtr.active)
		{
			if (rayPtr.isTap)
			{
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
			}
			else
			{
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
			}
			tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = tempVec - rayPtr.devicePos;
			rayPtr.devicePos = tempVec;
			rayPtr.active = false;
			rayPtr.activeTime = 0f;
		}
		else if (!rayActive && FingerGestures.mousePosition != rayPtr.devicePos)
		{
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
			tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = tempVec - rayPtr.devicePos;
			rayPtr.devicePos = tempVec;
		}
		else
		{
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			rayPtr.inputDelta = Vector3.zero;
		}
		rayPtr.prevRay = rayPtr.ray;
		rayPtr.ray = new Ray(raycastingTransform.position, raycastingTransform.forward);
	}

	protected void PollMouseRay()
	{
		PollMouse();
		PollRay();
	}

	protected void PollTouchpadRay()
	{
		PollTouchpad();
		PollRay();
	}

	protected void PollKeyboard()
	{
		ProcessKeyboard();
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			controlText = ((IKeyFocusable)focusObj).Content;
			insert = Mathf.Min(controlText.Length, insert + 1);
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			controlText = ((IKeyFocusable)focusObj).Content;
			insert = Mathf.Max(0, insert - 1);
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}
		else if (Input.GetKeyDown(KeyCode.Home))
		{
			controlText = ((IKeyFocusable)focusObj).Content;
			insert = 0;
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}
		else if (Input.GetKeyDown(KeyCode.End))
		{
			controlText = ((IKeyFocusable)focusObj).Content;
			insert = controlText.Length;
			((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			((IKeyFocusable)focusObj).GoUp();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			((IKeyFocusable)focusObj).GoDown();
		}
	}

	protected void ProcessKeyboard()
	{
		if (Input.inputString.Length == 0 && !Input.GetKeyDown(KeyCode.Delete))
		{
			return;
		}
		controlText = ((IKeyFocusable)focusObj).Content;
		insert = Mathf.Clamp(insert, 0, controlText.Length);
		sb.Length = 0;
		sb.Append(controlText);
		string inputString = Input.inputString;
		foreach (char c in inputString)
		{
			if (c == '\b')
			{
				insert = Mathf.Max(0, insert - 1);
				if (insert < sb.Length)
				{
					sb.Remove(insert, 1);
				}
			}
			else
			{
				sb.Insert(insert, c);
				insert++;
			}
		}
		if (Input.GetKeyDown(KeyCode.Delete) && insert < sb.Length)
		{
			sb.Remove(insert, 1);
		}
		controlText = sb.ToString();
		controlText = ((IKeyFocusable)focusObj).SetInputText(controlText, ref insert);
	}

	public void Detarget(IUIObject obj)
	{
		Retarget(obj, null);
	}

	public void Detarget(int pointerID)
	{
		if (uiCameras == null)
		{
			return;
		}
		for (int i = 0; i < uiCameras.Length; i++)
		{
			if (uiCameras[i].camera != null && uiCameras[i].camera.gameObject.activeSelf && pointers[i, pointerID].targetObj != null)
			{
				POINTER_INFO ptr = default(POINTER_INFO);
				ptr.Copy(pointers[i, pointerID]);
				ptr.isTap = false;
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
				pointers[i, pointerID].targetObj.OnInput(ptr);
				pointers[i, pointerID].targetObj = null;
			}
		}
	}

	public void DetargetAllExcept(int pointerID)
	{
		for (int i = 0; i < numActivePointers; i++)
		{
			if (activePointers[i] == pointerID)
			{
				continue;
			}
			for (int j = 0; j < uiCameras.Length; j++)
			{
				if (uiCameras[j].camera != null && uiCameras[j].camera.gameObject.activeSelf && pointers[j, activePointers[i]].targetObj != null)
				{
					POINTER_INFO ptr = default(POINTER_INFO);
					ptr.Copy(pointers[j, pointerID]);
					ptr.isTap = false;
					ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
					pointers[j, pointerID].targetObj.OnInput(ptr);
					pointers[j, activePointers[i]].targetObj = null;
				}
			}
		}
	}

	public void Retarget(IUIObject oldObj, IUIObject newObj)
	{
		if (uiCameras == null)
		{
			return;
		}
		for (int i = 0; i < numActivePointers; i++)
		{
			for (int j = 0; j < uiCameras.Length; j++)
			{
				if (uiCameras[j].camera != null && uiCameras[j].camera.gameObject.activeSelf && pointers[j, activePointers[i]].targetObj != null && pointers[j, activePointers[i]].targetObj == oldObj)
				{
					pointers[j, activePointers[i]].targetObj = newObj;
				}
			}
		}
		if (rayPtr.targetObj == oldObj)
		{
			rayPtr.targetObj = newObj;
		}
	}

	public bool GetPointer(IUIObject obj, out POINTER_INFO ptr)
	{
		if (uiCameras == null)
		{
			ptr = pointers[0, 0];
			return false;
		}
		for (int i = 0; i < numActivePointers; i++)
		{
			for (int j = 0; j < uiCameras.Length; j++)
			{
				if (uiCameras[j].camera != null && uiCameras[j].camera.gameObject.activeSelf && pointers[j, activePointers[i]].targetObj != null)
				{
					if (pointers[j, activePointers[i]].targetObj == obj)
					{
						ptr = pointers[j, activePointers[i]];
						return true;
					}
					break;
				}
			}
		}
		if (rayPtr.targetObj == obj)
		{
			ptr = rayPtr;
			return true;
		}
		ptr = pointers[0, 0];
		return false;
	}

	public bool GetPointer(int pointerID, int camera, out POINTER_INFO ptr)
	{
		if (uiCameras == null || camera < 0 || camera >= uiCameras.Length || pointerID < 0 || pointerID >= numPointers)
		{
			ptr = pointers[0, 0];
			return false;
		}
		for (int i = 0; i < numActivePointers; i++)
		{
			if (activePointers[i] == pointerID)
			{
				ptr = pointers[camera, activePointers[i]];
				return true;
			}
		}
		ptr = pointers[0, 0];
		return false;
	}

	public void LockInput()
	{
		blockInput = true;
		inputLockCount++;
	}

	public void UnlockInput()
	{
		inputLockCount--;
		if (inputLockCount < 1)
		{
			inputLockCount = 0;
			blockInput = false;
		}
	}

	public bool IsInputLocked()
	{
		return blockInput;
	}

	protected static int FindInsertionPoint(string before, string after)
	{
		if (before == null || after == null)
		{
			return 0;
		}
		for (int i = 0; i < before.Length && i < after.Length; i++)
		{
			if (before[i] != after[i])
			{
				return i + 1;
			}
		}
		return after.Length;
	}

	protected void LogNonUIObjErr(GameObject obj)
	{
		Debug.LogWarning("The UIManager encountered a collider on object \"" + obj.name + "\" that does not not contain an IUIObject or derivative component.  Please double-check that this object has the correct layer and components assigned.");
	}

	public static void AddAction<T>(IActionButton i_Button, GameObject i_ScriptObject, string i_MethodToInvoke, POINTER_INFO.INPUT_EVENT i_Event, string i_Argument)
	{
		UIActionInfo uIActionInfo = new UIActionInfo();
		uIActionInfo.scriptWithMethodToInvoke = i_ScriptObject.GetComponent(typeof(T)) as MonoBehaviour;
		uIActionInfo.methodToInvoke = i_MethodToInvoke;
		uIActionInfo.whenToInvoke = i_Event;
		uIActionInfo.sArgument = i_Argument;
		i_Button.GetActionInfo().Add(uIActionInfo);
	}

	public static void AddAction<T>(IActionButton i_Button, GameObject i_ScriptObject, string i_MethodToInvoke, POINTER_INFO.INPUT_EVENT i_Event)
	{
		AddAction<T>(i_Button, i_ScriptObject, i_MethodToInvoke, i_Event, string.Empty);
	}

	public static void CleanActions(IActionButton i_Button)
	{
		i_Button.GetActionInfo().Clear();
	}

	private void OnBlockInputChanging(BlockInputChangingEventArgs args)
	{
		if (UIManager.BlockInputChanging != null)
		{
			UIManager.BlockInputChanging(this, args);
		}
	}
}
