using UnityEngine;

public class HealingElixirScreen : MonoBehaviour
{
	public DropShadowSpriteText costDisplay;

	public DropShadowSpriteText totalDisplay;

	public UIButtonComposite continueButton;

	public DropShadowSpriteText continueCountdown;

	public DropShadowSpriteText tokenCount;

	public PowerupData healingElixirData;

	public SoundEventData purchaseSuccessSFX;

	public SoundEventData purchaseFailSFX;

	public SoundEventData purchaseWithGemsSFX;

	public PackedSprite treasureChest;

	public PackedSprite[] totalGemsIcon;

	public PackedSprite coinIcon;

	public PackedSprite gemIcon;

	public int m_MaxContinue;

	public int m_TokenNeededToContinue;

	private bool _isClosing;

	private bool _isButtonPressed;

	private float _introTime = 1f;

	private float _outroTime = 0.5f;

	private float _lingerTime = 10f;

	private string _textContinue;

	private OutOfFuelMessage _outOfFuelMessage;

	private float _timeLeft;

	private int _numElixirsUsed;

	private int m_TokenEntered = 4;

	public static bool IsActive { get; private set; }

	private void Awake()
	{
		IsActive = true;
	}

	private void Start()
	{
		HealingElixir.WasUsed = false;
		m_TokenEntered = 0;
		UpdateTokenText();
		GameManager.KillAllProjectiles();
		float num = healingElixirData.GetValueForLevel(healingElixirData.GetLevel());
		HealingElixirDiscountUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<HealingElixirDiscountUpgrade>();
		if (passiveUpgradeOrDefault != null)
		{
			num += passiveUpgradeOrDefault.percentDiscount;
			Debug.Log("Modified discount is " + num);
		}
		HealingElixir.percentCost = Mathf.Clamp01(1f - num);
		HealingElixir.Reset();
		_numElixirsUsed = GameManager.sessionStats.totalElixirsUsed;
		ExtraHealingElixirUpgrade passiveUpgradeOrDefault2 = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<ExtraHealingElixirUpgrade>();
		bool flag = _numElixirsUsed == 0 || (_numElixirsUsed == 1 && passiveUpgradeOrDefault2 != null);
		gemIcon.GetComponent<Renderer>().enabled = !flag;
		coinIcon.GetComponent<Renderer>().enabled = flag;
		costDisplay.Text = ((!flag) ? HealingElixir.GetGemCost() : HealingElixir.GetCoinCost()).ToString("n0");
		totalDisplay.Text = 0.ToString("n0");
		treasureChest.Hide(!flag);
		PackedSprite[] array = totalGemsIcon;
		foreach (PackedSprite packedSprite in array)
		{
			packedSprite.Hide(flag);
		}
		totalDisplay.SetColor((!flag) ? new Color(0.39f, 0.85f, 1f, 1f) : new Color(0.9f, 1f, 0.39f, 1f));
		_textContinue = LocalizationManager.Instance.GetString("GAME_CONTINUE");
		_timeLeft = _lingerTime + _introTime;
		base.transform.localScale = new Vector3(0f, 0f, 0f);
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", _introTime));
		_outOfFuelMessage = base.transform.parent.gameObject.GetComponent<OutOfFuelMessage>();
		_outOfFuelMessage.lingerTime = _lingerTime;
		_outOfFuelMessage.fadeInTime = _introTime;
		_outOfFuelMessage.fadeOutTime = _outroTime;
		InvokeHelper.InvokeSafe(ScaleAndClose, _lingerTime + _introTime, this);
	}

	private void OnDestroy()
	{
		IsActive = false;
	}

	private void Update()
	{
		CheckTokenInserted();
		if (!_isButtonPressed)
		{
			_timeLeft -= Time.deltaTime;
			_timeLeft = Mathf.Clamp(_timeLeft, -1f, _lingerTime);
			int num = Mathf.RoundToInt(_timeLeft);
			if (num >= 0 && _timeLeft < _lingerTime + _introTime)
			{
				continueCountdown.Text = _textContinue + " " + num;
			}
		}
	}

	private void CheckTokenInserted()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			m_TokenEntered++;
			UpdateTokenText();
			if (m_TokenEntered >= m_TokenNeededToContinue)
			{
				OnContinue();
			}
		}
	}

	private void UpdateTokenText()
	{
		tokenCount.Text = m_TokenEntered + "/" + m_TokenNeededToContinue;
	}

	private void OnContinue()
	{
		HealingElixir.WasUsed = true;
		SoundEventManager.Instance.Play2D(purchaseSuccessSFX);
		_isButtonPressed = true;
		InvokeHelper.InvokeSafe(ScaleAndClose, 0.5f, this);
	}

	private void ScaleAndClose()
	{
		if (!_isClosing)
		{
			_isClosing = true;
			continueButton.UIButton3D.controlIsEnabled = false;
			continueButton.boxCollider.enabled = false;
			iTween.ScaleTo(base.gameObject, iTween.Hash("scale", new Vector3(0f, 0f, 0f), "time", _outroTime));
			InvokeHelper.InvokeSafe(KillParent, _outroTime, this);
		}
	}

	private void KillParent()
	{
		IsActive = false;
		_outOfFuelMessage.HideScore();
		if (HealingElixir.WasUsed)
		{
			if (WackAManager.IsActive)
			{
				MusicManager.Instance.PlayCurrentBossMusic();
			}
			else
			{
				MusicManager.Instance.PlayCurrentGameplayMusic();
			}
			ShipManager.instance.shipVisual.Revive();
			if (!LevelManager.Instance.IsTransitioning)
			{
				GameManager.ExplodeBombs();
				GameManager.KillAllEnemies();
			}
			GameManager.ExplodeProjectiles();
			GameManager.Instance.Continue();
		}
	}
}
