using UnityEngine;

public class TextPaster : MonoBehaviour
{
	public SpriteText m_TextPrimary;

	private SpriteText m_TextFX;

	private void Awake()
	{
		m_TextFX = base.gameObject.GetComponent<SpriteText>();
		m_TextFX.Text = m_TextPrimary.Text;
	}
}
