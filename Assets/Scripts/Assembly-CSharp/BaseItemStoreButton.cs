using System;
using UnityEngine;

public class BaseItemStoreButton : MonoBehaviour
{
	public enum ButtonState
	{
		TRANSITION_IN = 0,
		NORMAL = 1,
		INACTIVE = 2,
		HIDDEN = 3,
		TRANSITION_OUT = 4
	}

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(BaseItemStoreButton), LogLevel.Log);

	public SpriteText txtGemPrice;

	public PackedSprite iconGem;

	public PrefabPlaceholder[] graphicPlaceholders;

	public Scale9Grid unlockedGlow;

	public PrefabPlaceholder saleTagPlaceholder;

	[HideInInspector]
	public PackedSprite[] graphics;

	[HideInInspector]
	public UIButton uiButton;

	[HideInInspector]
	public ButtonState state;

	[HideInInspector]
	public bool isLocked;

	[HideInInspector]
	public int price;

	[HideInInspector]
	public string graphicClip = string.Empty;

	protected PowerupData _itemData;

	private TweenPropagator _tweenProp;

	[HideInInspector]
	public SaleTag saleTag;

	public PowerupData powerupData
	{
		get
		{
			return _itemData;
		}
		set
		{
			_itemData = value;
			UpdateAppearance();
		}
	}

	public bool ShowUpgradeNotifyOverlaysIfAvailable { get; set; }

	public static event EventHandler<EventArgs> ButtonPressed;

	protected virtual void Awake()
	{
		graphics = new PackedSprite[graphicPlaceholders.Length];
		for (int i = 0; i < graphicPlaceholders.Length; i++)
		{
			graphics[i] = graphicPlaceholders[i].InstantiatePrefab().GetComponent<PackedSprite>();
		}
		ShowUpgradeNotifyOverlaysIfAvailable = true;
		uiButton = GetComponent<UIButton>();
		saleTag = saleTagPlaceholder.InstantiatePrefab().GetComponent<SaleTag>();
		UIManager.AddAction<BaseItemStoreButton>(uiButton, base.gameObject, "OnPowerUpBtnClick", POINTER_INFO.INPUT_EVENT.RELEASE);
		_tweenProp = GetComponent<TweenPropagator>();
	}

	private void OnEnable()
	{
		Bedrock.UnlockContentChanged += HandleUnlockContentChanged;
	}

	private void OnDisable()
	{
		Bedrock.UnlockContentChanged -= HandleUnlockContentChanged;
	}

	private void HandleUnlockContentChanged(object sender, EventArgs e)
	{
		UpdateGraphics();
	}

	public void UpdateGraphics()
	{
		UpdateAppearance();
	}

	public virtual void UpdateAppearance()
	{
		if (_itemData != null)
		{
			bool flag = powerupData.IsLocked && !powerupData.IsToyClaimable;
			uiButton.controlIsEnabled = !isLocked;
			for (int i = 0; i < graphics.Length; i++)
			{
				graphics[i].Hide(_itemData.storeSpriteGraphicIndex != i);
			}
			PackedSprite packedSprite = graphics[_itemData.storeSpriteGraphicIndex];
			string text = _itemData.storeSpriteName + ((!flag) ? string.Empty : "_Locked");
			if (packedSprite.GetAnim(text) != null)
			{
				packedSprite.PlayAnim(text);
			}
			else
			{
				Debug.LogError("Graphic does not have the animation '" + text + "'");
			}
			unlockedGlow.Hide(flag);
			iconGem.Hide(!flag);
			txtGemPrice.Hide(!flag);
			txtGemPrice.Text = _itemData.cost.ToString();
			saleTag.IsVisible = (flag && _itemData.isUnlockOnSale) || (_itemData.isUpgradeOnSale && !_itemData.IsAtMaxLevel) || DebugSettingsUI.forceSaleIcons;
		}
	}

	public void SelectButton()
	{
		uiButton.controlIsEnabled = false;
		uiButton.SetState(2);
	}

	public void DeselectButton()
	{
		uiButton.controlIsEnabled = true;
		uiButton.SetState(0);
	}

	private void OnPowerUpBtnClick()
	{
		_log.LogDebug("Magic Item Button clicked");
		OnButtonPressed();
	}

	private void OnButtonPressed()
	{
		if (BaseItemStoreButton.ButtonPressed != null)
		{
			BaseItemStoreButton.ButtonPressed(this, new EventArgs());
		}
	}

	public void DisableButton()
	{
		uiButton.controlIsEnabled = false;
		base.GetComponent<Collider>().enabled = false;
	}

	public void EnableButton()
	{
		uiButton.controlIsEnabled = !isLocked;
		_tweenProp.color = Color.white;
		base.GetComponent<Collider>().enabled = true;
		UpdateAppearance();
	}
}
