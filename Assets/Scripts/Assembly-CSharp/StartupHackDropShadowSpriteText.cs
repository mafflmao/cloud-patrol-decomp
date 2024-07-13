public class StartupHackDropShadowSpriteText : StartupHackLocalizedSpriteText
{
	public SpriteText shadowText;

	public bool autoSizeText;

	private float? _autoSizePreferredSize;

	public float autoSizeMinimumSize = 20f;

	public float autoSizeMaximumSingleLineWidth = 500f;

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			float? autoSizePreferredSize = _autoSizePreferredSize;
			if (!autoSizePreferredSize.HasValue)
			{
				_autoSizePreferredSize = characterSize;
			}
			if (autoSizeText)
			{
				DynamicFontSizeSpriteText.SetTextAndAutoSize(this, value, autoSizeMinimumSize, _autoSizePreferredSize.Value, autoSizeMaximumSingleLineWidth, SetBaseTextAndShadowText);
			}
			else
			{
				SetBaseTextAndShadowText(value);
			}
		}
	}

	public override void Start()
	{
		base.Start();
		if (shadowText != null)
		{
			shadowText.SetAlignment(alignment);
			shadowText.maxWidth = maxWidth;
			shadowText.SetAnchor(anchor);
		}
	}

	private void SetBaseTextAndShadowText(string value)
	{
		base.Text = value;
		if (shadowText != null)
		{
			shadowText.Text = value;
		}
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);
		if (shadowText != null)
		{
			shadowText.Hide(tf);
		}
	}

	public override void SetCharacterSize(float size)
	{
		base.SetCharacterSize(size);
		if (shadowText != null)
		{
			shadowText.SetCharacterSize(size);
		}
	}

	private new void OnDrawGizmosSelected()
	{
		DynamicFontSizeSpriteText.DrawSizeGizmo(autoSizeText, autoSizeMaximumSingleLineWidth, anchor, base.gameObject);
	}
}
