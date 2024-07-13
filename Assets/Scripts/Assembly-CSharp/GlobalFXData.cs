using UnityEngine;

public class GlobalFXData : SingletonMonoBehaviour
{
	public const string DeathFxResourcePathFormat = "HitEffects/HitEffect_{0}";

	public GameObject DeathFxPrefab;

	public static GlobalFXData Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<GlobalFXData>();
		}
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		string arg = StartGameSettings.Instance.activeSkylander.elementData.elementType.ToString();
		DeathFxPrefab = (GameObject)Resources.Load(string.Format("HitEffects/HitEffect_{0}", arg), typeof(GameObject));
	}
}
