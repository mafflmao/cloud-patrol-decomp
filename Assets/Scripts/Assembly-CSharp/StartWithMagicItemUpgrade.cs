using System;
using System.Collections;
using UnityEngine;

public class StartWithMagicItemUpgrade : CharacterUpgrade
{
	public PowerupData _magicItem;

	private void OnEnable()
	{
		GameManager.GameStarted += SpawnMagicItem;
	}

	private void OnDisable()
	{
		GameManager.GameStarted -= SpawnMagicItem;
	}

	private void SpawnMagicItem(object sender, EventArgs e)
	{
		GameObject gameObject = MagicItemManager.Instance.SpawnMagicItem(_magicItem);
		GameObjectUtils.SetLayerRecursive(gameObject, LayerMask.NameToLayer("LitHUD"));
		MagicItemCollectable componentInChildren = gameObject.GetComponentInChildren<MagicItemCollectable>();
		componentInChildren.StopMoving();
		componentInChildren.transform.parent.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 3f));
		componentInChildren.transform.parent.parent = ShipManager.instance.enemyProjectileTarget;
		componentInChildren.transform.parent.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		iTween.PunchScale(componentInChildren.transform.parent.gameObject, iTween.Hash("amount", new Vector3(0.25f, 0.25f, 0.25f), "time", 0.25f));
		StartCoroutine(CollectMagicItem(componentInChildren));
	}

	private IEnumerator CollectMagicItem(MagicItemCollectable magicItem)
	{
		yield return new WaitForSeconds(0.25f);
		if ((bool)magicItem)
		{
			magicItem.Collect();
		}
	}
}
