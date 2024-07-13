using UnityEngine;

public class Scale3Grid : MonoBehaviour
{
	public const string LeftComponentName = "Left";

	public const string RightComponentName = "Right";

	public const string MiddleComponentName = "Middle";

	public const float PulseTime = 0.5f;

	public Vector2 size;

	public Vector2 uvUpperLeft;

	public Vector2 uvSize;

	public Color color = new Color(1f, 1f, 1f);

	public Material material;

	public SimpleSprite left;

	public SimpleSprite middle;

	public SimpleSprite right;

	public GameObject[] extraElements;

	public int uvEndcapWidth;

	private float? lastPulseTime;

	protected virtual void Start()
	{
		SetupReferences();
	}

	protected virtual bool IsStateOkForRebuild()
	{
		if (uvSize.x - (float)(uvEndcapWidth * 2) <= 0f)
		{
			Debug.LogError("Could not rebuild the button. The UV size wasn't big enough to hold both endcaps!");
			return false;
		}
		if (uvEndcapWidth <= 0)
		{
			Debug.LogError("Could not rebuild button. You must have a positive (non-zero) endcap size. Otherwise, use a normal button.");
			return false;
		}
		if (uvSize.x < 0f || uvSize.y < 0f)
		{
			Debug.LogError("Could not rebuild button. You must specify a positive (non-zero) uv width and height.");
			return false;
		}
		return true;
	}

	public void Rebuild()
	{
		SetupReferences();
		if (IsStateOkForRebuild())
		{
			RebuildInternal();
		}
	}

	protected virtual void RebuildInternal()
	{
		left.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT;
		right.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		middle.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER;
		left.transform.localScale = Vector3.one;
		right.transform.localScale = Vector3.one;
		middle.transform.localScale = Vector3.one;
		Vector2 lowerLeftPixel = new Vector2(uvUpperLeft.x, uvUpperLeft.y + uvSize.y);
		left.SetLowerLeftPixel(lowerLeftPixel);
		left.SetPixelDimensions(new Vector2(uvEndcapWidth, uvSize.y));
		middle.SetLowerLeftPixel(left.lowerLeftPixel + new Vector2(left.pixelDimensions.x, 0f));
		middle.SetPixelDimensions(new Vector2(uvSize.x - 2f * (float)uvEndcapWidth, uvSize.y));
		right.SetLowerLeftPixel(middle.lowerLeftPixel + new Vector2(middle.pixelDimensions.x, 0f));
		right.SetPixelDimensions(new Vector2(uvEndcapWidth, uvSize.y));
		float num = (float)uvEndcapWidth / uvSize.y;
		float num2 = size.y * num;
		left.width = num2;
		left.height = size.y;
		middle.width = size.x - num2 * 2f;
		middle.height = size.y;
		right.width = num2;
		right.height = size.y;
		left.transform.localPosition = new Vector3(0f - size.x / 2f + num2, 0f, 0f);
		right.transform.localPosition = new Vector3(size.x / 2f - num2, 0f, 0f);
		middle.transform.localPosition = Vector3.zero;
		SetColor(color);
	}

	public SimpleSprite FindSprite(string name)
	{
		Transform transform = base.transform.Find(name);
		if (transform == null)
		{
			Debug.LogWarning("Could not locate the game object '" + name + "', Please ensure it is a child of the composite button");
			return null;
		}
		GameObject gameObject = transform.gameObject;
		SimpleSprite component = gameObject.GetComponent<SimpleSprite>();
		if (component == null)
		{
			Debug.LogWarning("Could not locate the component 'SimpleSprite' in game object '" + name + "'.");
			return null;
		}
		return component;
	}

	protected virtual void SetupReferences()
	{
		if (left == null)
		{
			left = FindSprite("Left");
		}
		if (middle == null)
		{
			middle = FindSprite("Middle");
		}
		if (right == null)
		{
			right = FindSprite("Right");
		}
	}

	public virtual void SetColor(Color color)
	{
		left.SetColor(color);
		right.SetColor(color);
		middle.SetColor(color);
		this.color = color;
	}

	public virtual void Hide(bool isHidden)
	{
		left.Hide(isHidden);
		right.Hide(isHidden);
		middle.Hide(isHidden);
		GameObject[] array = extraElements;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(!isHidden);
		}
	}

	public void Pulse()
	{
		if (!lastPulseTime.HasValue || Time.realtimeSinceStartup > lastPulseTime.Value + 0.5f)
		{
			SimpleSprite[] array = new SimpleSprite[3] { left, middle, right };
			foreach (SimpleSprite simpleSprite in array)
			{
				iTween.ColorFrom(simpleSprite.gameObject, iTween.Hash("color", Color.white, "time", 0.25f, "easetype", iTween.EaseType.easeInBounce, "ignoretimescale", true));
			}
			GameObject[] array2 = extraElements;
			foreach (GameObject target in array2)
			{
				iTween.ColorFrom(target, iTween.Hash("color", Color.white, "time", 0.25f, "easetype", iTween.EaseType.easeInBounce, "ignoretimescale", true));
			}
			iTween.PunchScale(base.gameObject, iTween.Hash("amount", new Vector3(0.15f, 0.15f, 0f), "time", 0.5f, "ignoretimescale", true));
			lastPulseTime = Time.realtimeSinceStartup;
		}
	}
}
