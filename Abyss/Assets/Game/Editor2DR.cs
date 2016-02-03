using UnityEngine;
using System.Collections;

public class Editor2DR : MonoBehaviour 
{
	public enum Stage
	{
		CHOOSE_SCHEME,
		EDIT_COLOR_TRAIT,
		EDIT_CANVAS,
		EDIT_MATCHING_RULE,
		EDIT_OPERATION_RULE,
		CHOOSE_EXTENSION_RULE,
		EDIT_SPECIAL,
	}
}
