using System;
using System.Collections.Generic;
using UnityEngine;

public class TicketSpliter : MonoBehaviour
{
	public List<SpriteText> m_Ticket;

	public SpriteText m_Level;

	public Animation m_MilestoneAnim;

	public GameObject m_LevelParent;

	public MeshRenderer[] m_Renderers;

	private void OnEnable()
	{
		PowerupCutscene.CutsceneStarted += HandlePowerupCutsceneStarted;
		PowerupCutscene.Completed += HandlePowerupCutsceneEnded;
	}

	private void OnDisable()
	{
		PowerupCutscene.CutsceneStarted -= HandlePowerupCutsceneStarted;
		PowerupCutscene.Completed -= HandlePowerupCutsceneEnded;
	}

	public void SetRoomTicket(RoomTicket i_RoomTicket)
	{
		for (int i = 0; i < m_Ticket.Count; i++)
		{
			m_Ticket[i].Text = i_RoomTicket.m_TicketCount.ToString();
		}
		m_Level.Text = i_RoomTicket.m_RoomCount.ToString();
	}

	public void SetLvlPos(float i_LvlPosX)
	{
		m_LevelParent.transform.position = new Vector3(i_LvlPosX, m_LevelParent.transform.position.y, m_LevelParent.transform.position.z);
	}

	private void HandlePowerupCutsceneStarted(object sender, EventArgs e)
	{
		for (int i = 0; i < m_Renderers.Length; i++)
		{
			m_Renderers[i].enabled = false;
		}
	}

	private void HandlePowerupCutsceneEnded(object sender, EventArgs e)
	{
		for (int i = 0; i < m_Renderers.Length; i++)
		{
			m_Renderers[i].enabled = true;
		}
	}
}
