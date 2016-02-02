using System;

public class SlotWrapper2D
{
	public SlotAttribute slotAttribute;
	public Pos2D pos = new Pos2D();
	public int Trait { get { return pos.GetHashCode(); } }
	public bool IsTarget { get { return slotAttribute.category == SlotAttribute.Category.TARGET; } }
}