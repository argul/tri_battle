using System;

public abstract class SlotTrait : ISerializable
{
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
}