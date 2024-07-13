using UnityEngine;

public class Flipbook : MonoBehaviour
{
	public int colCount = 4;

	public int rowCount = 4;

	public bool destroyAfterPlaying = true;

	public bool playOnStart = true;

	private int rowNumber;

	private int colNumber;

	public int totalCells = 4;

	public int fps = 10;

	private Vector2 offset;

	public bool loop;

	public bool lookAtCamera = true;

	private float startTime;

	private bool isPlaying;

	public bool randomOffset = true;

	public int index;

	private void OnEnable()
	{
		if (playOnStart)
		{
			Play();
		}
		else
		{
			base.GetComponent<Renderer>().enabled = false;
		}
	}

	public void Play()
	{
		base.GetComponent<Renderer>().enabled = true;
		if (randomOffset)
		{
			startTime = Time.time - Random.value * (float)(totalCells / fps);
		}
		else
		{
			startTime = Time.time;
		}
		UpdateFrame();
		isPlaying = true;
	}

	public void Stop()
	{
		isPlaying = false;
	}

	public void UpdateFrame()
	{
		index = (int)((Time.time - startTime) * (float)fps);
		if (loop && index > 0)
		{
			index %= totalCells;
		}
		Vector2 scale = new Vector2(1f / (float)colCount, 1f / (float)rowCount);
		int num = index % colCount;
		int num2 = index / colCount;
		offset = new Vector2((float)(num + colNumber) * scale.x, 1f - scale.y - (float)(num2 + rowNumber) * scale.y);
		base.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
		base.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
	}

	public void Update()
	{
		if (lookAtCamera)
		{
			base.transform.LookAt(Camera.main.transform);
		}
		if (!isPlaying)
		{
			return;
		}
		UpdateFrame();
		if (index >= totalCells)
		{
			if (destroyAfterPlaying)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			base.GetComponent<Renderer>().enabled = false;
			isPlaying = false;
		}
	}
}
