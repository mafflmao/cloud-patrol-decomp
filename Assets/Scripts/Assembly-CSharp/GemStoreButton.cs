using UnityEngine;

public class GemStoreButton : MonoBehaviour
{
	public SpriteText txtQuantity;

	public SpriteText txtPrice;

	public PrefabPlaceholder saleTagPrefab;

	public int quantity;

	public string price;

	public PackedSprite graphic;

	public string clip;

	private SaleTag _saleTagInstance;

	public string SaleTagText
	{
		set
		{
			if (string.IsNullOrEmpty(value) || value == "0")
			{
				if (_saleTagInstance != null)
				{
					Object.Destroy(_saleTagInstance);
				}
				return;
			}
			if (_saleTagInstance == null)
			{
				_saleTagInstance = saleTagPrefab.InstantiatePrefab().GetComponent<SaleTag>();
			}
			_saleTagInstance.SaleText = value;
		}
	}

	private void Start()
	{
		UpdateText();
		if (clip != string.Empty && graphic.GetAnim(clip) != null)
		{
			graphic.PlayAnim(clip);
		}
	}

	public void UpdateText()
	{
		txtQuantity.Text = quantity.ToString("n0");
		txtPrice.Text = price;
	}
}
