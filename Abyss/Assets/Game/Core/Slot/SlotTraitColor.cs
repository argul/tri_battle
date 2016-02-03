using System;

public class SlotTraitColor : SlotTrait
{
	public byte r;
	public byte g;
	public byte b;
	public byte a;
	public SlotTraitColor() {}
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

	protected override string SerializeUID { get { return SLOT_TRAIT_COLOR; } }

	public override string Serialize ()
	{
		return JsonHelper.Serialize(this);
	}

	public override void Deserialize (string str)
	{
		var obj = JsonHelper.Deserialize<SlotTraitColor>(str);
		r = obj.r;
		g = obj.g;
		b = obj.b;
		a = obj.a;
	}
}