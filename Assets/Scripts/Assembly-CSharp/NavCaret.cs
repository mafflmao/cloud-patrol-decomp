using UnityEngine;

public class NavCaret : MonoBehaviour
{
	public AnimationCurve animCurve;

	public UIScrollList scrollList;

	public GameObject highlight;

	public Transform[] HighlightPositions;

	public float[] scrollListPositionOfElementSectionStarts;

	private float _lerpT;

	private float _speed;

	private int _previousIndex = -1;

	private Vector3 _startPos = Vector3.zero;

	private Vector3 _endPos = Vector3.zero;

	private float _yOffset;

	private int _currentIndex;

	private float _oldScrollPosition;

	private void Start()
	{
		_yOffset = base.transform.localPosition.y;
		Vector3 position = HighlightPositions[_currentIndex].position;
		highlight.transform.position = new Vector3(position.x, position.y, highlight.transform.position.z);
		_endPos = highlight.transform.position;
		_endPos.y = _yOffset;
		base.transform.localPosition = _endPos;
	}

	private void Update()
	{
		float scrollPosition = scrollList.ScrollPosition;
		if (scrollPosition != _oldScrollPosition)
		{
			_oldScrollPosition = scrollPosition;
			_currentIndex = 0;
			for (int i = 0; i < scrollListPositionOfElementSectionStarts.Length; i++)
			{
				float num = scrollListPositionOfElementSectionStarts[i];
				if (num >= scrollPosition)
				{
					break;
				}
				_currentIndex = i;
			}
		}
		Vector3 position = HighlightPositions[_currentIndex].position;
		highlight.transform.position = new Vector3(position.x, position.y, highlight.transform.position.z);
		if (_previousIndex != _currentIndex)
		{
			_speed = (float)Mathf.Abs(_previousIndex - _currentIndex) * 2f;
			_previousIndex = _currentIndex;
			_startPos = base.transform.localPosition;
			_endPos = highlight.transform.position;
			_endPos.y = _yOffset;
			_lerpT = 0f;
		}
		base.transform.localPosition = Vector3.Lerp(_startPos, _endPos, Mathfx.Sinerp(0f, 1f, _lerpT));
		_lerpT += _speed * Time.deltaTime;
		_lerpT = Mathf.Clamp01(_lerpT);
	}
}
