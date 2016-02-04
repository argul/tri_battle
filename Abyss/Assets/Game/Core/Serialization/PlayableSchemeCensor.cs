using System;
using System.Collections.Generic;

public static class PlayableSchemeCensor
{
	public const int minimalPLMCount = 3;
	private static Dictionary<SchemeCategory, Predicate<PlayableScheme>> procedures = new Dictionary<SchemeCategory, Predicate<PlayableScheme>>()
	{
		{ SchemeCategory.RECTANGULAR_2D, Procedure2DR }
	};
	public static bool CensorScheme(SchemeCategory cat, PlayableScheme scheme)
	{
		return procedures[cat].Invoke(scheme);
	}

	private static bool Procedure2DR(PlayableScheme scheme)
	{
		var container = new Container2D_Rectangular(scheme.canvasConfig as CanvasConfig2DR,
		                                         scheme.slotConfig);
		var plmContext = new PLMSeeker2D_Retangular.SeekContext();
		plmContext.container = container;
		plmContext.matchRules = scheme.matchRules.SchemeStyleMap<RuleMatchBasic, RuleMatchBasic2D_Rectangular>((r)=>{
			return r as RuleMatchBasic2D_Rectangular;
		}).ToArray();
		plmContext.operationRules = scheme.operationRules.SchemeStyleMap<RuleOperation, RuleOperation2D_Rectangular>((r)=>{
			return r as RuleOperation2D_Rectangular;
		}).ToArray();

		var valid = 0;
		var total = 20;
		for (int i = 0; i < total; i++)
		{
			container.RecreateSubjects(true);
			plmContext.result = new List<PLMRecord2D_Retangular>();
			PLMSeeker2D_Retangular.Seek(plmContext);
			if (plmContext.result.Count >= minimalPLMCount)
			{
				valid++;
			}
		}
		return valid >= 2;
	}
}
