using System;

public abstract class RuleMatchExtension : Rule
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
	public static string StaticSerialize(RuleMatchExtension r)
	{
		return JsonHelper.Serialize(new Tuple<string, string>(r.SerializeUID, r.Serialize()));
	}
	public static RuleMatchExtension StaticDeserialize(string str)
	{
		throw new NotSupportedException();
	}
}
