using System;
using System.Collections.Generic;

public class SlotConfig
{
	protected bool constantRndSeed = false;
	public bool ConstantRndSeed { get { return constantRndSeed; } }

	protected List<SlotTrait> traits;
	protected Dictionary<string, SlotSpecialty> specials;
	public List<SlotTrait> Traits { get { return traits; } }
	public Dictionary<string, SlotSpecialty> Specials { get { return specials; } }
	
	public virtual void Init (List<string> traitDumps, List<string> specialDumps)
	{
		traits = traitDumps.SchemeStyleMap<string, SlotTrait>((str)=>{
			return SlotTrait.StaticDeserialize(str);
		});
		var tmp = specialDumps.SchemeStyleMap<string, SlotSpecialty>((str)=>{
			return SlotSpecialty.StaticDeserialize(str);
		});
		specials = new Dictionary<string, SlotSpecialty>();
		foreach (var s in tmp)
		{
			specials.Add(s.Name, s);
		}
	}
}

public class SlotConfig_Hardcoded : SlotConfig
{
	public SlotConfig_Hardcoded()
	{
		traits = new List<SlotTrait>();
		specials = new Dictionary<string, SlotSpecialty>();
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
		constantRndSeed = true;
	}

	public override void Init (List<string> traitDumps, List<string> specialDumps)
	{
		throw new NotImplementedException ();
	}
}