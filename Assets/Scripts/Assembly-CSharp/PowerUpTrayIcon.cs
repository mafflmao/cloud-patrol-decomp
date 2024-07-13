using UnityEngine;

public class PowerUpTrayIcon : MonoBehaviour
{
	public PackedSprite packedSprite;

	public UIButton3D button;

	private PowerUpStoreController _controller;

	public int index = -1;

	private PowerupData _data;

	public PowerupData data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
			UpdateIcon();
		}
	}

	private void Awake()
	{
		_controller = (PowerUpStoreController)Object.FindObjectOfType(typeof(PowerUpStoreController));
	}

	private void Start()
	{
		button.scriptWithMethodToInvoke = this;
		button.methodToInvoke = "OnTrayIconBtnClick";
		button.controlIsEnabled = false;
	}

	private void UpdateIcon()
	{
		if (_data == null)
		{
			button.controlIsEnabled = false;
			packedSprite.PlayAnim("None");
			return;
		}
		button.controlIsEnabled = true;
		if (packedSprite.GetAnim(data.storeSpriteName) != null)
		{
			packedSprite.PlayAnim(data.storeSpriteName);
		}
		else
		{
			packedSprite.PlayAnim("None");
		}
	}

	private void OnTrayIconBtnClick()
	{
		if (!(_controller != null))
		{
		}
	}
}
