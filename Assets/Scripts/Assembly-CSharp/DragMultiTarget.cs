using System;
using System.Collections.Generic;
using UnityEngine;

public class DragMultiTarget : MonoBehaviour
{
	private const int enemiesLayerMask = 256;

	private const int comboLayerMask = 1048576;

	private const int layerMask = 1048832;

	private const float INVULNERABLE_TIME = 1f;

	public int dragFingerIndex;

	private bool isHidden;

	public bool fireOnRelease = true;

	public bool showCrosshairOnTouch = true;

	public bool startHidden = true;

	public bool moveOnTouch = true;

	public bool stopShootingOnFingerDown;

	public float zOffset;

	public bool isSelecting;

	public SoundEventData Target_SFX_Fail;

	public float lastFailSoundTime;

	public float failSoundTimeDelay = 2000f;

	[NonSerialized]
	public TargetQueue targetQueue;

	private Rect activeArea;

	public Vector2 previousScreenPos;

	private TrailRenderer _trailRenderer;

	private GhostSwords m_CurGhostSword;

	private float _time;

	private bool _triggerBomb = true;

	public GhostSwords GhostSword
	{
		get
		{
			return m_CurGhostSword;
		}
		set
		{
			m_CurGhostSword = value;
		}
	}

	private void Awake()
	{
		m_CurGhostSword = null;
		_trailRenderer = GetComponent<TrailRenderer>();
		activeArea = new Rect(0f, 0f, Screen.width, Screen.height);
	}

	private void OnDestroy()
	{
		GameManager.PauseChanged -= HandleGameManagerPauseChanged;
		GameManager.GameOver -= HandleGameOver;
	}

	private void HandleGameManagerPauseChanged(object sender, EventArgs e)
	{
		SetEnabledFromGameState();
	}

	public void SetEnabledFromGameState()
	{
		base.enabled = !GameManager.Instance.IsPaused && GameManager.gameState == GameManager.GameState.Playing && !RocketBooster.IsActive;
	}

	public void Reset()
	{
		isSelecting = false;
		SetEnabledFromGameState();
	}

	private void HandleGameOver(object sender, EventArgs e)
	{
		SetEnabledFromGameState();
		MeshRenderer component = GetComponent<MeshRenderer>();
		component.enabled = false;
	}

	private void Start()
	{
		GameManager.PauseChanged += HandleGameManagerPauseChanged;
		GameManager.GameOver += HandleGameOver;
		if (startHidden)
		{
			BroadcastMessage("Hide");
			isHidden = true;
		}
	}

	private void Update()
	{
		BombSliceGhostSwordsUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<BombSliceGhostSwordsUpgrade>();
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		if (isSelecting)
		{
			_triggerBomb = true;
			List<RaycastHit> list = RaycastComparer.Sort(GetHits(vector));
			foreach (RaycastHit item in list)
			{
				GameObject gameObject = item.collider.gameObject;
				Hazard component = gameObject.GetComponent<Hazard>();
				bool flag = component != null && component.isActive;
				if (!flag && !ShipManager.instance.inTargetQueue(gameObject))
				{
					if (GameManager.gunSlotCount - targetQueue.Count > 0)
					{
						Health component2 = gameObject.GetComponent<Health>();
						if (component2 != null && m_CurGhostSword == null)
						{
							DebugScreen.Log("AddTarget" + dragFingerIndex);
							targetQueue.AddTarget(gameObject);
						}
					}
					else if (lastFailSoundTime <= Time.time)
					{
						SoundEventManager.Instance.Play(Target_SFX_Fail, base.gameObject);
						lastFailSoundTime = Time.time + failSoundTimeDelay;
						targetQueue.PlayCrosshairFeedback();
					}
					_triggerBomb = false;
				}
				if (TryCollectLoot(gameObject) || TryCollectComboCoin(gameObject) || TryCollectMagicItem(gameObject))
				{
					_triggerBomb = false;
				}
				if (!flag)
				{
					continue;
				}
				if (!_triggerBomb)
				{
					_time = Time.time + 1f;
				}
				else if (_time <= Time.time)
				{
					if (m_CurGhostSword == null)
					{
						component.ApplyDamage();
					}
					else if (passiveUpgradeOrDefault == null)
					{
						m_CurGhostSword.BombHit();
					}
				}
			}
		}
		previousScreenPos = vector;
	}

