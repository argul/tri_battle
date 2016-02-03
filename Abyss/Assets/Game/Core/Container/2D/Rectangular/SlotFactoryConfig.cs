using System;
using System.Collections.Generic;

public abstract class SlotConfig
{
	public int randomSeed = 0;
	public abstract List<SlotTrait> Traits { get; }
	public abstract Dictionary<string, SlotSpecialty> Specials { get; }

	public abstract void Init(List<string> traitDumps, List<string> specialDumps);
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
		traits.Add(red);
		traits.Add(green);
		traits.Add(blue);
		traits.Add(yellow);
		traits.Add(syan);
		traits.Add(purple);
		randomSeed = 0;
	}
	private List<SlotTrait> traits = new List<SlotTrait>();
	public override List<SlotTrait> Traits {
		get {
			return traits;
		}
	}
	public override Dictionary<string, SlotSpecialty> Specials {
		get {
			throw new NotImplementedException ();
		}
	}

	public override void Init (List<string> traitDumps, List<string> specialDumps)
	{
		throw new NotImplementedException ();
	}
}

public class SlotConfigFromFile : SlotConfig
{
	private List<SlotTrait> traits;
	private Dictionary<string, SlotSpecialty> specials;
	public override List<SlotTrait> Traits { get { return traits; } }
	public override Dictionary<string, SlotSpecialty> Specials { get { return specials; } }

	public override void Init (List<string> traitDumps, List<string> specialDumps)
	{
		throw new NotImplementedException ();
	}
}