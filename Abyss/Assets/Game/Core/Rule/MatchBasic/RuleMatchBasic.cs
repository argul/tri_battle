using System;

public abstract class RuleMatchBasic : Rule
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

	protected const string RECTANGULAR_2D = "Rectangular_2D";
	public static string StaticSerialize(RuleMatchBasic r)
	{
		return JsonHelper.Serialize(new Tuple<string, string>(r.SerializeUID, r.Serialize()));
	}

	public static RuleMatchBasic StaticDeserialize(string str)
	{
		var tuple = JsonHelper.Deserialize<Tuple<string, string>>(str);
		switch (tuple.item1)
		{
		case RECTANGULAR_2D:
			var ret = new RuleMatchBasic2D_Rectangular();
			ret.Deserialize(tuple.item2);
			return ret;
		default:
			throw new NotSupportedException();
		}
	}
}
