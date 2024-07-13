using UnityEngine;

public class Scale9Grid : MonoBehaviour
{
	private SimpleSprite[] sprites;

	public Material mat;

	public Color color = Color.white;

	public Vector2 size;

	public int cornerSize = 64;

	public void FetchSprites()
	{
		sprites = GetComponentsInChildren<SimpleSprite>();
	}

	public void ApplyTextureToComponents()
	{
		SimpleSprite[] array = sprites;
		foreach (SimpleSprite simpleSprite in array)
		{
			MeshRenderer component = simpleSprite.GetComponent<MeshRenderer>();
			component.sharedMaterial = mat;
		}
	}

	public void Hide(bool isHidden)
	{
		if (sprites == null || sprites.Length == 0)
		{
			FetchSprites();
		}
		SimpleSprite[] array = sprites;
		foreach (SimpleSprite simpleSprite in array)
		{
			simpleSprite.Hide(isHidden);
		}
	}

	public void SetColor(Color newColor)
	{
		color = newColor;
		if (sprites == null || sprites.Length == 0)
		{
			FetchSprites();
		}
		if (sprites != null)
		{
			SimpleSprite[] array = sprites;
			foreach (SimpleSprite simpleSprite in array)
			{
				simpleSprite.SetColor(newColor);
			}
		}
	}

	public void Resize()
	{
		if (sprites == null || sprites.Length == 0)
		{
			FetchSprites();
		}
		float num = mat.mainTexture.width;
		float num2 = mat.mainTexture.height;
		Vector2 vector = new Vector2(num / 2f, num2 / 2f);
		Vector2 vector2 = new Vector2(size.x - (float)(2 * cornerSize), size.y - (float)(2 * cornerSize));
		for (int i = 0; i < sprites.Length; i++)
		{
			SimpleSprite simpleSprite = sprites[i];
			simpleSprite.Color = color;
			simpleSprite.width = 0f;
			simpleSprite.height = 0f;
			simpleSprite.lowerLeftPixel = Vector2.zero;
			simpleSprite.pixelDimensions = Vector2.zero;
			switch (i)
			{
			case 0:
				simpleSprite.width = cornerSize;
				simpleSprite.height = cornerSize;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT;
				simpleSprite.transform.localPosition = new Vector3((0f - vector2.x) / 2f, vector2.y / 2f, 0f);
				simpleSprite.lowerLeftPixel = new Vector2(0f, num2 / 2f);
				simpleSprite.pixelDimensions = vector;
				break;
			case 1:
				simpleSprite.width = vector2.x;
				simpleSprite.height = cornerSize;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER;
				simpleSprite.transform.localPosition = new Vector3(0f, vector2.y / 2f, 0f);
				simpleSprite.lowerLeftPixel = vector;
				simpleSprite.pixelDimensions = new Vector2(1f, num2 / 2f);
				break;
			case 2:
				simpleSprite.width = cornerSize;
				simpleSprite.height = cornerSize;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT;
				simpleSprite.transform.localPosition = new Vector3(vector2.x / 2f, vector2.y / 2f, 0f);
				simpleSprite.lowerLeftPixel = vector;
				simpleSprite.pixelDimensions = vector;
				break;
			case 3:
				simpleSprite.width = cornerSize;
				simpleSprite.height = vector2.y;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT;
				simpleSprite.transform.localPosition = new Vector3((0f - vector2.x) / 2f, 0f, 0f);
				simpleSprite.lowerLeftPixel = new Vector2(0f, num2 / 2f);
				simpleSprite.pixelDimensions = new Vector2(num / 2f, 1f);
				break;
			case 4:
				simpleSprite.width = vector2.x;
				simpleSprite.height = vector2.y;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER;
				simpleSprite.transform.localPosition = new Vector3(0f, 0f, 0f);
				simpleSprite.lowerLeftPixel = vector;
				simpleSprite.pixelDimensions = Vector2.one;
				break;
			case 5:
				simpleSprite.width = cornerSize;
				simpleSprite.height = vector2.y;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
				simpleSprite.transform.localPosition = new Vector3(vector2.x / 2f, 0f, 0f);
				simpleSprite.lowerLeftPixel = vector;
				simpleSprite.pixelDimensions = new Vector2(num / 2f, 1f);
				break;
			case 6:
				simpleSprite.width = cornerSize;
				simpleSprite.height = cornerSize;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT;
				simpleSprite.transform.localPosition = new Vector3((0f - vector2.x) / 2f, (0f - vector2.y) / 2f, 0f);
				simpleSprite.lowerLeftPixel = Vector2.zero;
				simpleSprite.pixelDimensions = vector;
				break;
			case 7:
				simpleSprite.width = vector2.x;
				simpleSprite.height = cornerSize;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.UPPER_CENTER;
				simpleSprite.transform.localPosition = new Vector3(0f, (0f - vector2.y) / 2f, 0f);
				simpleSprite.lowerLeftPixel = new Vector2(num / 2f, 0f);
				simpleSprite.pixelDimensions = new Vector2(1f, num2 / 2f);
				break;
			case 8:
				simpleSprite.width = cornerSize;
				simpleSprite.height = cornerSize;
				simpleSprite.anchor = SpriteRoot.ANCHOR_METHOD.UPPER_LEFT;
				simpleSprite.transform.localPosition = new Vector3(vector2.x / 2f, (0f - vector2.y) / 2f, 0f);
				simpleSprite.lowerLeftPixel = new Vector2(num / 2f, 0f);
				simpleSprite.pixelDimensions = vector;
				break;
			}
		}
	}

	public void BuildGrid()
	{
		BuildGrid(false);
	}

	public void BuildGrid(bool force)
	{
		FetchSprites();
		if (Application.isPlaying)
		{
			Debug.LogError("Let's only do this stuff in the editor please ...");
			return;
		}
		if (force && sprites != null)
		{
			for (int i = 0; i < sprites.Length; i++)
			{
				if (sprites[i] != null)
				{
					Object.DestroyImmediate(sprites[i]);
				}
			}
			sprites = null;
		}
		if (sprites != null)
		{
			return;
		}
		sprites = new SimpleSprite[9];
		for (int j = 0; j < sprites.Length; j++)
		{
			int num = (int)Mathf.Floor((float)j / 3f);
			int num2 = j % 3;
			if ((float)cornerSize <= 0f && (num == 0 || num == 2))
			{
				sprites[j] = null;
				continue;
			}
			if ((float)cornerSize <= 0f && (num2 == 0 || num2 == 2))
			{
				sprites[j] = null;
				continue;
			}
			GameObject gameObject = new GameObject();
			gameObject.name = string.Format("{0} Grid [{1},{2}]", base.gameObject.name, num + 1, num2 + 1);
			gameObject.transform.parent = base.gameObject.transform;
			SimpleSprite simpleSprite = (SimpleSprite)gameObject.AddComponent(typeof(SimpleSprite));
			simpleSprite.GetComponent<Renderer>().sharedMaterial = mat;
			float num3 = simpleSprite.GetComponent<Renderer>().sharedMaterial.mainTexture.width;
			float num4 = simpleSprite.GetComponent<Renderer>().sharedMaterial.mainTexture.height;
			simpleSprite.lowerLeftPixel = new Vector2((float)num2 * (num3 * 0.25f), (float)(num + 2) * (num4 * 0.25f));
			simpleSprite.pixelDimensions = new Vector2(num3 * 0.5f, num4 * 0.5f);
			sprites[j] = simpleSprite;
		}
		Resize();
	}
}
