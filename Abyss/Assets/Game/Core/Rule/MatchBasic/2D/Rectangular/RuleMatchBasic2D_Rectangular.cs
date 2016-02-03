using System;
using System.Collections.Generic;

public class RuleMatchBasic2D_Rectangular : RuleMatchBasic
{
	public int maskWidth;
	public int maskHeight;
	public bool[,] mask;
	public Pos2D[] scatters;
	public bool compiled = false;
	public bool PeekMask(int xInMask, int yInMask) { return mask[yInMask, xInMask]; }
	
	public void Compile()
	{
		int minX = maskWidth, minY = maskHeight, maxX = 0, maxY = 0;
		for (int y = 0; y < maskWidth; y++)
		{
			for (int x = 0; x < maskHeight; x++)
			{
				if (mask[y, x])
				{
					minX = Math.Min(minX, x);
					minY = Math.Min(minY, y);
					maxX = Math.Max(maxX, x);
					maxY = Math.Max(maxY, y);
				}
			}
		}

		maskWidth = maxX - minX + 1;
		maskHeight = maxY - minY + 1;

		var tmpMask = new bool[maskHeight,maskWidth];
		var tmpScatter = new List<Pos2D>();
		for (int y = minY; y <= maxY; y++)
		{
			for (int x = minX; x <= maxX; x++)
			{
				if (mask[y, x])
				{
					tmpMask[y - minY, x - minX] = true;
					tmpScatter.Add(new Pos2D(x - minX, y - minY));
				}
			}
		}

		mask = tmpMask;
		scatters = tmpScatter.ToArray();
		compiled = true;
	}

	public override string SerializeUID { get { return RECTANGULAR_2D; } }
	public override string Serialize ()
	{
		var tuple = new Tuple<int, int, bool[,]>(maskWidth, maskHeight, mask);
		return JsonHelper.Serialize(tuple);
	}

	public override void Deserialize (string str)
	{
		var tuple = JsonHelper.Deserialize<Tuple<int, int, bool[,]>>(str);
		maskWidth = tuple.item1;
		maskHeight = tuple.item2;
		mask = tuple.item3;
		Compile();
	}
}

public class RuleMatchBasicHorizontal : RuleMatchBasic2D_Rectangular
{
	public RuleMatchBasicHorizontal()
	{
		this.maskWidth = 5;
		this.maskHeight = 5;
		mask = new bool[5,5];
		mask[2,1] = true;
		mask[2,2] = true;
		mask[2,3] = true;
	}
}

public class RuleMatchBasicVertical : RuleMatchBasic2D_Rectangular
{
	public RuleMatchBasicVertical()
	{
		this.maskWidth = 5;
		this.maskHeight = 5;
		mask = new bool[5,5];
		mask[1,2] = true;
		mask[2,2] = true;
		mask[3,2] = true;
	}
}