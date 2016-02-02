using System;

public class RuleOperation2D_Rectangular : RuleOperation
{
	public int xRelative;
	public int yRelative;
}

public class RuleOperationHorizontal : RuleOperation2D_Rectangular
{
	public RuleOperationHorizontal()
	{
		yRelative = 0;
		xRelative = 1;
	}
}

public class RuleOperationVertical : RuleOperation2D_Rectangular
{
	public RuleOperationVertical()
	{
		yRelative = 1;
		xRelative = 0;
	}
}