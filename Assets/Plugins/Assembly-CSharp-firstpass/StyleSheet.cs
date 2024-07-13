using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StyleSheet : MonoBehaviour
{
	public enum Display
	{
		TOP = 0,
		RIGHT = 1,
		BOTTOM = 2,
		LEFT = 3,
		BOTTOM_RIGHT = 4,
		BOTTOM_LEFT = 5,
		UPPER_LEFT = 6,
		UPPER_RIGHT = 7,
		CENTER = 8
	}

	public enum Position
	{
		ABSOLUTE = 0,
		RELATIVE = 1
	}

	public enum TargetType
	{
		EZGUI_COMPONENT = 0,
		GAMEOBJECT = 1,
		SPRITE_TEXT = 2,
		UI_PANEL = 3,
		UI_SCROLL_LIST = 4,
		UI_SLIDER = 5,
		UI_TEXT_FIELD = 6
	}

	public enum DimensionType
	{
		WIDTH = 0,
		HEIGHT = 1
	}

	public enum Rotation
	{
		ROTATE_R_0 = 0,
		ROTATE_R_90 = 90,
		ROTATE_R_180 = 180,
		ROTATE_R_270 = 270
	}

	[HideInInspector]
	public Transform m_Transform;

	[HideInInspector]
	private GameObject m_MyGameObject;

	public Display display = Display.CENTER;

	public Position position;

	public TargetType targetType;

	public Rotation rotation;

	public bool percent = true;

	public bool percentDimention = true;

	public bool applyScreenDimensionToSpriteText;

	public float hundredPercentSize = 50f;

	public int zIndex;

	public float widthKnob;

	public float heightKnob;

	public float widthCaret;

	public float heightCaret;

	public bool forceSquare = true;

	public float ratioX = 1f;

	public float ratioY = 1f;

	public bool flipX;

	public bool flipY;

	public bool destroyOnDone;

	public bool m_InitDone;

	public bool onlyWidth;

	public bool onlyHeight;

	public bool onlyScale;

	public bool onlyPosition;

	public bool disableRotation;

	public bool onlyY;

	public bool onlyX;

	public int parentQTE = 1;

	public float SettingWidth;

	public float SettingHeight;

	public bool EditModeGizmo;

	public Vector3 colliderComponent = new Vector3(1f, 1f, 0.2f);

	public Vector3 colliderOffset = Vector3.zero;

	public Vector3 SizeBox = Vector3.one;

	public Vector3 OffsetGizmo = Vector3.zero;

	public List<StyleSheet> m_ListStyleSheetRelative;

	public bool m_DisplayComplete;

	public float width;

	public float height;

	public float initialHeight;

	public float margin_left;

	public float margin_right;

	public float margin_bottom;

	public float margin_top;

	public bool keepRatioX;

	public bool keepRatioY;

	public Camera oCamera;

	private float oScreenWidth;

	private float oScreenHeight;

	private float sizeWidth;

	private float sizeHeight;

	private float sizeWidthKnob;

	private float sizeHeightKnob;

	private float sizeWidthCaret;

	private float sizeHeightCaret;

	private float fWidthConvert;

	private float fHeightConvert;

	private Vector3 vCurrentScale = Vector3.zero;

	private Vector3 oCurrentScreenPosition = default(Vector3);

	private Vector3 vCurrentLocalPosition = default(Vector3);

	private Vector3 vPhysicSize = Vector3.zero;

	private Vector3 vPhysicPosition = Vector3.zero;

	private Vector2 vSizeKnob = Vector2.zero;

	private Vector2 vSizeCaret = Vector2.zero;

	private SpriteText m_TargetText;

	private UIScrollList m_TargetScrollList;

	private StyleSheet oParent;

	private SpriteRoot m_TargetsGlobal;

	private float widthParent;

	private float HeightParent;

	private Display m_workingDisplay = Display.CENTER;

	public bool DisplayComplete
	{
		get
		{
			return m_DisplayComplete;
		}
	}

	public float Margin_left
	{
		get
		{
			return margin_left;
		}
		set
		{
			margin_left = value;
		}
	}

	public float Margin_right
	{
		get
		{
			return margin_right;
		}
		set
		{
			margin_right = value;
		}
	}

	public float Margin_bottom
	{
		get
		{
			return margin_bottom;
		}
		set
		{
			margin_bottom = value;
		}
	}

	public float Margin_top
	{
		get
		{
			return margin_top;
		}
		set
		{
			margin_top = value;
		}
	}

	public float Width
	{
		get
		{
			float result = width;
			Rotation rotation = this.rotation;
			if (rotation == Rotation.ROTATE_R_90 || rotation == Rotation.ROTATE_R_270)
			{
				result = height;
			}
			return result;
		}
		set
		{
			switch (rotation)
			{
			case Rotation.ROTATE_R_0:
			case Rotation.ROTATE_R_180:
				width = value;
				break;
			case Rotation.ROTATE_R_90:
			case Rotation.ROTATE_R_270:
				height = value;
				break;
			}
		}
	}

	public float Height
	{
		get
		{
			float result = height;
			Rotation rotation = this.rotation;
			if (rotation == Rotation.ROTATE_R_90 || rotation == Rotation.ROTATE_R_270)
			{
				result = width;
			}
			return result;
		}
		set
		{
			switch (rotation)
			{
			case Rotation.ROTATE_R_0:
			case Rotation.ROTATE_R_180:
				height = value;
				break;
			case Rotation.ROTATE_R_90:
			case Rotation.ROTATE_R_270:
				width = value;
				break;
			}
		}
	}

	public float ObjectWidth
	{
		get
		{
			float result = 0f;
			switch (rotation)
			{
			case Rotation.ROTATE_R_0:
			case Rotation.ROTATE_R_180:
				result = GetObjectWidth();
				break;
			case Rotation.ROTATE_R_90:
			case Rotation.ROTATE_R_270:
				result = GetObjectHeight();
				break;
			}
			return result;
		}
	}

	public float ObjectHeight
	{
		get
		{
			float result = 0f;
			switch (rotation)
			{
			case Rotation.ROTATE_R_0:
			case Rotation.ROTATE_R_180:
				result = GetObjectHeight();
				break;
			case Rotation.ROTATE_R_90:
			case Rotation.ROTATE_R_270:
				result = GetObjectWidth();
				break;
			}
			return result;
		}
	}

	public Display DisplayPosition
	{
		get
		{
			return display;
		}
		set
		{
			display = value;
		}
	}

	public bool FlipX
	{
		get
		{
			return flipX;
		}
		set
		{
			flipX = value;
		}
	}

	public bool FlipY
	{
		get
		{
			return flipY;
		}
		set
		{
			flipY = value;
		}
	}

	public Camera CameraSheet
	{
		get
		{
			return oCamera;
		}
		set
		{
			oCamera = value;
			InitTarget();
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetCamera(oCamera);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetCamera(oCamera);
				break;
			case TargetType.UI_SCROLL_LIST:
				m_TargetScrollList.renderCamera = oCamera;
				break;
			}
			ResetStyleSheet(false);
		}
	}

	public Camera CameraSheetActive
	{
		get
		{
			return oCamera;
		}
		set
		{
			oCamera = value;
			InitTarget();
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.RenderCamera = oCamera;
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.RenderCamera = oCamera;
				break;
			case TargetType.UI_SCROLL_LIST:
				m_TargetScrollList.renderCamera = oCamera;
				break;
			}
			ResetStyleSheet(true);
		}
	}

	public bool KeepRatioX
	{
		get
		{
			bool result = keepRatioX;
			Rotation rotation = this.rotation;
			if (rotation == Rotation.ROTATE_R_90 || rotation == Rotation.ROTATE_R_270)
			{
				result = keepRatioY;
			}
			return result;
		}
		set
		{
			switch (rotation)
			{
			case Rotation.ROTATE_R_0:
			case Rotation.ROTATE_R_180:
				keepRatioX = value;
				break;
			case Rotation.ROTATE_R_90:
			case Rotation.ROTATE_R_270:
				keepRatioY = value;
				break;
			}
		}
	}

	public bool KeepRatioY
	{
		get
		{
			bool result = keepRatioY;
			Rotation rotation = this.rotation;
			if (rotation == Rotation.ROTATE_R_90 || rotation == Rotation.ROTATE_R_270)
			{
				result = keepRatioX;
			}
			return result;
		}
		set
		{
			switch (rotation)
			{
			case Rotation.ROTATE_R_0:
			case Rotation.ROTATE_R_180:
				keepRatioY = value;
				break;
			case Rotation.ROTATE_R_90:
			case Rotation.ROTATE_R_270:
				keepRatioX = value;
				break;
			}
		}
	}

	private float OScreenWidth
	{
		get
		{
			float result = oScreenWidth;
			Rotation rotation = this.rotation;
			if (rotation == Rotation.ROTATE_R_90 || rotation == Rotation.ROTATE_R_270)
			{
				result = oScreenHeight;
			}
			return result;
		}
		set
		{
			oScreenWidth = value;
		}
	}

	private float OScreenHeight
	{
		get
		{
			float result = oScreenHeight;
			Rotation rotation = this.rotation;
			if (rotation == Rotation.ROTATE_R_90 || rotation == Rotation.ROTATE_R_270)
			{
				result = oScreenWidth;
			}
			return result;
		}
		set
		{
			oScreenHeight = value;
		}
	}

	public StyleSheet Parent
	{
		get
		{
			return oParent;
		}
	}

	public float RatioX
	{
		get
		{
			return ratioX;
		}
		set
		{
			ratioX = value;
		}
	}

	public float RatioY
	{
		get
		{
			return ratioY;
		}
		set
		{
			ratioY = value;
		}
	}

	public void Awake()
	{
	}

	public void Start()
	{
	}

	public void OnDestroy()
	{
		m_ListStyleSheetRelative = null;
		m_Transform = null;
		m_MyGameObject = null;
		oCamera = null;
		oParent = null;
		m_TargetText = null;
		m_TargetScrollList = null;
		m_TargetsGlobal = null;
	}

	public void SetCameraRecursivly(Camera i_Camera)
	{
		SetCameraRecursivlyFunctionality(i_Camera);
		ResetStyleSheet(true);
	}

	public void SetStyleSheet()
	{
		if (!Init())
		{
			return;
		}
		if (EditModeGizmo)
		{
			m_Transform.localScale = Vector3.one;
		}
		DisplayObject();
		if (Application.isPlaying && destroyOnDone)
		{
			base.enabled = false;
		}
		foreach (StyleSheet item in m_ListStyleSheetRelative)
		{
			if (item != null)
			{
				item.SetChildStyleSheet();
			}
		}
	}

	public void ResetStyleSheet(bool i_SetStyleSheetRelative)
	{
		m_InitDone = false;
		m_DisplayComplete = false;
		InitTarget();
		if (RegisterToParent() && (position == Position.ABSOLUTE || i_SetStyleSheetRelative))
		{
			SetStyleSheet();
		}
	}

	public void SetChildStyleSheet()
	{
		m_InitDone = false;
		m_DisplayComplete = false;
		if (RegisterToParent())
		{
			SetStyleSheet();
		}
	}

	public bool AddChildToMyList(StyleSheet i_StyleSheetRegister)
	{
		m_ListStyleSheetRelative.Add(i_StyleSheetRegister);
		return m_InitDone && m_DisplayComplete;
	}

	private void SetCameraRecursivlyFunctionality(Camera i_Camera)
	{
		CameraSheet = i_Camera;
		StyleSheet[] componentsInChildren = GetComponentsInChildren<StyleSheet>();
		foreach (StyleSheet styleSheet in componentsInChildren)
		{
			if (styleSheet != this)
			{
				styleSheet.SetCameraRecursivlyFunctionality(i_Camera);
			}
		}
	}

	private bool RegisterToParent()
	{
		bool result = true;
		if (position == Position.RELATIVE)
		{
			if (parentQTE > 1)
			{
				Transform parent = m_Transform;
				for (int i = 1; i <= parentQTE; i++)
				{
					if (parent.parent != null)
					{
						parent = parent.parent;
						continue;
					}
					result = false;
					Debug.LogError("NO PARENT RELATTIVE FOR STYLESHEET GAMEOBJECT :   " + m_MyGameObject.name, m_MyGameObject);
					return result;
				}
				oParent = parent.GetComponent<StyleSheet>();
			}
			else
			{
				oParent = m_Transform.parent.GetComponent<StyleSheet>();
			}
			if (!(oParent != null))
			{
				return false;
			}
			if (!oParent.m_ListStyleSheetRelative.Contains(this) && oParent.AddChildToMyList(this))
			{
				SetStyleSheet();
			}
		}
		return result;
	}

	private bool Init()
	{
		if (m_InitDone)
		{
			return m_InitDone;
		}
		if (oCamera == null)
		{
			return m_InitDone;
		}
		m_InitDone = true;
		sizeHeight = Screen.height;
		sizeWidth = sizeHeight * (SettingWidth / SettingHeight);
		sizeWidth = oCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f)).x + sizeWidth / 2f;
		oScreenHeight = oCamera.orthographicSize * 2f;
		oScreenWidth = oScreenHeight * (SettingWidth / SettingHeight);
		return m_InitDone;
	}

	public void InitTarget()
	{
		if (m_Transform == null || m_MyGameObject == null)
		{
			m_Transform = base.transform;
			m_MyGameObject = base.gameObject;
		}
		switch (targetType)
		{
		case TargetType.EZGUI_COMPONENT:
		case TargetType.UI_SLIDER:
		case TargetType.UI_TEXT_FIELD:
			m_TargetsGlobal = m_MyGameObject.GetComponent<SpriteRoot>();
			if (m_TargetsGlobal == null)
			{
				Debug.LogError("One Object with style sheet not contain target type : " + targetType, base.gameObject);
			}
			break;
		case TargetType.SPRITE_TEXT:
			m_TargetText = m_MyGameObject.GetComponent<SpriteText>();
			if (m_TargetText == null)
			{
				Debug.LogError("One Object with style sheet not contain target type : " + targetType, base.gameObject);
			}
			break;
		case TargetType.UI_SCROLL_LIST:
			m_TargetScrollList = m_MyGameObject.GetComponent<UIScrollList>();
			if (m_TargetScrollList == null)
			{
				Debug.LogError("One Object with style sheet not contain target type : " + targetType, base.gameObject);
			}
			break;
		case TargetType.GAMEOBJECT:
		case TargetType.UI_PANEL:
			break;
		}
	}

	private bool DisplayObject()
	{
		if (m_DisplayComplete)
		{
			return m_DisplayComplete;
		}
		if (targetType != TargetType.UI_PANEL)
		{
			if (position == Position.RELATIVE && !oParent.m_DisplayComplete)
			{
				return false;
			}
			SetWorkingDisplay();
			if (!disableRotation)
			{
				SetRotation();
			}
			if (!onlyPosition)
			{
				if (keepRatioX)
				{
					SetWidth();
					SetHeight();
				}
				else
				{
					SetHeight();
					SetWidth();
				}
				if (targetType == TargetType.UI_SLIDER)
				{
					SetKnobSize();
				}
				if (targetType == TargetType.UI_TEXT_FIELD)
				{
					SetCaretSize();
				}
				SetCollider();
			}
			if (!onlyScale)
			{
				SetDisplay();
				SetMargin();
				SetZIndex();
			}
			else
			{
				ResetAnchorPos();
			}
			if (!onlyPosition)
			{
				SetCollider();
			}
		}
		m_DisplayComplete = true;
		return m_DisplayComplete;
	}

	private void SetDisplay()
	{
		SetAnchor();
		switch (display)
		{
		case Display.LEFT:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			ConvertRelative(0f, DimensionType.WIDTH);
			ConvertRelative(sizeHeight / 2f, DimensionType.HEIGHT);
			break;
		case Display.RIGHT:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			ConvertRelative(sizeWidth, DimensionType.WIDTH);
			ConvertRelative(sizeHeight / 2f, DimensionType.HEIGHT);
			break;
		case Display.TOP:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			ConvertRelative(sizeHeight, DimensionType.HEIGHT);
			ConvertRelative(sizeWidth / 2f, DimensionType.WIDTH);
			break;
		case Display.BOTTOM:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(base.transform.position);
			ConvertRelative(sizeWidth / 2f, DimensionType.WIDTH);
			ConvertRelative(0f, DimensionType.HEIGHT);
			break;
		case Display.CENTER:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			ConvertRelative(sizeHeight / 2f, DimensionType.HEIGHT);
			ConvertRelative(sizeWidth / 2f, DimensionType.WIDTH);
			break;
		case Display.BOTTOM_LEFT:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			ConvertRelative(0f, DimensionType.HEIGHT);
			ConvertRelative(0f, DimensionType.WIDTH);
			break;
		case Display.BOTTOM_RIGHT:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			ConvertRelative(0f, DimensionType.HEIGHT);
			ConvertRelative(sizeWidth, DimensionType.WIDTH);
			break;
		case Display.UPPER_LEFT:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			ConvertRelative(sizeHeight, DimensionType.HEIGHT);
			ConvertRelative(0f, DimensionType.WIDTH);
			break;
		case Display.UPPER_RIGHT:
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			ConvertRelative(sizeHeight, DimensionType.HEIGHT);
			ConvertRelative(sizeWidth, DimensionType.WIDTH);
			break;
		}
		if (position == Position.ABSOLUTE)
		{
			oCurrentScreenPosition = oCamera.ScreenToWorldPoint(oCurrentScreenPosition);
		}
		if (onlyX)
		{
			oCurrentScreenPosition.y = m_Transform.localPosition.y;
		}
		else if (onlyY)
		{
			oCurrentScreenPosition.x = m_Transform.localPosition.x;
		}
		if (position == Position.ABSOLUTE)
		{
			m_Transform.position = oCurrentScreenPosition;
		}
		else
		{
			m_Transform.localPosition = oCurrentScreenPosition;
		}
	}

	private void SetWorkingDisplay()
	{
		int num = (int)display;
		if (num < 4)
		{
			num = (int)(num + rotation) % 4;
		}
		else if (num < 8)
		{
			num = (int)(num + rotation) % 8;
			if (num < 4)
			{
				num += 4;
			}
		}
		m_workingDisplay = (Display)num;
	}

	private void SetAnchor()
	{
		switch (m_workingDisplay)
		{
		case Display.LEFT:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Middle_Left);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		case Display.RIGHT:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Middle_Right);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		case Display.TOP:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.UPPER_CENTER);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Upper_Center);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		case Display.BOTTOM:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Lower_Center);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		case Display.CENTER:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Middle_Center);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		case Display.BOTTOM_LEFT:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Lower_Left);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		case Display.BOTTOM_RIGHT:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Lower_Right);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		case Display.UPPER_LEFT:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.UPPER_LEFT);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Upper_Left);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		case Display.UPPER_RIGHT:
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.SetAnchor(SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetAnchor(SpriteText.Anchor_Pos.Upper_Right);
				break;
			case TargetType.GAMEOBJECT:
			case TargetType.UI_PANEL:
			case TargetType.UI_SCROLL_LIST:
				break;
			}
			break;
		}
	}

	private void SetMargin()
	{
		if (position == Position.RELATIVE)
		{
			oCurrentScreenPosition = m_Transform.localPosition;
			fWidthConvert = oParent.ObjectWidthParent(rotation);
			fHeightConvert = oParent.ObjectHeightParent(rotation);
		}
		else if (position == Position.ABSOLUTE)
		{
			oCurrentScreenPosition = oCamera.WorldToScreenPoint(m_Transform.position);
			fWidthConvert = sizeWidth;
			fHeightConvert = sizeHeight;
		}
		if (!onlyY)
		{
			oCurrentScreenPosition.x += Convert(Margin_left, fWidthConvert);
			oCurrentScreenPosition.x -= Convert(Margin_right, fWidthConvert);
		}
		if (!onlyX)
		{
			oCurrentScreenPosition.y -= Convert(Margin_top, fHeightConvert);
			oCurrentScreenPosition.y += Convert(Margin_bottom, fHeightConvert);
		}
		if (position == Position.RELATIVE)
		{
			m_Transform.localPosition = oCurrentScreenPosition;
		}
		else
		{
			m_Transform.position = oCamera.ScreenToWorldPoint(oCurrentScreenPosition);
		}
	}

	private void SetRotation()
	{
		m_Transform.rotation = Quaternion.AngleAxis((float)rotation, Vector3.forward);
	}

	private void SetColliderSize(BoxCollider _sizePhysic)
	{
		vPhysicSize.x = m_TargetsGlobal.width * colliderComponent.x;
		vPhysicSize.y = m_TargetsGlobal.height * colliderComponent.y;
		vPhysicSize.z = 1f * colliderComponent.z;
		_sizePhysic.size = vPhysicSize;
	}

	private void SetColliderSizeScrolllist(BoxCollider _sizePhysic, float _width, float _height)
	{
		vPhysicSize.x = _width * colliderComponent.x;
		vPhysicSize.y = _height * colliderComponent.y;
		vPhysicSize.z = 0.001f * colliderComponent.z;
		_sizePhysic.size = vPhysicSize;
	}

	private void SetColliderPosition(BoxCollider positionPhysic)
	{
		switch (m_workingDisplay)
		{
		case Display.LEFT:
			vPhysicPosition.x = positionPhysic.size.x / 2f;
			vPhysicPosition.y = 0f;
			vPhysicPosition.z = 0f;
			break;
		case Display.UPPER_LEFT:
			vPhysicPosition.x = positionPhysic.size.x / 2f;
			vPhysicPosition.y = positionPhysic.size.y / 2f * -1f;
			vPhysicPosition.z = 0f;
			break;
		case Display.TOP:
			vPhysicPosition.x = 0f;
			vPhysicPosition.y = positionPhysic.size.y / 2f * -1f;
			vPhysicPosition.z = 0f;
			break;
		case Display.UPPER_RIGHT:
			vPhysicPosition.x = positionPhysic.size.x / 2f * -1f;
			vPhysicPosition.y = positionPhysic.size.y / 2f * -1f;
			vPhysicPosition.z = 0f;
			break;
		case Display.RIGHT:
			vPhysicPosition.x = positionPhysic.size.x / 2f * -1f;
			vPhysicPosition.y = 0f;
			vPhysicPosition.z = 0f;
			break;
		case Display.BOTTOM_RIGHT:
			vPhysicPosition.x = positionPhysic.size.x / 2f * -1f;
			vPhysicPosition.y = positionPhysic.size.y / 2f;
			vPhysicPosition.z = 0f;
			break;
		case Display.BOTTOM:
			vPhysicPosition.x = 0f;
			vPhysicPosition.y = positionPhysic.size.y / 2f;
			vPhysicPosition.z = 0f;
			break;
		case Display.BOTTOM_LEFT:
			vPhysicPosition.x = positionPhysic.size.x / 2f;
			vPhysicPosition.y = positionPhysic.size.y / 2f;
			vPhysicPosition.z = 0f;
			break;
		case Display.CENTER:
			vPhysicPosition.x = 0f;
			vPhysicPosition.y = 0f;
			vPhysicPosition.z = 0f;
			break;
		}
		vPhysicPosition.x += colliderOffset.x * m_TargetsGlobal.width;
		vPhysicPosition.y += colliderOffset.y * m_TargetsGlobal.height;
		vPhysicPosition.z += colliderOffset.z;
		positionPhysic.center = vPhysicPosition;
	}

	private void SetWidth()
	{
		if (!onlyHeight && !keepRatioY)
		{
			if (position == Position.RELATIVE)
			{
				widthParent = oParent.ObjectWidthParent(rotation);
			}
			else
			{
				widthParent = OScreenWidth;
			}
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.width = ConvertDimention(width, widthParent);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.maxWidth = ConvertDimention(width, widthParent);
				break;
			case TargetType.UI_SCROLL_LIST:
				m_TargetScrollList.viewableArea.x = ConvertDimention(width, widthParent);
				m_TargetScrollList.SetViewableAreaPixelDimensions(oCamera, (int)m_TargetScrollList.viewableArea.x, (int)m_TargetScrollList.viewableArea.y);
				break;
			case TargetType.GAMEOBJECT:
			{
				float num = 0f;
				vCurrentScale = m_Transform.localScale;
				num = ConvertDimention(width, widthParent);
				vCurrentScale.x = num / ObjectWidth;
				vCurrentScale.z = vCurrentScale.x;
				m_Transform.localScale = vCurrentScale;
				break;
			}
			}
		}
		else if (keepRatioY)
		{
			if (targetType == TargetType.GAMEOBJECT)
			{
				vCurrentScale = m_Transform.localScale;
			}
			width = Mathf.Abs(GetObjectHeight()) * ratioX / ratioY;
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.width = width;
				break;
			case TargetType.GAMEOBJECT:
				vCurrentScale.x = vCurrentScale.y * ratioX / ratioY;
				vCurrentScale.z = vCurrentScale.x;
				m_Transform.localScale = vCurrentScale;
				break;
			}
		}
		switch (targetType)
		{
		case TargetType.EZGUI_COMPONENT:
		case TargetType.UI_SLIDER:
		case TargetType.UI_TEXT_FIELD:
			if (flipX && m_TargetsGlobal.width > 0f)
			{
				m_TargetsGlobal.width *= -1f;
			}
			break;
		case TargetType.SPRITE_TEXT:
			if (flipX && m_TargetText.maxWidth > 0f)
			{
				m_TargetText.maxWidth *= -1f;
			}
			break;
		case TargetType.GAMEOBJECT:
		case TargetType.UI_PANEL:
		case TargetType.UI_SCROLL_LIST:
			break;
		}
	}

	private void SetHeight()
	{
		if (applyScreenDimensionToSpriteText)
		{
		}
		if (!onlyWidth && !keepRatioX)
		{
			if (position == Position.RELATIVE)
			{
				HeightParent = oParent.ObjectHeightParent(rotation);
			}
			else
			{
				HeightParent = OScreenHeight;
			}
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.height = ConvertDimention(height, HeightParent);
				break;
			case TargetType.SPRITE_TEXT:
				m_TargetText.SetCharacterSize(ConvertDimention(height, HeightParent));
				break;
			case TargetType.UI_SCROLL_LIST:
				m_TargetScrollList.viewableArea.y = ConvertDimention(height, HeightParent);
				m_TargetScrollList.SetViewableAreaPixelDimensions(oCamera, (int)m_TargetScrollList.viewableArea.x, (int)m_TargetScrollList.viewableArea.y);
				break;
			case TargetType.GAMEOBJECT:
			{
				float num = 0f;
				vCurrentScale = m_Transform.localScale;
				num = ConvertDimention(height, HeightParent);
				vCurrentScale.y = num / ObjectHeight;
				m_Transform.localScale = vCurrentScale;
				break;
			}
			}
		}
		else if (keepRatioX)
		{
			if (targetType == TargetType.GAMEOBJECT)
			{
				vCurrentScale = m_Transform.localScale;
			}
			height = Mathf.Abs(GetObjectWidth()) * ratioY / ratioX;
			switch (targetType)
			{
			case TargetType.EZGUI_COMPONENT:
			case TargetType.UI_SLIDER:
			case TargetType.UI_TEXT_FIELD:
				m_TargetsGlobal.height = height;
				break;
			case TargetType.GAMEOBJECT:
				vCurrentScale.y = vCurrentScale.x * ratioY / ratioX;
				m_Transform.localScale = vCurrentScale;
				break;
			}
		}
		switch (targetType)
		{
		case TargetType.EZGUI_COMPONENT:
		case TargetType.UI_SLIDER:
		case TargetType.UI_TEXT_FIELD:
			if (flipY && m_TargetsGlobal.height > 0f)
			{
				m_TargetsGlobal.height *= -1f;
			}
			break;
		case TargetType.SPRITE_TEXT:
			if (flipY && m_TargetText.characterSize > 0f)
			{
				m_TargetText.characterSize *= -1f;
			}
			break;
		case TargetType.GAMEOBJECT:
		case TargetType.UI_PANEL:
		case TargetType.UI_SCROLL_LIST:
			break;
		}
	}

	private void SetCollider()
	{
		BoxCollider boxCollider = null;
		switch (targetType)
		{
		case TargetType.EZGUI_COMPONENT:
		case TargetType.UI_SLIDER:
		case TargetType.UI_TEXT_FIELD:
			boxCollider = m_TargetsGlobal.GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				SetColliderSize(boxCollider);
				SetColliderPosition(boxCollider);
				m_TargetsGlobal.m_StylesheetCollider = true;
			}
			break;
		case TargetType.UI_SCROLL_LIST:
			boxCollider = m_TargetScrollList.GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				SetColliderSizeScrolllist(boxCollider, m_TargetScrollList.viewableArea.x, m_TargetScrollList.viewableArea.y);
			}
			break;
		case TargetType.GAMEOBJECT:
		case TargetType.SPRITE_TEXT:
		case TargetType.UI_PANEL:
			break;
		}
	}

	private float Convert(float fToConvert, float screenDimention)
	{
		if (percent)
		{
			fToConvert = screenDimention * (fToConvert / 100f);
		}
		return fToConvert;
	}

	private float ConvertDimention(float fToConvert, float screenDimention)
	{
		if (percentDimention)
		{
			fToConvert = screenDimention * (fToConvert / 100f);
		}
		return fToConvert;
	}

	private void SetZIndex()
	{
		vCurrentLocalPosition.x = m_Transform.localPosition.x;
		vCurrentLocalPosition.y = m_Transform.localPosition.y;
		vCurrentLocalPosition.z = zIndex;
		m_Transform.localPosition = vCurrentLocalPosition;
	}

	private void SetKnobSize()
	{
		widthParent = 0f;
		HeightParent = 0f;
		if (position == Position.RELATIVE)
		{
			widthParent = ObjectHeight;
			HeightParent = ObjectHeight;
		}
		else
		{
			widthParent = oScreenHeight;
			HeightParent = oScreenHeight;
		}
		sizeWidthKnob = ConvertDimention(widthKnob, HeightParent);
		sizeHeightKnob = ConvertDimention(heightKnob, HeightParent);
		if (forceSquare)
		{
			vSizeKnob.y = sizeWidthKnob;
		}
		else
		{
			vSizeKnob.y = sizeHeightKnob;
		}
		vSizeKnob.x = sizeWidthKnob;
		vSizeKnob.y = sizeHeightKnob;
		GetComponent<UISlider>().knobSize = vSizeKnob;
	}

	private void SetCaretSize()
	{
		widthParent = 0f;
		HeightParent = 0f;
		if (position == Position.RELATIVE)
		{
			widthParent = ObjectWidth;
			HeightParent = ObjectHeight;
		}
		else
		{
			widthParent = oScreenWidth;
			HeightParent = oScreenHeight;
		}
		sizeWidthCaret = ConvertDimention(widthCaret, widthParent);
		sizeHeightCaret = ConvertDimention(heightCaret, HeightParent);
		vSizeCaret.x = sizeWidthCaret;
		vSizeCaret.y = sizeHeightCaret;
		GetComponent<UITextField>().caretSize = vSizeCaret;
	}

	private void ConvertRelative(float numberToConvert, DimensionType dimentionType)
	{
		if (position == Position.RELATIVE)
		{
			switch (dimentionType)
			{
			case DimensionType.WIDTH:
				switch (oParent.DisplayPosition)
				{
				case Display.LEFT:
				case Display.BOTTOM_LEFT:
				case Display.UPPER_LEFT:
					if (numberToConvert == 0f)
					{
						numberToConvert = 0f;
					}
					else if (numberToConvert == sizeWidth / 2f)
					{
						numberToConvert = oParent.ObjectWidthParent(rotation) / 2f;
					}
					else if (numberToConvert == sizeWidth)
					{
						numberToConvert = oParent.ObjectWidthParent(rotation);
					}
					break;
				case Display.TOP:
				case Display.BOTTOM:
				case Display.CENTER:
					if (numberToConvert == 0f)
					{
						numberToConvert = oParent.ObjectWidthParent(rotation) / 2f * -1f;
					}
					else if (numberToConvert == sizeWidth / 2f)
					{
						numberToConvert = 0f;
					}
					else if (numberToConvert == sizeWidth)
					{
						numberToConvert = oParent.ObjectWidthParent(rotation) / 2f;
					}
					break;
				case Display.RIGHT:
				case Display.BOTTOM_RIGHT:
				case Display.UPPER_RIGHT:
					if (numberToConvert == 0f)
					{
						numberToConvert = oParent.ObjectWidthParent(rotation) * -1f;
					}
					else if (numberToConvert == sizeWidth / 2f)
					{
						numberToConvert = oParent.ObjectWidthParent(rotation) / 2f * -1f;
					}
					else if (numberToConvert == sizeWidth)
					{
						numberToConvert = 0f;
					}
					break;
				}
				oCurrentScreenPosition.x = numberToConvert;
				break;
			case DimensionType.HEIGHT:
				switch (oParent.DisplayPosition)
				{
				case Display.TOP:
				case Display.UPPER_LEFT:
				case Display.UPPER_RIGHT:
					if (numberToConvert == 0f)
					{
						numberToConvert = oParent.ObjectHeightParent(rotation) * -1f;
					}
					else if (numberToConvert == sizeHeight / 2f)
					{
						numberToConvert = oParent.ObjectHeightParent(rotation) / 2f * -1f;
					}
					else if (numberToConvert == sizeHeight)
					{
						numberToConvert = 0f;
					}
					break;
				case Display.RIGHT:
				case Display.LEFT:
				case Display.CENTER:
					if (numberToConvert == 0f)
					{
						numberToConvert = oParent.ObjectHeightParent(rotation) / 2f * -1f;
					}
					else if (numberToConvert == sizeHeight / 2f)
					{
						numberToConvert = 0f;
					}
					else if (numberToConvert == sizeHeight)
					{
						numberToConvert = oParent.ObjectHeightParent(rotation) / 2f;
					}
					break;
				case Display.BOTTOM:
				case Display.BOTTOM_RIGHT:
				case Display.BOTTOM_LEFT:
					if (numberToConvert == 0f)
					{
						numberToConvert = 0f;
					}
					else if (numberToConvert == sizeHeight / 2f)
					{
						numberToConvert = oParent.ObjectHeightParent(rotation) / 2f;
					}
					else if (numberToConvert == sizeHeight)
					{
						numberToConvert = oParent.ObjectHeightParent(rotation);
					}
					break;
				}
				oCurrentScreenPosition.y = numberToConvert;
				break;
			}
		}
		else if (position == Position.ABSOLUTE)
		{
			switch (dimentionType)
			{
			case DimensionType.WIDTH:
				oCurrentScreenPosition.x = numberToConvert;
				break;
			case DimensionType.HEIGHT:
				oCurrentScreenPosition.y = numberToConvert;
				break;
			}
		}
	}

	private void ResetAnchorPos()
	{
		switch (targetType)
		{
		case TargetType.EZGUI_COMPONENT:
		case TargetType.UI_SLIDER:
		case TargetType.UI_TEXT_FIELD:
			m_TargetsGlobal.SetAnchor(m_TargetsGlobal.anchor);
			break;
		case TargetType.SPRITE_TEXT:
			m_TargetText.SetAnchor(m_TargetText.anchor);
			break;
		case TargetType.GAMEOBJECT:
		case TargetType.UI_PANEL:
		case TargetType.UI_SCROLL_LIST:
			break;
		}
	}

	public float ObjectWidthParent(Rotation i_RotationChild)
	{
		float num = 0f;
		int num2 = (int)i_RotationChild + (int)rotation;
		if (num2 % 2 > 0)
		{
			return GetObjectHeight();
		}
		return GetObjectWidth();
	}

	public float ObjectHeightParent(Rotation i_RotationChild)
	{
		float num = 0f;
		int num2 = (int)i_RotationChild + (int)rotation;
		if (num2 % 2 > 0)
		{
			return GetObjectWidth();
		}
		return GetObjectHeight();
	}

	private float GetObjectWidth()
	{
		float result = 0f;
		switch (targetType)
		{
		case TargetType.EZGUI_COMPONENT:
		case TargetType.UI_SLIDER:
		case TargetType.UI_TEXT_FIELD:
			result = m_TargetsGlobal.width;
			break;
		case TargetType.SPRITE_TEXT:
			result = m_TargetText.maxWidth;
			break;
		case TargetType.UI_SCROLL_LIST:
			result = m_TargetScrollList.viewableArea.x;
			break;
		case TargetType.GAMEOBJECT:
			result = SizeBox.x;
			break;
		}
		return result;
	}

	private float GetObjectHeight()
	{
		float result = 0f;
		switch (targetType)
		{
		case TargetType.EZGUI_COMPONENT:
		case TargetType.UI_SLIDER:
		case TargetType.UI_TEXT_FIELD:
			result = m_TargetsGlobal.height;
			break;
		case TargetType.SPRITE_TEXT:
			result = m_TargetText.characterSize;
			break;
		case TargetType.UI_SCROLL_LIST:
			result = m_TargetScrollList.viewableArea.y;
			break;
		case TargetType.GAMEOBJECT:
			result = SizeBox.y;
			break;
		}
		return result;
	}

	public void SetLayerRecursivly(int i_Layer)
	{
		m_MyGameObject.layer = i_Layer;
		if (targetType == TargetType.GAMEOBJECT)
		{
			foreach (Transform item in m_Transform)
			{
				if (item != m_Transform)
				{
					SetLayerRecursivly(i_Layer, item);
				}
			}
			return;
		}
		foreach (StyleSheet item2 in m_ListStyleSheetRelative)
		{
			if (item2 != this)
			{
				item2.SetLayerRecursivly(i_Layer);
			}
		}
	}

	private void SetLayerRecursivly(int i_Layer, Transform i_Child)
	{
		i_Child.gameObject.layer = i_Layer;
		foreach (Transform item in i_Child)
		{
			if (item != i_Child)
			{
				SetLayerRecursivly(i_Layer, item);
			}
		}
	}

	public static Vector2 GetButtonHeightWidth(float i_Width, float i_Height, Camera i_Cam)
	{
		float num = Screen.width;
		float num2 = Screen.height;
		float num3 = i_Cam.orthographicSize * 2f;
		float num4 = num3 * (num / num2);
		Vector2 result = default(Vector2);
		result.x = num4 * i_Width;
		result.y = num3 * i_Height;
		return result;
	}

	public static Vector3 GetButtonPosWithMargin(Camera i_Cam, Display i_Display, float i_Depth, float i_MarginLeft, float i_MarginRight, float i_MarginBot, float i_MarginTop)
	{
		float y = 0f;
		float x = 0f;
		switch (i_Display)
		{
		case Display.BOTTOM:
			x = 0.5f + i_MarginLeft - i_MarginRight;
			y = i_MarginBot - i_MarginTop;
			break;
		case Display.BOTTOM_LEFT:
			x = i_MarginLeft - i_MarginRight;
			y = i_MarginBot - i_MarginTop;
			break;
		case Display.BOTTOM_RIGHT:
			x = 1f + i_MarginLeft - i_MarginRight;
			y = i_MarginBot - i_MarginTop;
			break;
		case Display.CENTER:
			x = 0.5f + i_MarginLeft - i_MarginRight;
			y = 0.5f + i_MarginBot - i_MarginTop;
			break;
		case Display.LEFT:
			x = i_MarginLeft - i_MarginRight;
			y = 0.5f + i_MarginBot - i_MarginTop;
			break;
		case Display.RIGHT:
			x = 1f + i_MarginLeft - i_MarginRight;
			y = 0.5f + i_MarginBot - i_MarginTop;
			break;
		case Display.TOP:
			x = 0.5f + i_MarginLeft - i_MarginRight;
			y = 1f + i_MarginBot - i_MarginTop;
			break;
		case Display.UPPER_LEFT:
			x = i_MarginLeft - i_MarginRight;
			y = 1f + i_MarginBot - i_MarginTop;
			break;
		case Display.UPPER_RIGHT:
			x = 1f + i_MarginLeft - i_MarginRight;
			y = 1f + i_MarginBot - i_MarginTop;
			break;
		}
		return i_Cam.ViewportToWorldPoint(new Vector3(x, y, i_Depth - i_Cam.transform.position.z));
	}
}
