using System;
using System.Collections.Generic;

public class PlayableEnvBuilder
{
	public PlayableEnv2DR BuildHardcodedEnv()
	{
		var env = new PlayableEnv2DR();
		var containerTemplate = new Container2D_Rectangular.TemplateInfo();
		containerTemplate.width = 10;
		containerTemplate.height = 10;
		containerTemplate.insulators = new List<Pos2D>();

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

		var cfg = new PlayableEnvConfig();
		cfg.minimalPlayablePLMCount = 3;
		env.Cfg = cfg;

		env.RefillRule = new RuleRefill2DR_Downward();

		env.OperationRules = new RuleOperation2D_Rectangular[2]
		{
			new RuleOperationHorizontal(),
			new RuleOperationVertical()
		};

		var containerCfg = new SlotConfig_Hardcoded();
		env.Foreground = new Container2D_Rectangular(containerTemplate, containerCfg);
		env.Background = new Container2D_Rectangular(containerTemplate, containerCfg);

		return env;
	}
}
