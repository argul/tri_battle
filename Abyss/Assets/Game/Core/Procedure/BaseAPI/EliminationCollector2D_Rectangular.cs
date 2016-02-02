using System;
using System.Collections.Generic;

public static class EliminationCollector2D_Rectangular
{
	public class EliminateContext
	{
		public bool[,] sandbox;
		public Container2D_Rectangular container;
		public List<LMRecord2D_Retangular> records;
		public RuleMatchExtension2D_Rectangular[] extensionRules;
	}
	public static void CollectElimination(EliminateContext ctx)
	{
		var extensionResults = new List<Pos2D>();
		foreach (var record in ctx.records)
		{
			foreach (var scatter in record.rule.scatters)
			{
				ctx.sandbox[record.y + scatter.y, record.x + scatter.x] = true;
			}
			foreach (var extension in ctx.extensionRules)
			{
				if (extension.Apply(ctx.container, record, extensionResults))
				{
					for (int i = 0, len = extensionResults.Count; i < len; ++i)
					{
						var e = extensionResults[i];
						ctx.sandbox[e.y, e.x] = true;
					}
					extensionResults.Clear();
				}
			}
		}

		ctx.container.ForeachSlot((x, y, slot)=>{
			if (null != slot && slot.slotAttribute.category == SlotAttribute.Category.SPECIAL){
				slot.slotAttribute.specialty.Trigger(SlotSpecialty.TRIGGER_STAGE_ELIMINATION,
				                                     ctx.container, 
				                                     ctx.sandbox);
			}
		});
	}
}