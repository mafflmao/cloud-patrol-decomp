using System.Collections;
using System.Collections.Generic;
using KaboomTestDLLCSharp;
using UnityEngine;

public class KaboomMgr : MonoBehaviour
{
	public enum WorkingMode
	{
		Callbacks = 0,
		Polling = 1,
		InitOnly = 2
	}

	public struct CoinCountEvent
	{
		public ushort Count1;

		public ushort Count2;

		public CoinCountEvent(ushort s1, ushort s2)
		{
			Count1 = s1;
			Count2 = s2;
		}
	}

	public struct KeyPadEvent
	{
		public ushort KeyPressed;

		public KeyPadEvent(ushort k)
		{
			KeyPressed = k;
		}
	}

	public WorkingMode m_WorkingMode = WorkingMode.Polling;

	public float m_PollingInterval = 0.5f;

	public bool m_KaboomEnabled = true;

	private static int m_SerialPortNb = -1;

	public int m_ticketOwned;

	public bool m_isGivingTicket;

	private int m_ReInitTry;

	private int m_haveReportedTicketFeederEmptySinceCount;

	public GameObject m_OutOfOrderPrefab;

	private OutOfOrderScreen _OutOfOrderScreenInstance;

	private static Queue<KeyPadEvent> mKeyPadEventQueue = new Queue<KeyPadEvent>();

	private static Queue<CoinCountEvent> mCoinEventsQueue = new Queue<CoinCountEvent>();

	private ushort[] vPreviousCoins = new ushort[2];

	private static KaboomMgr m_Instance = null;

	public static KaboomMgr Instance
	{
		get
		{
			return m_Instance;
		}
	}

