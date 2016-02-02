using System;
using System.Collections.Generic;

public abstract class SlotConfig
{
	public int randomSeed = 0;
	public abstract SlotTrait[] Traits { get; }
	public abstract Dictionary<string, SlotSpecialty> Specials { get; }
}

public class SlotConfig_Hardcoded : SlotConfig
{
	public SlotConfig_Hardcoded()
	{
		var red = new SlotTraitColor(255, 0, 0, 255);
		var green = new SlotTraitColor(0, 255, 0, 255);
		var blue = new SlotTraitColor(0, 0, 255, 255);
		var yellow = new SlotTraitColor(255, 255, 0, 255);
		var syan = new SlotTraitColor(0, 255, 255, 255);
		var purple = new SlotTraitColor(255, 0, 255, 255);
		traits[0] = red;
		traits[1] = green;
		traits[2] = blue;
		traits[3] = yellow;
		traits[4] = syan;
		traits[5] = purple;
		randomSeed = 0;
	}
	private SlotTrait[] traits = new SlotTrait[6];
	public override SlotTrait[] Traits {
		get {
			return traits;
		}
	}
	public override Dictionary<string, SlotSpecialty> Specials {
		get {
			throw new NotImplementedException ();
		}
	}
}