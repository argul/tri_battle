using UnityEngine;
using System.Collections;

public class EditCanvas2DR : EditContainer2DR
{
	public float canvasWidth = 10f;
	public float canvasHeight = 10f;

	public int Width { get { return selection.GetLength(1); } }
	public int Height { get { return selection.GetLength(0); } }

	public CanvasConfig2DR SerializeData()
	{
		var ret = new CanvasConfig2DR();
		ret.mapWidth = selection.GetLength(1);
		ret.mapHeight = selection.GetLength(0);
		ret.insulators = new System.Collections.Generic.List<Pos2D>();
		for (int y = 0; y < ret.mapHeight; y++)
		{
			for (int x = 0; x < ret.mapWidth; x++)
			{
				if (selection[y, x])
				{
					ret.insulators.Add(new Pos2D(x, y));
				}
			}
		}
		return ret;
	}

	public void LoadData(CanvasConfig2DR cc)
	{
		Reset(cc.mapWidth, cc.mapHeight);
		foreach (var b in cc.insulators)
		{
			Mark(b.x, b.y);
		}
	}

	public void Reset(int logicWidth, int logicHeight)
	{
		Draw(canvasWidth, canvasHeight, logicWidth, logicHeight);
	}
}
