using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayModeUI : MonoBehaviour 
{
	public PlayMode2DR playMode;
	public void OnBackClicked()
	{
		Application.LoadLevel("scn_main");
	}

	public void OnAutoPlayToggled(bool toggle)
	{
		playMode.IsAutoPlay = toggle;
	}
}
