using UnityEngine;
using System.Collections;

public class MainUI : MonoBehaviour
{
	public void OnPlayModeClick()
	{
		Debug.LogWarning("OnPlayModeClick");
	}

	public void OnEditModeClick()
	{
		Debug.LogWarning("OnEditModeClick");
	}

	public void OnSchemeSelectorChanged(int idx)
	{
		Debug.LogWarning("OnSchemeSelectorChanged  " + idx);
	}

	public void OnNewSchemeClick()
	{
		Debug.LogWarning("OnNewSchemeClick");
	}

	public void OnDeleteSchemeClick()
	{
		Debug.LogWarning("OnDeleteSchemeClick");
	}
}