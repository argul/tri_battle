using System;

public abstract class RuleScore : Rule
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
	public static string StaticSerialize(RuleScore r)
	{
		return JsonHelper.Serialize(new Tuple<string, string>(r.SerializeUID, r.Serialize()));
	}
	public static RuleScore StaticDeserialize(string str)
	{
		throw new NotSupportedException();
	}
}