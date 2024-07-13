using System.Collections;
using UnityEngine;

public class SkylanderDisplay : MonoBehaviour
{
	public GameObject m_GiantBG;

	public GameObject m_LegendaryBG;

	public Renderer m_IconSkylander;

	public SpriteText m_NameSkylander;

	private CharacterData m_CD;

	public void InitSkylander(string _name, bool _geant, bool _legendary, Texture _pic, CharacterData _cd)
	{
		m_CD = _cd;
		m_NameSkylander.Text = _name;
		m_IconSkylander.material.mainTexture = _pic;
		if (!_geant)
		{
			m_GiantBG.SetActive(false);
		}
		if (!_legendary)
		{
			m_LegendaryBG.SetActive(false);
		}
	}

	public void OnSkylanderSelect()
	{
		UIManager.instance.blockInput = true;
		ApplicationManager.Instance.m_CountdownObj.Activate(false);
		StartCoroutine("OnElementBtnClickRte");
	}

	private IEnumerator OnElementBtnClickRte()
	{
		CommonAnimations.AnimateButtonElement(base.gameObject);
		SoundEventManager.Instance.Play2D(SkylanderSelectController.Instance.sfxSkylanderSelect);
		yield return new WaitForSeconds(0.5f);
		SwrveEventsUI.SkylanderTouched(m_CD.charName);
		StartGameSettings.Instance.activeSkylander = m_CD;
		if (OperatorMenu.Instance.m_ShowIntroVideo)
		{
			MoviePlayer.Instance.PlayMovie(StartGameSettings.Instance.activeSkylander.movieIntro);
		}
		else
		{
			TransitionController.Instance.StartTransitionFromFrontEnd();
		}
		base.transform.root.gameObject.SetActive(false);
	}
}
