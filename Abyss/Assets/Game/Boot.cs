using UnityEngine;
using System.Collections;

public class Boot : MonoBehaviour 
{	
	void Start () 
	{
		Game.Launch();
		Application.LoadLevel("scn_main");
	}
}
