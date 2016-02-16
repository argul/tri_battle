using System;
using System.Collections.Generic;

public class PlayableEnvBuilder
{
	public PlayableEnv2DR Build_2DR_Hardcoded()
	{
		var env = new PlayableEnv2DR();
		env.MinimalPlayablePLM = 3;

		var canvasConfig = new CanvasConfig2DR();
		canvasConfig.mapWidth = 10;
		canvasConfig.mapHeight = 10;
		canvasConfig.insulators = new List<Pos2D>();
		var slotConfig = new SlotConfig_Hardcoded();
		env.Foreground = new Container2D_Rectangular(canvasConfig, slotConfig);
		env.Background = new Container2D_Rectangular(canvasConfig, slotConfig);

		var matchRules = new RuleMatchBasic2D_Rectangular[2]
		{
			new RuleMatchBasicHorizontal(),
			new RuleMatchBasicVertical()
		};
		foreach (var r in matchRules)
		{
			r.Compile();
		}
		env.MatchRules = matchRules;

		env.ExtensionRules = new RuleMatchExtension2D_Rectangular[0];



		env.RefillRule = new RuleRefill2DR_Downward();

		env.OperationRules = new RuleOperation2D_Rectangular[2]
		{
			new RuleOperationHorizontal(),
			new RuleOperationVertical()
		};

		env.ScoreRules = new RuleScore2D_Rectangular[0];

		return env;
	}

	public PlayableEnv2DR Build_2DR(PlayableScheme scheme)
	{
		var env = new PlayableEnv2DR();
		env.MinimalPlayablePLM = scheme.minimalPlayablePLM;
		env.Foreground = new Container2D_Rectangular(scheme.canvasConfig as CanvasConfig2DR,
		                                             scheme.slotConfig);
		env.Background = new Container2D_Rectangular(scheme.canvasConfig as CanvasConfig2DR,
		                                             scheme.slotConfig);

		env.MatchRules = scheme.matchRules.SchemeStyleMap<RuleMatchBasic, RuleMatchBasic2D_Rectangular>((r)=>{
			var raw = r as RuleMatchBasic2D_Rectangular;
			var ret = new RuleMatchBasic2D_Rectangular();
			ret.maskWidth = raw.maskWidth;
			ret.maskHeight = raw.maskHeight;
			ret.mask = raw.mask;
			ret.Compile();
			return ret;
		}).ToArray();

		env.ExtensionRules = scheme.extensionRules.SchemeStyleMap<RuleMatchExtension, RuleMatchExtension2D_Rectangular>((r)=>{
			return r as RuleMatchExtension2D_Rectangular;
		}).ToArray();

		env.RefillRule = scheme.refillRule as RuleRefill2D_Rectangular;

		env.OperationRules = scheme.operationRules.SchemeStyleMap<RuleOperation, RuleOperation2D_Rectangular>((r)=>{
			return r as RuleOperation2D_Rectangular;
		}).ToArray();

		env.ScoreRules = scheme.scoreRules.SchemeStyleMap<RuleScore, RuleScore2D_Rectangular>((r)=>{
			return r as RuleScore2D_Rectangular;
		}).ToArray();

		return env;
	}
}
