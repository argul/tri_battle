using UnityEngine;
using System.Collections;

public class EditMatchRule2DR : EditContainer2DR 
{
	public MeshRenderer plane;
	public float planeWidth = 5f;
	public float planeHeight = 5f;
	public int maskWidth = 5;
	public int maskHeight = 5;

	public void LoadData(RuleMatchBasic2D_Rectangular rule)
	{
		plane.gameObject.SetActive(true);
		Draw(planeWidth, planeHeight, rule.maskWidth, rule.maskHeight);
		for (int y = 0; y < rule.maskHeight; y++)
		{
			for (int x = 0; x < rule.maskWidth; x++)
			{
				if (rule.mask[y, x])
				{
					Mark(x, y);
				}
			}
		}
	}

	public void LoadDefault()
	{
		plane.gameObject.SetActive(true);
		Draw(planeWidth, planeHeight, maskWidth, maskHeight);
	}

	public void LoadEmpty()
	{
		plane.gameObject.SetActive(false);
		cellRoot.DestroyAllChildren();
		lineRoot.DestroyAllChildren();
	}

	public RuleMatchBasic2D_Rectangular SerializeData()
	{
		var ret = new RuleMatchBasic2D_Rectangular();
		ret.maskWidth = maskWidth;
		ret.maskHeight = maskHeight;
		ret.mask = selection.Clone() as bool[,];
		return ret;
	}
}