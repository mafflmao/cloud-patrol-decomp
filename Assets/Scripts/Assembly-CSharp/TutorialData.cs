using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialData : ScriptableObject
{
	public RoomGroup gameplayTutorials;

	public Room magicItemTutorialRoom;

	public int roomsBeforeMagicItemTutorial = 7;

	public int NumberOfTimesToShowTutorialForEachEnemyType = 1;

	public RoomGroup shooterTutorials;

	public RoomGroup lobberTutorials;

	public RoomGroup standardShieldedTutorials;

	public RoomGroup hazardShieldedTutorials;

	public RoomGroup dodgerTutorials;

	public RoomGroup bombHazardTutorials;

	public RoomGroup tripleShooterTutorials;

	public RoomGroup protectorTutorials;

	public RoomGroup GetTutorialGroupFor(EnemyTypes enemyType)
	{
		switch (enemyType)
		{
		case EnemyTypes.Shooter:
			return shooterTutorials;
		case EnemyTypes.Lobber:
			return lobberTutorials;
		case EnemyTypes.StandardShielded:
			return standardShieldedTutorials;
		case EnemyTypes.HazardShielded:
			return hazardShieldedTutorials;
		case EnemyTypes.Dodger:
			return dodgerTutorials;
		case EnemyTypes.BombHazard:
			return bombHazardTutorials;
		case EnemyTypes.TripleShooter:
			return tripleShooterTutorials;
		case EnemyTypes.Protector:
			return protectorTutorials;
		default:
			return null;
		}
	}

	public IEnumerable<string> GetAllRoomSceneNames()
	{
		List<string> list = new List<string>();
		if (magicItemTutorialRoom != null)
		{
			list.Add(magicItemTutorialRoom.sceneName);
		}
		AddRoomScenesToSceneList(list, gameplayTutorials);
		AddRoomScenesToSceneList(list, shooterTutorials);
		AddRoomScenesToSceneList(list, lobberTutorials);
		AddRoomScenesToSceneList(list, standardShieldedTutorials);
		AddRoomScenesToSceneList(list, hazardShieldedTutorials);
		AddRoomScenesToSceneList(list, dodgerTutorials);
		AddRoomScenesToSceneList(list, bombHazardTutorials);
		AddRoomScenesToSceneList(list, tripleShooterTutorials);
		AddRoomScenesToSceneList(list, protectorTutorials);
		return list;
	}

	private void AddRoomScenesToSceneList(List<string> listToAddTo, RoomGroup roomGroup)
	{
		if (roomGroup != null && roomGroup.Scenes.Any())
		{
			if (DebugSettingsUI.BuildWithMinimumRooms)
			{
				listToAddTo.Add(roomGroup.Scenes.First());
			}
			else
			{
				listToAddTo.AddRange(roomGroup.Scenes);
			}
		}
	}
}
