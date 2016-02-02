using System;

public class SlotTraitColor : SlotTrait
{
	public byte r;
	public byte g;
	public byte b;
	public byte a;
	public SlotTraitColor(byte r, byte g, byte b, byte a)
	{
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}

	public override string Identifer {
		get {
			var hash = ((ulong)r << 24) + ((ulong)g << 16) + ((ulong)b << 8) + (ulong)a;
			return hash.ToString();
		}
	}
}