using UnityEngine;

[AddComponentMenu("AVPro Windows Media/Movie")]
public class AVProWindowsMediaMovie : MonoBehaviour
{
	public enum ColourFormat
	{
		RGBA32 = 0,
		YCbCr_SD = 1,
		YCbCr_HD = 2
	}

	protected AVProWindowsMedia _moviePlayer;

	public string _folder = "./";

	public string _filename = "movie.mov";

	public bool _loop;

	public ColourFormat _colourFormat = ColourFormat.YCbCr_HD;

	public bool _loadOnStart = true;

	public bool _playOnStart = true;

	public bool _editorPreview;

	public float _volume = 1f;

	public Texture OutputTexture
	{
		get
		{
			if (_moviePlayer != null)
			{
				return _moviePlayer.OutputTexture;
			}
			return null;
		}
	}

	public AVProWindowsMedia MovieInstance
	{
		get
		{
			return _moviePlayer;
		}
	}

	public void Start()
	{
		if (MoviePlayer.Instance != null)
		{
			MoviePlayer.Instance.m_movie = this;
		}
		if (_loadOnStart)
		{
			LoadMovie(_playOnStart);
		}
	}

	public void LoadMovie(bool autoPlay)
	{
		if (_moviePlayer == null)
		{
			_moviePlayer = new AVProWindowsMedia();
		}
		if (_moviePlayer.StartVideo(_folder + _filename, _loop, _colourFormat != ColourFormat.RGBA32, _colourFormat == ColourFormat.YCbCr_HD))
		{
			_moviePlayer.Volume = _volume;
			if (autoPlay)
			{
				_moviePlayer.Play();
			}
		}
		else
		{
			Debug.LogWarning("[AVProWindowsMedia] Couldn't load movie " + _filename);
			UnloadMovie();
		}
	}

	public void Update()
	{
		_volume = Mathf.Clamp01(_volume);
		if (_moviePlayer != null)
		{
			if (_volume != _moviePlayer.Volume)
			{
				_moviePlayer.Volume = _volume;
			}
			_moviePlayer.Update(false);
		}
	}

	public void Play()
	{
		if (_moviePlayer != null)
		{
			_moviePlayer.Play();
		}
	}

	public void Pause()
	{
		if (_moviePlayer != null)
		{
			_moviePlayer.Pause();
		}
	}

	public void UnloadMovie()
	{
		if (_moviePlayer != null)
		{
			_moviePlayer.Dispose();
			_moviePlayer = null;
		}
	}

	public void OnDestroy()
	{
		UnloadMovie();
	}
}
