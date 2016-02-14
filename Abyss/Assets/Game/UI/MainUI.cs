using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainUI : MonoBehaviour
{
	public Dropdown dropdown;
	public void OnPlayModeClick()
	{
		Application.LoadLevel("scn_play_2d");
	}

	public void OnEditModeClick()
	{
		if (Game.IsClassicScheme) return;
		Game.PrepareEditingScheme(false);
		Application.LoadLevel("scn_edit_2d");
	}

	public void OnSchemeSelectorChanged(int idx)
	{
		Game.Selection = idx;
	}

	public void OnNewSchemeClick()
	{
		Game.PrepareEditingScheme(true);
		Application.LoadLevel("scn_edit_2d");
	}

	public void OnDeleteSchemeClick()
	{
		if (Game.IsClassicScheme) return;
		Game.DeleteSelectedScheme();
		FlushDropList();
	}

	void Start()
	{
		FlushDropList();
	}

	void FlushDropList()
	{
		var list = new List<Dropdown.OptionData>();
		foreach (var d in Game.Dumps)
		{
			var o = new Dropdown.OptionData();
			o.text = (null == d) ? "Classic" : d.name;
			list.Add(o);
		}
		dropdown.options = list;
	}
}