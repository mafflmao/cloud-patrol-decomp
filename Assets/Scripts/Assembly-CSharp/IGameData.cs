using System.IO;
using XmlTool;

public interface IGameData
{
	ProfileManager.ExecutionOrder ExecutionOrder { get; }

	void UnRegister();

	void Register();

	void ResetData();

	void SaveGame(StreamWriter i_Writer);

	void LoadGame(XmlNode i_RootNode);
}
