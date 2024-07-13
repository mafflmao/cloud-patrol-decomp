using UnityEngine;

public class MakeDestructibleWhenUpgradeActive : MonoBehaviour
{
	private void OnEnable()
	{
		SpawnerChangeUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<SpawnerChangeUpgrade>();
		if (!(passiveUpgradeOrDefault != null))
		{
			return;
		}
		SpawnerChangeData[] replacements = passiveUpgradeOrDefault.replacements;
		foreach (SpawnerChangeData spawnerChangeData in replacements)
		{
			if (base.gameObject.name == spawnerChangeData.originalObject.gameObject.name)
			{
				Transform transform = (Transform)Object.Instantiate(spawnerChangeData.replacement, base.transform.position, base.transform.rotation);
				transform.parent = base.transform.parent;
				transform.localScale = base.transform.localScale;
				if (transform.name.ToLower().StartsWith("rock_"))
				{
					transform.gameObject.AddComponent<BoxCollider>();
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
