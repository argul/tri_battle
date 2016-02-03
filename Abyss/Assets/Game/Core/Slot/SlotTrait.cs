using System;

public abstract class SlotTrait : ISerializable
{
	protected abstract string SerializeUID { get; }
	public abstract string Identifer { get; }
	public bool AbsoluteEqual(SlotTrait other)
	{
		Assert.AssertNotNull(other);
		return other.Identifer == Identifer;
	}
	public static bool AbsoluteEqual(SlotTrait a, SlotTrait b)
	{
		return a.Identifer == b.Identifer;
	}

	public abstract string Serialize();
	public abstract void Deserialize(string str);

	protected const string SLOT_TRAIT_COLOR = "SlotTraitColor";

	public static string StaticSerialize(SlotTrait t)
	{
		return JsonHelper.Serialize(new Tuple<string, string>(t.SerializeUID, t.Serialize()));
	}

	public static SlotTrait StaticDeserialize(string str)
	{
		var tuple = JsonHelper.Deserialize<Tuple<string, string>>(str);
		switch (tuple.item1)
		{
		case SLOT_TRAIT_COLOR:
			var ret = new SlotTraitColor();
			ret.Deserialize(tuple.item2);
			return ret;
		default:
			throw new NotSupportedException();
		}
	}
}