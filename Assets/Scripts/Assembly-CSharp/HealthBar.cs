using System;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	public enum HealthBarEvent
	{
		TimeDecrease = 0,
		BombHit = 1,
		ProjHit = 2,
		ScreenCleared = 3,
		ComboCompleted = 4,
		CoinCollected = 5
	}

	public string m_AnimIn;

	public string m_AnimOut;

	public float m_AnimInDuration = 3f;

	public float m_AnimOutDuration = 1f;

	public UIProgressBar m_Visual;

	public UIProgressBar m_VisualPanic;

	private MeshRenderer[] m_Render;

	public int m_MaxHealth;

	public int m_PerSec;

	public int m_BombHit;

	public int m_ProjHit;

	public int m_BaseScreenClear;

	public int m_ClearedScreenFactor;

	public int m_ComboFactor;

	public int m_CoinCollected;

	public int m_FatalBombRoom = 12;

	public string m_HealthGainAnim;

	public string m_StandardAnim;

	public string m_PanicAnim;

	private float m_CurHealth;

	private float m_RealHealthValue;

	private int m_RoomClearedCount;

	private bool m_IsDead;

	private bool m_CanDie;

	private bool m_Pause;

	private static HealthBar m_Instance;

	public static HealthBar Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public bool IsDead
	{
		get
		{
			return m_IsDead;
		}
	}

	public bool Pause
	{
		set
		{
			m_Pause = value;
		}
	}

	public float RealHealth
	{
		get
		{
			return m_RealHealthValue;
		}
		set
		{
			if (!(m_RealHealthValue <= 0f) && !m_IsDead)
			{
				m_RealHealthValue = value;
				m_RealHealthValue = Mathf.Clamp(m_RealHealthValue, 0f, m_MaxHealth);
			}
		}
	}

	private void Show()
	{
		for (int i = 0; i < m_Render.Length; i++)
		{
			if (!(m_Render[i] == null))
			{
				m_Render[i].enabled = true;
			}
		}
		base.transform.parent.GetComponent<Animation>().Play(m_AnimIn);
		InvokeHelper.InvokeSafe(SetCanDie, m_AnimInDuration, this);
	}

	private void Hide()
	{
		for (int i = 0; i < m_Render.Length; i++)
		{
			if (!(m_Render[i] == null))
			{
				m_Render[i].enabled = false;
			}
		}
		m_CanDie = false;
	}

	private void SetCanDie()
	{
		m_CanDie = true;
	}

	private void OnEnable()
	{
		Loot.Collected += HandleGameManagerMoneyCollected;
		ComboCoin.ComboCoinSpawned += HandleComboCoinComboCoinSpawned;
	}

	private void OnDisable()
	{
		ComboCoin.ComboCoinSpawned -= HandleComboCoinComboCoinSpawned;
		Loot.Collected -= HandleGameManagerMoneyCollected;
	}

	private void Awake()
	{
		m_CanDie = false;
		m_Instance = this;
		m_CurHealth = 0f;
		m_Visual.Value = 0f;
	}

	private void Start()
	{
		m_Render = GetComponentsInChildren<MeshRenderer>();
		if (m_MaxHealth == 0)
		{
			Debug.LogError("m_MaxHealth in HealthBar Object is 0. This will lead to a division by 0. HealthBar is going to be destroy to avoid crash.");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			base.GetComponent<Animation>().Play(m_StandardAnim);
			Hide();
		}
	}

	private void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			m_PerSec *= 2;
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			m_PerSec /= 2;
		}
		UpdateHealth();
		UpdateAnim();
		CheckDeath();
	}

	private void UpdateHealth()
	{
		if (m_CanDie && !m_Pause)
		{
			RealHealth -= Time.deltaTime * (float)(m_PerSec + m_RoomClearedCount);
		}
		float num = (float)m_MaxHealth / 2.5f * Time.deltaTime;
		float f = RealHealth - m_CurHealth;
		if (num > Mathf.Abs(f))
		{
			m_CurHealth = RealHealth;
		}
		else
		{
			m_CurHealth += Mathf.Sign(f) * num;
		}
		m_Visual.Value = m_CurHealth / (float)m_MaxHealth;
		m_VisualPanic.Value = m_Visual.Value;
	}

	private void UpdateAnim()
	{
		string text = m_StandardAnim;
		if (RealHealth > m_CurHealth)
		{
			text = m_HealthGainAnim;
		}
		else if (m_Visual.Value < 0.2f)
		{
			text = m_PanicAnim;
		}
		if (!base.GetComponent<Animation>().IsPlaying(text))
		{
			base.GetComponent<Animation>().Play(text);
		}
	}

	private void CheckDeath()
	{
	}

	private void OutOfHealth()
	{
		ShipManager.instance.DisableTargetting();
		m_CanDie = false;
		base.transform.parent.GetComponent<Animation>().Play(m_AnimOut);
		InvokeHelper.InvokeSafe(Die, m_AnimOutDuration, this);
	}

	public void Die()
	{
		Hide();
		m_IsDead = true;
		GameManager.Instance.ShowGameOverScreen();
	}

	public void Reset()
	{
		Show();
		m_IsDead = false;
		m_RoomClearedCount = -1;
		m_RealHealthValue = m_MaxHealth;
	}

	public void Continue()
	{
		Reset();
	}

	public void BombsAreFatal()
	{
		m_BombHit = int.MaxValue;
	}

	public void TriggerEvent(HealthBarEvent i_Event, int i_Value = 0)
	{
		int num = 0;
		switch (i_Event)
		{
		case HealthBarEvent.BombHit:
			num = -m_BombHit;
			break;
		case HealthBarEvent.ProjHit:
			num = -m_ProjHit;
			break;
		case HealthBarEvent.ScreenCleared:
			num = m_BaseScreenClear + m_ClearedScreenFactor * m_RoomClearedCount++;
			break;
		case HealthBarEvent.ComboCompleted:
			num = m_ComboFactor * i_Value;
			break;
		case HealthBarEvent.CoinCollected:
			num = m_CoinCollected;
			break;
		}
		RealHealth += num;
	}

	private void HandleComboCoinComboCoinSpawned(object sender, EventArgs e)
	{
		TriggerEvent(HealthBarEvent.ComboCompleted, ((ComboCoin)sender).number);
	}

	private void HandleGameManagerMoneyCollected(object sender, EventArgs e)
	{
		TriggerEvent(HealthBarEvent.CoinCollected);
	}
}
