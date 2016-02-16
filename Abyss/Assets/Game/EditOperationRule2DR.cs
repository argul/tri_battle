using UnityEngine;
using System.Collections;

public class EditOperationRule2DR: EditContainer2DR
{
	public MeshRenderer plane;
	public float planeWidth = 5f;
	public float planeHeight = 5f;
	public int maskWidth = 5;
	public int maskHeight = 5;

	public Pos2D p1;
	public Pos2D p2;
	public void LoadData(RuleOperation2D_Rectangular rule)
	{
		plane.gameObject.SetActive(true);
		Draw(planeWidth, planeHeight, maskWidth, maskHeight);
		if (rule.xRelative != 0 || rule.yRelative != 0)
		{
			var offset = new Pos2D(rule.xRelative / 2, rule.yRelative / 2);
			p1 = new Pos2D(2 - offset.x, 2 - offset.y);
			p2 = new Pos2D(2 + rule.xRelative - offset.x, 2 + rule.yRelative - offset.y);
			Mark(p1.x, p1.y);
			Mark(p2.x, p2.y);
		}
	}
	
	public void LoadDefault()
	{
		plane.gameObject.SetActive(true);
		Draw(planeWidth, planeHeight, maskWidth, maskHeight);
		p1 = null;
		p2 = null;
	}
	
	public void LoadEmpty()
	{
		plane.gameObject.SetActive(false);
		cellRoot.DestroyAllChildren();
		lineRoot.DestroyAllChildren();
		p1 = null;
		p2 = null;
	}

	public bool IsLegal { get { return null != p1 && null != p2; } }
	
	public RuleOperation2D_Rectangular SerializeData()
	{
		Assert.AssertIsTrue(IsLegal);
		var ret = new RuleOperation2D_Rectangular();
		ret.xRelative = p1.x - p2.x;
		ret.yRelative = p1.y - p2.y;
		return ret;
	}

	protected override void OnSelection (int logicX, int logicY)
	{
		if (null == p1 && null == p2)
		{
			Mark(logicX, logicY);
			p1 = new Pos2D(logicX, logicY);
		}
		else if (null != p1 && null == p2)
		{
			if (p1.x == logicX && p1.y == logicY)
			{
				p1 = null;
				Mark(logicX, logicY);
			}
			else
			{
				p2 = new Pos2D(logicX, logicY);
				Mark(logicX, logicY);
			}
		}
		else
		{
			if (p1.x == logicX && p1.y == logicY)
			{
				p1 = p2;
				p2 = null;
				Mark(logicX, logicY);
				return;
			}
			if (p2.x == logicX && p2.y == logicY)
			{
				p2 = null;
				Mark(logicX, logicY);
				return;
			}
		}
	}
}
