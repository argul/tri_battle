using System;

public abstract class RuleOperation : Rule
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
	public static string StaticSerialize(RuleOperation r)
	{
		return JsonHelper.Serialize(new Tuple<string, string>(r.SerializeUID, r.Serialize()));
	}
	public static RuleOperation StaticDeserialize(string str)
	{
		throw new NotSupportedException();
	}
}