	private void Start()
	{
		m_SerialPortNb = -1;
		m_ReInitTry = 0;
		if (m_Instance == null)
		{
			m_Instance = this;
			if (m_KaboomEnabled)
			{
				_Initialize(true);
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(base.gameObject);
		_OutOfOrderScreenInstance = ((GameObject)Object.Instantiate(m_OutOfOrderPrefab)).GetComponent<OutOfOrderScreen>();
	}

	private void ShowOutOfOrderScreen()
	{
		_OutOfOrderScreenInstance.gameObject.SetActive(true);
		_OutOfOrderScreenInstance.ActivateScreen();
	}

	private void _Initialize(bool canStartCoroutine)
	{
		KaboomCommWrapper.KABOOM_DLL_ERROR kABOOM_DLL_ERROR = KaboomCommWrapper.KABOOM_DLL_ERROR.NO_ERROR;
		Debug.Log("Initialize");
		if (m_SerialPortNb == -1)
		{
			for (int i = 0; i < 10; i++)
			{
				Debug.Log(i.ToString());
				DebugScreen.Log(i.ToString());
				if (KaboomCommWrapper.InitLib(i) != 0)
				{
					KaboomCommWrapper.CloseLib();
					continue;
				}
				DebugScreen.Log("Port Open");
				ushort[] array = new ushort[2];
				if (KaboomCommWrapper.GetCurrentCoinCount(ref array[0], ref array[1]) != 0)
				{
					DebugScreen.Log("Not Kaboom Port");
					KaboomCommWrapper.CloseLib();
					continue;
				}
				m_SerialPortNb = i;
				Debug.Log("Kaboom Connected To Serial : " + m_SerialPortNb);
				DebugScreen.Log("Kaboom Connected To Serial : " + m_SerialPortNb);
				break;
			}
		}
		if (canStartCoroutine && m_SerialPortNb != -1)
		{
			StartCoroutine("PollingCoroutine");
		}
	}

	private bool ReInitLib()
	{
		_Finalize();
		_Initialize(false);
		if (m_SerialPortNb == -1)
		{
			m_ReInitTry++;
			if (m_ReInitTry >= 5)
			{
				return false;
			}
			return true;
		}
		m_ReInitTry = 0;
		return true;
	}

	private void _Finalize()
	{
		if (m_SerialPortNb != -1)
		{
			m_SerialPortNb = -1;
			KaboomCommWrapper.CloseLib();
		}
	}

	private void OnApplicationQuit()
	{
		KaboomCommWrapper.ResetTicketFeeder();
		KaboomCommWrapper.CloseLib();
	}

	private IEnumerator PollingCoroutine()
	{
		float vWaitTime = m_PollingInterval / 2f;
		KaboomCommWrapper.KABOOM_DLL_ERROR vKbErr = KaboomCommWrapper.KABOOM_DLL_ERROR.NO_ERROR;
		ushort[] vCoins = new ushort[2];
		KaboomCommWrapper.KaboomKeypad vKeys = default(KaboomCommWrapper.KaboomKeypad);
		KaboomCommWrapper.KaboomKeypad vPreviousKeys = default(KaboomCommWrapper.KaboomKeypad);
		vKeys.un8UpButtonPressCount = 0;
		vKeys.un8DownButtonPressCount = 0;
		vKeys.un8MenuButtonPressCount = 0;
		vKeys.un8SelectButtonPressCount = 0;
		bool initPreviousKey = true;
		vPreviousKeys.un8UpButtonPressCount = 0;
		vPreviousKeys.un8DownButtonPressCount = 0;
		vPreviousKeys.un8MenuButtonPressCount = 0;
		vPreviousKeys.un8SelectButtonPressCount = 0;
		while (true)
		{
			if (KaboomCommWrapper.GetCurrentCoinCount(ref vCoins[0], ref vCoins[1]) == KaboomCommWrapper.KABOOM_DLL_ERROR.NO_ERROR)
			{
				if (vCoins[0] > vPreviousCoins[0] || vCoins[1] > vPreviousCoins[1])
				{
					OnCoinCountEvent(vCoins[0], vCoins[1]);
				}
				for (int i = 0; i < 2; i++)
				{
					vPreviousCoins[i] = vCoins[i];
				}
			}
			else
			{
				ReInitLib();
			}
			yield return new WaitForSeconds(vWaitTime);
			if (KaboomCommWrapper.GetKeypadCount(ref vKeys) == KaboomCommWrapper.KABOOM_DLL_ERROR.NO_ERROR)
			{
				if (initPreviousKey)
				{
					initPreviousKey = false;
					vPreviousKeys.un8UpButtonPressCount = vKeys.un8UpButtonPressCount;
					vPreviousKeys.un8DownButtonPressCount = vKeys.un8DownButtonPressCount;
					vPreviousKeys.un8MenuButtonPressCount = vKeys.un8MenuButtonPressCount;
					vPreviousKeys.un8SelectButtonPressCount = vKeys.un8SelectButtonPressCount;
				}
				else
				{
					if (vKeys.un8UpButtonPressCount != vPreviousKeys.un8UpButtonPressCount)
					{
						OnKeyboardEvent(48);
					}
					if (vKeys.un8DownButtonPressCount != vPreviousKeys.un8DownButtonPressCount)
					{
						OnKeyboardEvent(48);
					}
					if (vKeys.un8MenuButtonPressCount != vPreviousKeys.un8MenuButtonPressCount)
					{
						OnKeyboardEvent(48);
					}
					if (vKeys.un8SelectButtonPressCount != vPreviousKeys.un8SelectButtonPressCount)
					{
						OnKeyboardEvent(48);
					}
					vPreviousKeys.un8UpButtonPressCount = vKeys.un8UpButtonPressCount;
					vPreviousKeys.un8DownButtonPressCount = vKeys.un8DownButtonPressCount;
					vPreviousKeys.un8MenuButtonPressCount = vKeys.un8MenuButtonPressCount;
					vPreviousKeys.un8SelectButtonPressCount = vKeys.un8SelectButtonPressCount;
				}
			}
			else
			{
				ReInitLib();
			}
			yield return new WaitForSeconds(vWaitTime);
		}
	}

	private static void OnKeyboardEvent(ushort un16KeyPressed)
	{
		mKeyPadEventQueue.Enqueue(new KeyPadEvent(un16KeyPressed));
	}

	public bool GetKeyboardEvent(out KeyPadEvent aEvt)
	{
		if (m_SerialPortNb == -1)
		{
			aEvt = default(KeyPadEvent);
			return false;
		}
		if (mKeyPadEventQueue.Count > 0)
		{
			aEvt = mKeyPadEventQueue.Dequeue();
			return true;
		}
		aEvt = default(KeyPadEvent);
		return false;
	}

	private static void OnCoinCountEvent(ushort un16CoinCount1, ushort un16CoinCount2)
	{
		mCoinEventsQueue.Enqueue(new CoinCountEvent(un16CoinCount1, un16CoinCount2));
	}

	private void OnDestroy()
	{
		if (m_SerialPortNb != -1)
		{
			_Finalize();
		}
	}

	public bool GetCoinEvent(out CoinCountEvent aEvt)
	{
		if (m_SerialPortNb == -1)
		{
			aEvt = default(CoinCountEvent);
			return false;
		}
		if (mCoinEventsQueue.Count > 0)
		{
			aEvt = mCoinEventsQueue.Dequeue();
			return true;
		}
		aEvt = default(CoinCountEvent);
		return false;
	}

	public KaboomCommWrapper.KABOOM_DLL_ERROR ResetCoinCount(ref ushort aUn16CoinCount1, ref ushort aUn16CoinCount2)
	{
		if (m_SerialPortNb == -1)
		{
			return KaboomCommWrapper.KABOOM_DLL_ERROR.SERIAL_COMMUNICATION_NOT_INITIALIZED;
		}
		return KaboomCommWrapper.ResetCoinCount(ref aUn16CoinCount1, ref aUn16CoinCount2);
	}

	public void SetTicketToFeed(ushort i_ticket)
	{
		if (m_SerialPortNb == -1)
		{
			DebugScreen.Log("SERIAL PORT = -1");
			return;
		}
		ushort pun16TicketToFeedCount = 0;
		KaboomCommWrapper.GetTicketFeederTicketToFeedCount(ref pun16TicketToFeedCount);
		pun16TicketToFeedCount += i_ticket;
		if (KaboomCommWrapper.SetTicketFeederTicketToFeedCount(pun16TicketToFeedCount) != 0)
		{
			if (ReInitLib())
			{
				SetTicketToFeed(i_ticket);
			}
		}
		else if (!m_isGivingTicket)
		{
			StartCoroutine("GiveTicket");
			m_isGivingTicket = true;
		}
	}

	public void RestartGiveTicket()
	{
		m_isGivingTicket = true;
		StartCoroutine("GiveTicket");
	}

	private IEnumerator GiveTicket()
	{
		ushort ticketCounter = 0;
		ushort ticketPointer = 0;
		bool empty = true;
		while (true)
		{
			if (m_SerialPortNb == -1)
			{
				empty = true;
			}
			else if (KaboomCommWrapper.GetTicketFeederTicketToFeedCount(ref ticketPointer) == KaboomCommWrapper.KABOOM_DLL_ERROR.NO_ERROR)
			{
				GetStateTicketFeeder(ref ticketCounter, ref empty);
			}
			else if (ReInitLib())
			{
				empty = false;
				ticketCounter = 1;
				ticketPointer = 0;
			}
			else
			{
				ticketCounter = ticketPointer;
				empty = true;
			}
			if (ticketCounter == ticketPointer)
			{
				break;
			}
			if (empty)
			{
				if (++m_haveReportedTicketFeederEmptySinceCount == 2)
				{
					ushort lNumReset = 0;
					if (KaboomCommWrapper.ResetEmptyTicketFeeder(ref lNumReset) != 0)
					{
					}
				}
				else if (m_haveReportedTicketFeederEmptySinceCount > 2)
				{
					break;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		if (m_SerialPortNb == -1 || !empty)
		{
			DebugScreen.Log("Not Empty ResetFeeder and Stop GivingTicket");
			ResetTicketFeeder();
			m_isGivingTicket = false;
			m_haveReportedTicketFeederEmptySinceCount = 0;
		}
		else
		{
			WaitForNoMoreTicket();
		}
	}

	public void GetStateTicketFeeder(ref ushort i_ticketCounter, ref bool i_empty)
	{
		ushort pun16IsEmpty = 0;
		if (KaboomCommWrapper.GetTicketFeederCount(ref i_ticketCounter, ref pun16IsEmpty) != 0)
		{
			if (ReInitLib())
			{
				GetStateTicketFeeder(ref i_ticketCounter, ref i_empty);
				return;
			}
			i_ticketCounter = 0;
			i_empty = true;
		}
		else if (pun16IsEmpty == 0)
		{
			i_empty = false;
		}
		else
		{
			i_empty = true;
		}
	}

	private void WaitForNoMoreTicket()
	{
		if (m_SerialPortNb == -1)
		{
			return;
		}
		ushort i_ticketCounter = 0;
		ushort pun16TicketToFeedCount = 0;
		bool i_empty = true;
		GetStateTicketFeeder(ref i_ticketCounter, ref i_empty);
		KaboomCommWrapper.GetTicketFeederTicketToFeedCount(ref pun16TicketToFeedCount);
		if (i_empty)
		{
			if (i_ticketCounter == 0)
			{
				m_ticketOwned = pun16TicketToFeedCount - 1;
			}
			else
			{
				m_ticketOwned = pun16TicketToFeedCount - i_ticketCounter - 2;
			}
			DebugScreen.Log("OutOfOrder");
			ShowOutOfOrderScreen();
		}
	}

	public void ResetTicketFeeder()
	{
		KaboomCommWrapper.KABOOM_DLL_ERROR kABOOM_DLL_ERROR = KaboomCommWrapper.ResetTicketFeeder();
		m_isGivingTicket = false;
		if (kABOOM_DLL_ERROR != 0)
		{
			Debug.Log("Error with ticket feeder [RESET]");
		}
	}
}
