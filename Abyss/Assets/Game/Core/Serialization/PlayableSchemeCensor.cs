using System;
using System.Collections.Generic;

public static class PlayableSchemeCensor
{
	private static bool CheckMatchRule(RuleMatchBasic rule, out string reason)
	{
		reason = "";
		var r1 = rule as RuleMatchBasic2D_Rectangular;
		if (null != r1)
		{
			int count = 0;
			foreach (var b in r1.mask)
			{
				if (b) count++;
			}
			if (count <= 2)
			{
				reason = "Illegal Match Rule!";
				return false;
			}

			return true;
		}
		throw new NotImplementedException();
	}

	private static bool CheckOperationRule(RuleOperation rule, out string reason)
	{
		reason = "";
		var r1 = rule as RuleOperation2D_Rectangular;
		if (null != r1)
		{
			if (r1.xRelative == 0 && r1.yRelative == 0)
			{
				reason = "Illegal Operation Rule!";
				return false;
			}
			
			return true;
		}
		throw new NotImplementedException();
	}

	private static bool CheckTraits(List<SlotTrait> traits, out string reason)
	{
		// TODO
		reason = "";
		return true;
	}

	private static bool CheckCanvas(CanvasConfig canvas, out string reason)
	{
		reason = "";
		var c1 = canvas as CanvasConfig2DR;
		if (null != c1)
		{
			if (c1.mapWidth <= 2
			    || c1.mapWidth >= 30
			    || c1.mapHeight <= 2
			    || c1.mapHeight >= 30)
			{
				reason = "Illegal Canvas Config!";
				return false;
			}
			
			return true;
		}
		throw new NotImplementedException();
	}

	private static bool DryRun(PlayableScheme scheme, out string reason)
	{
		reason = "";
		if (scheme.canvasConfig is CanvasConfig2DR)
		{
			var container = new Container2D_Rectangular(scheme.canvasConfig as CanvasConfig2DR,
			                                            scheme.slotConfig);
			container.InitBlocks();

			var count1 = 0;
			var count2 = 0;
			for (int i = 0; i < 20; i++)
			{
				container.RecreateSubjects(true);
				var ctx1 = new LMSeeker2D_Retangular.SeekContext();
				ctx1.container = container;
				ctx1.matchRules = scheme.matchRules.SchemeStyleMap<RuleMatchBasic, RuleMatchBasic2D_Rectangular>((r)=>{
					var raw = r as RuleMatchBasic2D_Rectangular;
					var ret = new RuleMatchBasic2D_Rectangular();
					ret.mask = raw.mask;
					ret.maskWidth = raw.maskWidth;
					ret.maskHeight = raw.maskHeight;
					ret.Compile();
					return ret;
				}).ToArray();
				ctx1.result = new List<LMRecord2D_Retangular>();
				LMSeeker2D_Retangular.Seek(ctx1);
				if (ctx1.result.Count > 0)
				{
					count1++;
				}

				var ctx2 = new PLMSeeker2D_Retangular.SeekContext();
				ctx2.container = container;
				ctx2.matchRules = scheme.matchRules.SchemeStyleMap<RuleMatchBasic, RuleMatchBasic2D_Rectangular>((r)=>{
					var raw = r as RuleMatchBasic2D_Rectangular;
					var ret = new RuleMatchBasic2D_Rectangular();
					ret.mask = raw.mask;
					ret.maskWidth = raw.maskWidth;
					ret.maskHeight = raw.maskHeight;
					ret.Compile();
					return ret;
				}).ToArray();
				ctx2.operationRules = scheme.operationRules.SchemeStyleMap<RuleOperation, RuleOperation2D_Rectangular>((r)=>{
					return r as RuleOperation2D_Rectangular;
				}).ToArray();
				ctx2.result = new List<PLMRecord2D_Retangular>();
				PLMSeeker2D_Retangular.Seek(ctx2);
				if (ctx2.result.Count >= scheme.minimalPlayablePLM)
				{
					count2++;
				}
			}

			if (count1 < 2 || count2 < 2)
			{
				reason = "Scheme is NOT playable!";
				return false;
			}

			return true;
		}
		throw new NotImplementedException();
	}

	public static bool CheckScheme(PlayableScheme scheme, out string reason)
	{
		reason = "";
		if (String.IsNullOrEmpty(scheme.Name))
		{
			reason = "Invalid Scheme Name!";
			return false;
		}
		if (null == scheme.canvasConfig)
		{
			reason = "No Canvas Config";
			return false;
		}
		if (!CheckCanvas(scheme.canvasConfig, out reason))
		{
			return false;
		}
		if (null == scheme.slotConfig || null == scheme.slotConfig.Traits || scheme.slotConfig.Traits.Count <= 1)
		{
			reason = "Illegal slot trait config!";
			return false;
		}
		if (null == scheme.matchRules || scheme.matchRules.Count <= 0)
		{
			reason = "No Match Rule Found!";
			return false;
		}
		foreach (var r in scheme.matchRules)
		{
			if (!CheckMatchRule(r, out reason))
			{
				return false;
			}
		}
		if (null == scheme.operationRules || scheme.operationRules.Count <= 0)
		{
			reason = "No Operation Rule Found!";
			return false;
		}
		foreach (var r in scheme.operationRules)
		{
			if (!CheckOperationRule(r, out reason))
			{
				return false;
			}
		}
		if (!DryRun(scheme, out reason))
		{
			return false;
		}

		return true;
	}
}
