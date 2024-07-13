using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipManager : SingletonMonoBehaviour
{
	public List<DragMultiTarget> dragMultiTarget;

	[NonSerialized]
	public Transform moneyDestinationPoint;

	public GameObject moneyDrop;

	public Transform enemyProjectileTarget;

	public List<Shooter> shooter;

	public GameObject shooterPrefab;

	private Shooter shooterA;

	public TurretCapsuleManager turretCapsuleManager;

	public ShipVisual shipVisual;

	[NonSerialized]
	public GameObject muzzleFlash;

	public GameObject muzzleFlashParent;

	public PowerupHolder[] powerupHolders;

	public PowerupData extraMagicItemSlotData;

	private PowerupHolder[] _unlockedPowerupHolders;

	public MeshRenderer powerupHandRenderer;

	public int lastComboCount;

	public static ShipManager instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<ShipManager>();
		}
	}

	public IEnumerable<PowerupHolder> UnlockedPowerupHolders
	{
		get
		{
			return _unlockedPowerupHolders;
		}
	}

	public bool PowerupTutorialHandRendererEnabled
	{
		get
		{
			return powerupHandRenderer.enabled;
		}
		set
		{
			powerupHandRenderer.enabled = value;
		}
	}

	public bool isShooting
	{
		get
		{
			for (int i = 0; i < shooter.Count; i++)
			{
				if (shooter[i].isShooting)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool hasTargets
	{
		get
		{
			for (int i = 0; i < shooter.Count; i++)
			{
				if (shooter[i].targetQueue.Count > 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		shooter = new List<Shooter>();
		moneyDestinationPoint = GameObject.Find("MoneyDestination").transform;
		int num = 1;
		if (PqmtScreenGestures.m_instancePQMT != null)
		{
			num = PqmtScreenGestures.m_instancePQMT.mMaxFingers;
		}
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(shooterPrefab, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.name = "ShooterA";
			shooterA = gameObject.GetComponent<Shooter>();
			if (i == 0)
			{
				shooterA.weaponVisual = shipVisual;
			}
			shooter.Add(shooterA);
			dragMultiTarget[i].targetQueue = shooter[i].targetQueue;
		}
		_unlockedPowerupHolders = new PowerupHolder[1] { powerupHolders[0] };
	}

	public void Start()
	{
		CreateMuzzleFlash();
		PowerupTutorialHandRendererEnabled = false;
	}

	private void OnEnable()
	{
		GameManager.GameStateChanged += HandleGameManagerGameStateChanged;
	}

	private void OnDisable()
	{
		GameManager.GameStateChanged -= HandleGameManagerGameStateChanged;
	}

	private void HandleGameManagerGameStateChanged(object sender, GameManager.GameStateChangedEventArgs e)
	{
		if (e.NewState != GameManager.GameState.Playing)
		{
			PowerupTutorialHandRendererEnabled = false;
		}
	}

	public PowerupHolder GetAvailablePowerupHolder()
	{
		PowerupHolder powerupHolder = _unlockedPowerupHolders.Where((PowerupHolder holder) => holder.State == PowerupStates.hidden).FirstOrDefault();
		if (powerupHolder == null)
		{
			powerupHolder = (from holder in _unlockedPowerupHolders
				where holder.State == PowerupStates.active
				orderby holder.LastQueuedDateTime
				select holder).FirstOrDefault();
		}
		if (powerupHolder == null)
		{
			powerupHolder = (from holder in _unlockedPowerupHolders
				where holder.State == PowerupStates.ready
				orderby holder.LastQueuedDateTime
				select holder).First();
		}
		return powerupHolder;
	}

	public void EnableTargetting(bool i_ActivateRenderer = true)
	{
		for (int i = 0; i < dragMultiTarget.Count; i++)
		{
			dragMultiTarget[i].enabled = true;
			if (i_ActivateRenderer)
			{
				dragMultiTarget[i].GetComponent<Renderer>().enabled = true;
			}
		}
	}

	public void DisableTargetting()
	{
		for (int i = 0; i < dragMultiTarget.Count; i++)
		{
			dragMultiTarget[i].enabled = false;
			dragMultiTarget[i].GetComponent<Renderer>().enabled = false;
		}
	}

	public void ResetTargetting()
	{
		for (int i = 0; i < dragMultiTarget.Count; i++)
		{
			dragMultiTarget[i].Reset();
		}
	}

	public void CreateMuzzleFlash()
	{
		muzzleFlash = (GameObject)UnityEngine.Object.Instantiate(StartGameSettings.Instance.activeSkylander.elementData.LoadMuzzleFlashPrefab());
		muzzleFlash.transform.parent = muzzleFlashParent.transform;
		muzzleFlash.transform.localRotation = Quaternion.identity;
		muzzleFlash.transform.localPosition = Vector3.zero;
		muzzleFlash.layer = Layers.LitHud;
		shooter[0].muzzleFlash = muzzleFlash;
	}

	public void StartFiring(int i_Index)
	{
		DebugScreen.Log("FireAtTargets" + i_Index);
		shooter[i_Index].FireAtTargets();
	}

	public bool inTargetQueue(GameObject go)
	{
		for (int i = 0; i < shooter.Count; i++)
		{
			if (shooter[i].targetQueue.Contains(go))
			{
				return true;
			}
		}
		return false;
	}

	public void SwapShooters()
	{
	}

	public void RemoveTarget(GameObject i_GameObject)
	{
		for (int i = 0; i < shooter.Count; i++)
		{
			shooter[i].targetQueue.RemoveGameObject(i_GameObject);
		}
	}

	public void StopFiring()
	{
		for (int i = 0; i < shooter.Count; i++)
		{
			shooter[i].StopAutoFire();
		}
	}
}
