using UnityEngine;
using UnityEngine.Video;

public class MoviePlayer : MonoBehaviour
{
	public delegate void MovieEndHandler(bool i_Skipped);

	public static MoviePlayer mInstance;

	public VideoPlayer defaultVideo;

	public AVProWindowsMediaMovie m_movie;

	private bool m_VideoIsPlaying;

	private bool m_StartTransitionAtEnd = true;

	private bool m_Skippable = true;

	public bool IsVideoPlaying
	{
		get
		{
			return m_VideoIsPlaying;
		}
	}

	public static MoviePlayer Instance
	{
		get
		{
			return mInstance;
		}
	}

	public static event MovieEndHandler OnMovieEnd;

	private void Awake()
	{
		m_StartTransitionAtEnd = false;
		if (mInstance == null)
		{
			mInstance = this;
			return;
		}
		Debug.Log("More than one instance of MoviePlayer.", this);
		Object.Destroy(this);
	}

	private void Update()
	{
		if (m_movie.MovieInstance == null)
		{
			if (m_VideoIsPlaying && !m_movie._loop)
			{
				EndMovie();
			}
		}
		else if (m_Skippable && FingerGestures.InputFinger.IsDown && !m_movie._loop)
		{
			EndMovie(true);
		}
		else if (m_movie.MovieInstance.PositionSeconds >= m_movie.MovieInstance.DurationSeconds && !m_movie._loop)
		{
			EndMovie();
		}
	}

	public void PlayMovie(string i_Video, bool i_StartTransitionAtEnd = true, bool i_Skippable = true, bool i_Loop = false)
	{
		m_Skippable = i_Skippable;
		m_StartTransitionAtEnd = i_StartTransitionAtEnd;
		m_movie._folder = Application.streamingAssetsPath;
		m_movie._loop = i_Loop;
		m_movie._filename = "\\" + i_Video;
		m_movie.LoadMovie(true);
		m_VideoIsPlaying = true;
		m_movie._volume = AudioListener.volume;
	}

	public void EndMovie(bool i_Skipped = false)
	{
		if (m_VideoIsPlaying)
		{
			m_movie.UnloadMovie();
			m_VideoIsPlaying = false;
			if (MoviePlayer.OnMovieEnd != null)
			{
				MoviePlayer.OnMovieEnd(i_Skipped);
			}
			if (m_StartTransitionAtEnd)
			{
				TransitionController.Instance.StartTransitionFromFrontEnd();
			}
		}
	}
}
