using System;
using UnityEngine;

public class DynamicFontSizeSpriteText : SpriteText
{
	public bool m_shrinkToFit = true;

	public float m_minCharacterSize = 0.2f;

	public float m_maxWidthSingleLine;

	private float? m_preferedCharacterSize;

	public float MaxWidthSingleLine
	{
		get
		{
			return m_maxWidthSingleLine;
		}
		set
		{
			m_maxWidthSingleLine = value;
		}
	}

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			if (!m_preferedCharacterSize.HasValue)
			{
				m_preferedCharacterSize = characterSize;
			}
			else if (characterSize != m_preferedCharacterSize.Value)
			{
				SetCharacterSize(m_preferedCharacterSize.Value);
			}
			if (m_shrinkToFit && m_maxWidthSingleLine > 0f)
			{
				SetTextAndAutoSize(this, value, m_minCharacterSize, m_preferedCharacterSize.Value, m_maxWidthSingleLine, SetBaseText);
			}
			else
			{
				SetBaseText(value);
			}
		}
	}

	public override void Start()
	{
		base.Start();
		if (Application.isPlaying)
		{
			Text = LocalizationManager.Instance.GetString(Text);
		}
	}

	private void SetBaseText(string value)
	{
		base.Text = value;
	}

	private new void OnDrawGizmosSelected()
	{
		DrawSizeGizmo(m_shrinkToFit, m_maxWidthSingleLine, anchor, base.gameObject);
	}

	public static void SetTextAndAutoSize(SpriteText spriteText, string text, float minimumCharacterSize, float preferredCharacterSize, float maxWidthOfLine, Action<string> setTextDelegate)
	{
		if (spriteText.characterSize != preferredCharacterSize)
		{
			spriteText.SetCharacterSize(preferredCharacterSize);
		}
		setTextDelegate(text);
		float num = Mathf.Abs(spriteText.UnclippedBottomRight.x - spriteText.UnclippedTopLeft.x);
		if (num > maxWidthOfLine)
		{
			string text2 = text;
			float num2 = maxWidthOfLine / num;
			float num3 = preferredCharacterSize * num2;
			if (num3 < minimumCharacterSize)
			{
				int length = Mathf.Clamp(Mathf.FloorToInt((float)text2.Length * (num3 / minimumCharacterSize)) - 1, 0, text2.Length);
				text2 = text2.Substring(0, length) + "...";
				num3 = minimumCharacterSize;
			}
			setTextDelegate(text2);
			spriteText.SetCharacterSize(num3);
		}
	}

	public static void DrawSizeGizmo(bool autosizeEnabled, float maxLineWidth, Anchor_Pos anchor, GameObject gameObject)
	{
		if (autosizeEnabled)
		{
			Vector3 vector = new Vector3(maxLineWidth / 2f, 0f, 0f);
			Vector3 vector2 = gameObject.transform.localToWorldMatrix.MultiplyVector(vector);
			Vector3 vector3 = gameObject.transform.position - vector2;
			Vector3 vector4 = gameObject.transform.position + vector2;
			Vector3 zero = Vector3.zero;
			switch (anchor)
			{
			case Anchor_Pos.Upper_Left:
			case Anchor_Pos.Middle_Left:
			case Anchor_Pos.Lower_Left:
				zero += vector;
				break;
			case Anchor_Pos.Upper_Right:
			case Anchor_Pos.Middle_Right:
			case Anchor_Pos.Lower_Right:
				zero -= vector;
				break;
			}
			Gizmos.DrawLine(vector3 + zero, vector4 + zero);
		}
	}
}
