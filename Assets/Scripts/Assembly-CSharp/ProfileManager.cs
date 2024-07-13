using System.Collections;
using System.IO;
using UnityEngine;
using XmlTool;

public class ProfileManager : BaseManager
{
	public enum ExecutionOrder
	{
		Progression = 0,
		Tutorial = 1,
		Achievement = 2,
		Options = 3,
		Localization = 4,
		Goal = 5,
		Upgrades = 6,
		DLC = 7,
		Bundle = 8,
		CurrentUser = 9,
		Leaderboard = 10,
		OperatorMenu = 11
	}

	private const float m_FileVersion = 0.01f;

	private const float m_CompatibleVersion = 0.01f;

	public SortedList m_GameDataList;

	public TextAsset m_DefaultSaveFile;

	public string m_SaveFileName = "ProjectName_Profile_0";

	public string m_SaveFileExtension = ".svg";

	private XmlNode XMLRootNode;

	private static ProfileManager m_Instance;

	public static ProfileManager Instance
	{
		get
		{
			return m_Instance;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else
		{
			Debug.Log("More than one instance of ProfileManager.", this);
		}
		m_GameDataList = new SortedList();
	}

	private void Start()
	{
		LoadGame();
	}

	public void SaveGame()
	{
		SaveGame(Application.persistentDataPath + "/" + m_SaveFileName + m_SaveFileExtension);
	}

	private void SaveGame(string i_FilePath)
	{
		StreamWriter streamWriter = File.CreateText(i_FilePath);
		streamWriter.WriteLine("<SavedGame version=\"" + 0.01f + "\">");
		Debug.Log("[PROFILE] Saving");
		for (int i = 0; i < m_GameDataList.Count; i++)
		{
			IGameData gameData = (IGameData)m_GameDataList.GetByIndex(i);
			gameData.SaveGame(streamWriter);
		}
		streamWriter.WriteLine("</SavedGame>");
		streamWriter.Close();
	}

	[ContextMenu("Load Game")]
	public void LoadGame()
	{
		bool flag = false;
		bool flag2 = false;
		XmlParser xmlParser;
		try
		{
			xmlParser = new XmlParser(Application.persistentDataPath + "/" + m_SaveFileName + m_SaveFileExtension);
		}
		catch
		{
			Debug.Log("[PROFILE] No save file found.", this);
			return;
		}
		XMLRootNode = null;
		do
		{
			XMLRootNode = xmlParser.GetRoot();
			if (!flag2)
			{
				float attributeAsFloat = XMLRootNode.GetAttributeAsFloat("version");
				if (attributeAsFloat < 0.01f)
				{
					Debug.Log("!!!New Save Game Version, old Save will not be loaded!!! " + attributeAsFloat + " < " + 0.01f, this);
					flag2 = true;
					return;
				}
				flag = true;
			}
			else
			{
				flag = true;
			}
		}
		while (!flag);
		for (int i = 0; i < m_GameDataList.Count; i++)
		{
			IGameData gameData = (IGameData)m_GameDataList.GetByIndex(i);
			gameData.LoadGame(XMLRootNode);
		}
	}

	public void ResetGame()
	{
		StreamWriter streamWriter = File.CreateText(Application.persistentDataPath + "/" + m_SaveFileName + m_SaveFileExtension);
		streamWriter.Write(string.Empty);
		streamWriter.Close();
		for (int i = 0; i < m_GameDataList.Count; i++)
		{
			IGameData gameData = (IGameData)m_GameDataList.GetByIndex(i);
			gameData.ResetData();
		}
	}

	public void Register(IGameData i_GameData)
	{
		m_GameDataList.Add(i_GameData.ExecutionOrder, i_GameData);
		if (XMLRootNode != null)
		{
			i_GameData.LoadGame(XMLRootNode);
		}
	}

	public void Unregister(IGameData i_GameData)
	{
		m_GameDataList.Remove(i_GameData.ExecutionOrder);
	}
}
