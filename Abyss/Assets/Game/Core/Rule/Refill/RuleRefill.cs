using System;
using System.Collections.Generic;

public abstract class RuleRefill : Rule
{
	public abstract string SerializeUID { get; }
	public override string Serialize ()
	{
		throw new NotImplementedException ();
	}
	public override void Deserialize (string str)
	{
		throw new NotImplementedException ();
	}
	protected const string ON_SPOT = "OnSpot";
	protected const string DOWNWARD_2DR = "Downward_2DR";
	public static string StaticSerialize(RuleRefill r)
	{
		return JsonHelper.Serialize(new Tuple<string, string>(r.SerializeUID, r.Serialize()));
	}
	public static RuleRefill StaticDeserialize(string str)
	{
		var tuple = JsonHelper.Deserialize<Tuple<string, string>>(str);
		switch (tuple.item1)
		{
		case DOWNWARD_2DR:
			var ret = new RuleRefill2DR_Downward();
			ret.Deserialize(tuple.item2);
			return ret;
		default:
			throw new NotSupportedException();
		}
	}

	public class FillInfo
	{
		public FillInfo()
		{
			ancestorPos = null;
		}
		public bool IsOnSpot { get { return null == ancestorPos; } }
		public Pos2D ancestorPos;
		public List<Pos2D> childrenPos;
	}
}
