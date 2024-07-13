using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : ScriptableObject
{
	public const string LowQualityBackgroundSuffix = "_LowPoly";

	public List<RoomGroup> rooms;

	public List<string> backgroundScenes;

	public List<RoundData> roundData;

	public Room firstRoom;

	public Room bombShipTrollRoom;

	public Texture2D skyTexture;

	[NonSerialized]
	private Dictionary<Difficulty, List<RoomGroup>> _roomPools = new Dictionary<Difficulty, List<RoomGroup>>();

	public int RoomGroupCount
	{
		get
		{
			int num = 0;
			foreach (RoundData roundDatum in roundData)
			{
				num += roundDatum.Count;
			}
			return num;
		}
	}

	public void InitializeRuntime()
	{
		foreach (Difficulty item in Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>())
		{
			List<RoomGroup> list = new List<RoomGroup>();
			FillListWithAllGroupsOfDifficulty(list, item);
			_roomPools[item] = list;
		}
	}

	private void FillListWithAllGroupsOfDifficulty(List<RoomGroup> listToPopulate, Difficulty difficultyToPopulateWith)
	{
		listToPopulate.Clear();
		foreach (RoomGroup room in rooms)
		{
			if (difficultyToPopulateWith == room.difficulty)
			{
				listToPopulate.Add(room);
			}
		}
	}

	public RoomGroup GetNextRoomGroupForDifficulty(Difficulty difficulty)
	{
		List<RoomGroup> list = _roomPools[difficulty];
		if (!list.Any())
		{
			FillListWithAllGroupsOfDifficulty(list, difficulty);
			if (!list.Any())
			{
				string message = string.Format("Unable to get any level of difficulty '{0}', do any exist?", difficulty);
				Debug.LogError(message);
				return null;
			}
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		RoomGroup result = list[index];
		list.RemoveAt(index);
		return result;
	}

	public Difficulty? GetDifficultyForRoom(string sceneName)
	{
		foreach (RoomGroup room in rooms)
		{
			foreach (Room room2 in room.rooms)
			{
				if (room2.sceneName.Equals(sceneName))
				{
					return room.difficulty;
				}
			}
		}
		return null;
	}

	public IEnumerable<string> GetAllRoomSceneNames()
	{
		foreach (Room room in GetAllRooms())
		{
			if (string.IsNullOrEmpty(room.sceneName))
			{
				Debug.LogError("Room '" + base.name + "' has NULL or EMPTY room in its list.");
			}
			yield return room.sceneName;
		}
	}

	public IEnumerable<Room> GetAllRooms()
	{
		HashSet<Difficulty> difficultiesAdded = new HashSet<Difficulty>();
		foreach (RoomGroup roomGroup in rooms)
		{
			if (DebugSettingsUI.BuildWithMinimumRooms)
			{
				if (difficultiesAdded.Add(roomGroup.difficulty))
				{
					yield return roomGroup.rooms.First();
				}
				continue;
			}
			foreach (Room room in roomGroup.rooms)
			{
				if (room == null)
				{
					Debug.LogError(roomGroup.name + " RoomGroup has a null room in it!  Please fix it or you will break the build.");
				}
				yield return room;
			}
		}
		if (firstRoom != null)
		{
			yield return firstRoom;
		}
		if (bombShipTrollRoom != null)
		{
			yield return bombShipTrollRoom;
		}
	}

	public List<string> GetAllSceneNames()
	{
		List<string> list = new List<string>();
		list.AddRange(GetAllRoomSceneNames());
		list.AddRange(backgroundScenes);
		if (backgroundScenes.Where((string sceneName) => string.IsNullOrEmpty(sceneName)).Any())
		{
			Debug.LogError("NULL or Empty scene name in background scenes for level " + base.name);
		}
		foreach (string backgroundScene in backgroundScenes)
		{
			list.Add(backgroundScene + "_LowPoly");
		}
		return list;
	}
}
