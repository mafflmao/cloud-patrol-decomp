using UnityEngine;

public class Elements
{
	public enum Type
	{
		Air = 0,
		Life = 1,
		Undead = 2,
		Earth = 3,
		Fire = 4,
		Water = 5,
		Magic = 6,
		Tech = 7,
		Count = 8
	}

	public static string[] Names = new string[8] { "Air", "Life", "Undead", "Earth", "Fire", "Water", "Magic", "Tech" };

	public static string GetLocalizedName(Type type)
	{
		switch (type)
		{
		case Type.Air:
			return LocalizationManager.Instance.GetString("ELEMENT_AIR");
		case Type.Life:
			return LocalizationManager.Instance.GetString("ELEMENT_LIFE");
		case Type.Undead:
			return LocalizationManager.Instance.GetString("ELEMENT_UNDEAD");
		case Type.Earth:
			return LocalizationManager.Instance.GetString("ELEMENT_EARTH");
		case Type.Fire:
			return LocalizationManager.Instance.GetString("ELEMENT_FIRE");
		case Type.Water:
			return LocalizationManager.Instance.GetString("ELEMENT_WATER");
		case Type.Magic:
			return LocalizationManager.Instance.GetString("ELEMENT_MAGIC");
		case Type.Tech:
			return LocalizationManager.Instance.GetString("ELEMENT_TECH");
		default:
			return null;
		}
	}

	public static Type GetRandomElement()
	{
		return (Type)Random.Range(0, 8);
	}
}
