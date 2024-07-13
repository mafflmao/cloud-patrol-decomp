using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	public SpriteText mFPSText;

	private int m_frameCounter;

	private float m_timeCounter;

	private float m_lastFramerate;

	public float m_refreshTime = 0.5f;

	private void Update()
	{
		if (m_timeCounter < m_refreshTime)
		{
			m_timeCounter += Time.deltaTime;
			m_frameCounter++;
			return;
		}
		m_lastFramerate = (float)m_frameCounter / m_timeCounter;
		m_frameCounter = 0;
		m_timeCounter = 0f;
		mFPSText.Text = Mathf.RoundToInt(m_lastFramerate).ToString();
	}
}