	private bool TryCollectLoot(GameObject hitGameObject)
	{
		Loot component = hitGameObject.GetComponent<Loot>();
		if (component != null)
		{
			component.TakeHit();
			return true;
		}
		return false;
	}

	private bool TryCollectComboCoin(GameObject hitGameObject)
	{
		ComboCoin component = hitGameObject.GetComponent<ComboCoin>();
		if (component != null)
		{
			component.Pop();
			return true;
		}
		return false;
	}

	private bool TryCollectMagicItem(GameObject hitGameObject)
	{
		MagicItemCollectable component = hitGameObject.GetComponent<MagicItemCollectable>();
		if (component != null)
		{
			component.Collect();
			return true;
		}
		return false;
	}

	private IEnumerable<RaycastHit> GetHits(Vector3 screenPos)
	{
		IEnumerable<RaycastHit> result = new RaycastHit[0];
		if (isSelecting)
		{
			if (m_CurGhostSword == null)
			{
				result = PlaneCast.Comb(previousScreenPos, screenPos, 1048832);
			}
			else
			{
				Ray ray = Camera.main.ScreenPointToRay(screenPos);
				result = Physics.RaycastAll(ray, PlaneCast.CastDistance, 1048832);
			}
		}
		return result;
	}

	private void OnEnable()
	{
		isSelecting = false;
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
	}

	private void OnDisable()
	{
		Hide();
		isSelecting = false;
		FingerGestures.OnFingerMove -= FingerGestures_OnFingerMove;
		FingerGestures.OnFingerStationary -= FingerGestures_OnStationary;
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
	}

	private void FingerGestures_OnStationary(int fingerIndex, Vector2 fingerPos, float fingerDownTime)
	{
		if (fingerIndex == dragFingerIndex && isSelecting)
		{
			MoveToFingerPosition(fingerPos);
		}
	}

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		if (fingerIndex != dragFingerIndex)
		{
			return;
		}
		Show();
		if (isSelecting || !activeArea.Contains(fingerPos))
		{
			return;
		}
		if (!isSelecting && moveOnTouch)
		{
			MoveToFingerPosition(fingerPos);
		}
		if (showCrosshairOnTouch)
		{
			Show();
			BroadcastMessage("Show");
			isSelecting = true;
		}
		else
		{
			GameObject gameObject = PickObject(fingerPos);
			if (gameObject == base.gameObject)
			{
				isSelecting = true;
			}
		}
		previousScreenPos = fingerPos;
		FingerGestures.OnFingerStationary += FingerGestures_OnStationary;
		FingerGestures.OnFingerMove += FingerGestures_OnFingerMove;
	}

	private void FingerGestures_OnFingerMove(int fingerIndex, Vector2 fingerPos)
	{
		if (fingerIndex == dragFingerIndex)
		{
			base.transform.position = GetWorldPos(fingerPos);
		}
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float fingerDownTime)
	{
		if (fingerIndex == dragFingerIndex)
		{
			Hide();
			ReleaseAndFire();
		}
	}

	public void ReleaseAndFire()
	{
		BroadcastMessage("Hide");
		FingerGestures.OnFingerMove -= FingerGestures_OnFingerMove;
		FingerGestures.OnFingerStationary -= FingerGestures_OnStationary;
		isSelecting = false;
		if (fireOnRelease)
		{
			ShipManager.instance.StartFiring(dragFingerIndex);
		}
	}

	public Vector3 GetWorldPos(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		float distance = zOffset / ray.direction.z;
		return ray.GetPoint(distance);
	}

	public static GameObject PickObject(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			return hitInfo.collider.gameObject;
		}
		return null;
	}

	public void Hide()
	{
		base.gameObject.GetComponent<Renderer>().enabled = false;
		_trailRenderer.enabled = false;
		isHidden = true;
	}

	public void Show()
	{
		base.gameObject.GetComponent<Renderer>().enabled = true;
		_trailRenderer.enabled = true;
		isHidden = false;
	}

	public void MoveToFingerPosition(Vector2 aFingerPos)
	{
		base.transform.position = GetWorldPos(aFingerPos);
	}

	public bool IsHidden()
	{
		return isHidden;
	}
}
