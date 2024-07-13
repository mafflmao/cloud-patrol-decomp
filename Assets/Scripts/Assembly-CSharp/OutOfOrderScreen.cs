using System.Collections;
using UnityEngine;

public class OutOfOrderScreen : MonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		base.gameObject.SetActive(false);
	}

	public void ActivateScreen()
	{
		StartCoroutine("CheckRefillTicket");
	}

	private IEnumerator CheckRefillTicket()
	{
		ushort ticketCounter = 0;
		bool empty = true;
		while (true)
		{
			KaboomMgr.Instance.GetStateTicketFeeder(ref ticketCounter, ref empty);
			if (!empty)
			{
				CloseScreen();
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void CloseScreen()
	{
		KaboomMgr.Instance.RestartGiveTicket();
		StopCoroutine("CheckRefillTicket");
		base.gameObject.SetActive(false);
	}
}
