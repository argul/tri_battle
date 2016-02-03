using System;

public class RuleOperation2D_Rectangular : RuleOperation
{
	public int xRelative;
	public int yRelative;
	public override string SerializeUID { get { return RECTANGULAR_2D; } }
	public override string Serialize ()
	{
		return JsonHelper.Serialize(new Tuple<int, int>(xRelative, yRelative));
	}
	public override void Deserialize (string str)
	{
		var tuple = JsonHelper.Deserialize<Tuple<int, int>>(str);
		xRelative = tuple.item1;
		yRelative = tuple.item2;
	}
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