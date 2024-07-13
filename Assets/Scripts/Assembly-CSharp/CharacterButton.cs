using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UIButton3D))]
[RequireComponent(typeof(SimpleSprite))]
public class CharacterButton : MonoBehaviour
{
	public const float UV_WIDTH = 0.25f;

	public const float UV_HEIGHT = 21f / 64f;

	public SkylanderSelectController controller;

	public CharacterData cd;

	public SpriteText costText;

	public Scale3Grid costOverlay;

	public PackedSprite costGemIcon;

	public PackedSprite toyLinkedIcon;

	public PrefabPlaceholder newTagPlaceholder;

	public PrefabPlaceholder saleTagPlaceholder;

	public Scale3Grid claimableOverlay;

	public SpriteText claimableText;

	public PackedSprite background;

	public UIButton3D button;

	public PackedSprite skylanderPortrait;

	public Material portraitMaterial_Locked;

	public Material portraitMaterial_Unlocked;

	public SpriteText legendaryText;

	public Scale3Grid legendaryOverlay;

	public Scale3Grid comingSoonOverlay;

	private GameObject _newTagInstance;

	private GameObject _saleTagInstance;

	private void Start()
	{
		if (!cd.isLegendary)
		{
			if (cd.isGiant)
			{
				toyLinkedIcon.transform.localPosition += new Vector3(0f, 60f, 0f);
			}
			else
			{
				toyLinkedIcon.transform.localPosition += new Vector3(0f, 45f, 0f);
			}
		}
		UpdateGraphics();
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

	private void SetCostVisible(bool visible)
	{
		visible = false;
		costText.Text = cd.GemCost.ToString();
		costText.Hide(!visible);
		costOverlay.Hide(!visible);
		costGemIcon.Hide(!visible);
	}

	private void SetNewTagVisible(bool isVisible)
	{
		if (isVisible && _newTagInstance == null)
		{
			_newTagInstance = newTagPlaceholder.InstantiatePrefab();
		}
		else if (!isVisible && _newTagInstance != null)
		{
			UnityEngine.Object.Destroy(_newTagInstance);
		}
	}

	private void SetComingSoonVisible(bool visible)
	{
		comingSoonOverlay.Hide(!visible);
	}

	private void SetLegendaryOverlayVisible(bool isVisible)
	{
		legendaryText.Hide(!isVisible);
		legendaryOverlay.Hide(!isVisible);
	}

	public void UpdateGraphics()
	{
		button.controlIsEnabled = cd.IsReleased;
		SetLegendaryOverlayVisible(cd.isLegendary);
		if (!cd.IsReleased)
		{
			SetSkylanderPortraitLocked(true);
			SetCostVisible(false);
			SetNewTagVisible(false);
			SetComingSoonVisible(true);
			toyLinkedIcon.Hide(true);
			SetClaimableVisible(false);
		}
		else
		{
			CharacterUserData characterUserData = ElementDataManager.Instance.GetCharacterUserData(cd);
			bool isToyClaimable = characterUserData.IsToyClaimable;
			UpdateToyLinkIconState(characterUserData);
			SetClaimableVisible(isToyClaimable);
			UpdateSaleTag(characterUserData);
			bool flag = characterUserData.IsUnlocked || characterUserData.IsToyLinked;
			SetSkylanderPortraitLocked(!flag && !isToyClaimable);
			SetCostVisible(!flag && !isToyClaimable);
			SetNewTagVisible(cd.IsNew && !cd.IsUnlockOnSale);
			SetComingSoonVisible(false);
		}
	}

	private void SetSkylanderPortraitLocked(bool isLocked)
	{
		isLocked = false;
		background.PlayAnim((!isLocked) ? "Unlocked" : "Locked");
		background.SetColor((!isLocked) ? cd.elementData.skylanderSelectColor : Color.gray);
		if (skylanderPortrait != null)
		{
			skylanderPortrait.SetMaterial((!isLocked) ? portraitMaterial_Unlocked : portraitMaterial_Locked);
		}
	}

	private void OnCharacterBtnClick()
	{
		if (cd.IsReleased && controller != null)
		{
			controller.PlaySelectSound();
			UIManager.instance.blockInput = true;
			StartCoroutine(OnCharacterBtnClickRte());
		}
	}

	private void UpdateSaleTag(CharacterUserData characterUserData)
	{
		if (characterUserData.characterData.IsUpgradeOnSale || (!characterUserData.IsUnlocked && !characterUserData.IsToyLinked && characterUserData.characterData.IsUnlockOnSale) || DebugSettingsUI.forceSaleIcons)
		{
			if (_saleTagInstance == null)
			{
				_saleTagInstance = saleTagPlaceholder.InstantiatePrefab();
			}
		}
		else if (_saleTagInstance != null)
		{
			UnityEngine.Object.Destroy(_saleTagInstance);
		}
	}

	private void SetClaimableVisible(bool isVisible)
	{
		claimableOverlay.Hide(!isVisible);
		claimableText.Hide(!isVisible);
	}

	private void UpdateToyLinkIconState(CharacterUserData characterData)
	{
		if (characterData.IsUnlocked)
		{
			toyLinkedIcon.Hide(false);
			toyLinkedIcon.PlayAnim((!characterData.IsToyLinked) ? "Unlinked" : "Linked");
		}
		else
		{
			toyLinkedIcon.Hide(true);
		}
	}

	private IEnumerator OnCharacterBtnClickRte()
	{
		CommonAnimations.AnimateButtonRestore(base.gameObject);
		yield return new WaitForSeconds(0.3f);
		controller.OnCharacterBtnClick(cd);
	}
}
