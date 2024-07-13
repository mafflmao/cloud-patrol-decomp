using UnityEngine;

public class ResultFireWorks : MonoBehaviour
{
	public EventAnimation m_EventAnimation;

	public float m_maxRandomValue;

	private int m_lastValue;

	private void PlayRandomFireWorks()
	{
		float f = Random.Range(0f, m_maxRandomValue);
		int num = Mathf.RoundToInt(f);
		num *= 10;
		if (num == m_lastValue)
		{
			num = (m_lastValue = num + 10);
		}
		else
		{
			m_lastValue = num;
		}
		m_EventAnimation.PlayBySubID(num);
	}
}
