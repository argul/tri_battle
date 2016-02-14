using System;
using System.Collections.Generic;

public enum SchemeCategory
{
	UNKNOWN = -1,
	RECTANGULAR_2D,
}

public class PlayableScheme : ISerializable
{
	public PlayableScheme() {}
	public PlayableScheme(DumpWrapper dump)
	{
		InitFrom(dump);
	}

	public string Name { get;set; }
	public int minimalPlayablePLM;
	public CanvasConfig canvasConfig;
	public List<RuleMatchBasic> matchRules;
	public SlotConfig slotConfig;
	public List<RuleOperation> operationRules;
	public List<RuleMatchExtension> extensionRules;
	public List<RuleScore> scoreRules;
	public RuleRefill refillRule;

	public DumpWrapper Dump()
	{
		var ret = new DumpWrapper();
		ret.name = Name;
		ret.minimalPLMDump = minimalPlayablePLM;
		ret.canvasDump = canvasConfig.Serialize();
		ret.matchRuleDumps = matchRules.SchemeStyleMap<RuleMatchBasic, string>((r)=>{
			return r.Serialize();
		});
		ret.operationRuleDumps = operationRules.SchemeStyleMap<RuleOperation, string>((r)=>{
			return r.Serialize();
		});
		ret.extensionRuleDumps = extensionRules.SchemeStyleMap<RuleMatchExtension, string>((r)=>{
			return r.Serialize();
		});
		ret.scoreRuleDumps = scoreRules.SchemeStyleMap<RuleScore, string>((r)=>{
			return r.Serialize();
		});
		ret.refillRuleDump = refillRule.Serialize();
		ret.traitDumps = slotConfig.Traits.SchemeStyleMap<SlotTrait, string>((t)=>{
			return t.Serialize();
		});
		ret.specialDumps = new List<string>();
		foreach (var kvp in slotConfig.Specials)
		{
			ret.specialDumps.Add(kvp.Value.Serialize());
		}
		return ret;
	}

	private void InitFrom(DumpWrapper dump)
	{
		Name = dump.name;
		minimalPlayablePLM = dump.minimalPLMDump;
		canvasConfig = CanvasConfig.StaticDeserialize(dump.canvasDump);
		slotConfig = new SlotConfig();
		slotConfig.Init(dump.traitDumps, dump.specialDumps);
		matchRules = dump.matchRuleDumps.SchemeStyleMap<string, RuleMatchBasic>((str)=>{
			return RuleMatchBasic.StaticDeserialize(str);
		});
		operationRules = dump.operationRuleDumps.SchemeStyleMap<string, RuleOperation>((str)=>{
			return RuleOperation.StaticDeserialize(str);
		});
		extensionRules = dump.extensionRuleDumps.SchemeStyleMap<string, RuleMatchExtension>((str)=>{
			return RuleMatchExtension.StaticDeserialize(str);
		});
		scoreRules = dump.scoreRuleDumps.SchemeStyleMap<string, RuleScore>((str)=>{
			return RuleScore.StaticDeserialize(str);
		});
		refillRule = RuleRefill.StaticDeserialize(dump.refillRuleDump);
	}

	public string Serialize()
	{
		return JsonHelper.Serialize(Dump());
	}

	public void Deserialize(string str)
	{
		var dump = JsonHelper.Deserialize<DumpWrapper>(str);
		InitFrom(dump);
	}
}

public class DumpWrapper
{
	public string name;
	public int minimalPLMDump;
	public string canvasDump;
	public List<string> matchRuleDumps;
	public List<string> operationRuleDumps;
	public List<string> extensionRuleDumps;
	public List<string> scoreRuleDumps;
	public string refillRuleDump;
	public List<string> traitDumps;
	public List<string> specialDumps;
}