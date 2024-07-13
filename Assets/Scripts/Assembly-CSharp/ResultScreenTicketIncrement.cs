using UnityEngine;

public class ResultScreenTicketIncrement : MonoBehaviour
{
	public SpriteText ticketCount;

	public float m_IncrementDuration;

	private float m_CurTicketCount;

	private float m_TicketIncrementSpeed;

	private void Start()
	{
		m_CurTicketCount = 0f;
		m_TicketIncrementSpeed = 0f;
	}

	private void Update()
	{
		if (!(m_TicketIncrementSpeed <= 0f))
		{
			m_CurTicketCount += m_TicketIncrementSpeed * Time.deltaTime;
			ticketCount.Text = Mathf.Min(Mathf.FloorToInt(m_CurTicketCount), TicketBar.Instance.TicketEarned).ToString();
		}
	}

	public void StartTicketIncrement()
	{
		m_TicketIncrementSpeed = (float)TicketBar.Instance.TicketEarned / m_IncrementDuration;
		ticketCount.Text = "0";
	}
}
