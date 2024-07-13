using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RoomGroup : ScriptableObject, IComparable<RoomGroup>
{
	public Difficulty difficulty;

	public List<Room> rooms;

	[NonSerialized]
	private List<Room> _roomsRemainingBeforeReset = new List<Room>();

	[NonSerialized]
	private Room _lastDrawnRoom;

	public IEnumerable<string> Scenes
	{
		get
		{
			foreach (Room room in rooms)
			{
				yield return room.sceneName;
			}
		}
	}

	public Room DrawNextRandomRoom()
	{
		RefillRoomPoolIfNecessary();
		int index = UnityEngine.Random.Range(0, _roomsRemainingBeforeReset.Count);
		Room room = _roomsRemainingBeforeReset[index];
		_roomsRemainingBeforeReset.RemoveAt(index);
		_lastDrawnRoom = room;
		return room;
	}

	public Room PeekNextSequentialRoom()
	{
		RefillRoomPoolIfNecessary();
		return _roomsRemainingBeforeReset[0];
	}

	public Room DrawNextSequentialRoom()
	{
		RefillRoomPoolIfNecessary();
		Room room = _roomsRemainingBeforeReset[0];
		_roomsRemainingBeforeReset.RemoveAt(0);
		_lastDrawnRoom = room;
		return room;
	}

	public void Reset()
	{
		_lastDrawnRoom = null;
		_roomsRemainingBeforeReset.Clear();
	}

	public int CompareTo(RoomGroup other)
	{
		return base.name.CompareTo(other.name);
	}

	private void RefillRoomPoolIfNecessary()
	{
		if (!_roomsRemainingBeforeReset.Any())
		{
			if (!rooms.Any())
			{
				string message = string.Format("Unable to get scene name from group '{0}', there are no scenes in this group.", base.name);
				throw new InvalidOperationException(message);
			}
			if (DebugSettingsUI.BuildWithMinimumRooms)
			{
				_roomsRemainingBeforeReset.Add(rooms.First());
			}
			else
			{
				_roomsRemainingBeforeReset.AddRange(rooms);
			}
			if (_roomsRemainingBeforeReset.Count() > 1 && _lastDrawnRoom != null)
			{
				_roomsRemainingBeforeReset.Remove(_lastDrawnRoom);
			}
		}
	}
}
