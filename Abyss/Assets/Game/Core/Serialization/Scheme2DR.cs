using System;
using System.Collections.Generic;

public class Scheme2DR : ISerializable
{
	public int canvasWidth;
	public int canvasHeight;
	public List<Pos2D> obstacles = new List<Pos2D>();
	public List<RuleMatchBasic> matchRules = new List<RuleMatchBasic>();
	public SlotConfig slotConfig;
	public List<RuleOperation> operationRules = new List<RuleOperation>();
	public List<RuleMatchExtension> extensionRules = new List<RuleMatchExtension>();
	public List<RuleScore> scoreRules = new List<RuleScore>();

	private DumpWrapper Dump()
	{
		var ret = new DumpWrapper();
		ret.canvasWidth = canvasWidth;
		ret.canvasHeight = canvasHeight;
		ret.obstaclesDumps = obstacles;
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
		ret.traitDumps = slotConfig.Traits.SchemeStyleMap<SlotTrait, string>((t)=>{
			return t.Serialize();
		});
//		ret.specialDumps = slotConfig.Specials.Values.SchemeStyleMap<SlotTrait, string>((t)=>{
//			return t.Serialize();
//		});
		return ret;
	}

	private void InitFrom(DumpWrapper dump)
	{
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
	public int canvasWidth;
	public int canvasHeight;
	public List<Pos2D> obstaclesDumps;
	public List<string> matchRuleDumps;
	public List<string> operationRuleDumps;
	public List<string> extensionRuleDumps;
	public List<string> scoreRuleDumps;
	public List<string> traitDumps;
	public List<string> specialDumps;
}