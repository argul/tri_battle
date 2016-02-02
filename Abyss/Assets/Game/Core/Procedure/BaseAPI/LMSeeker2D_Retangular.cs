using System;
using System.Collections.Generic;

public class LMRecord2D_Retangular
{
	public int x;
	public int y;
	public RuleMatchBasic2D_Rectangular rule;
}

// Legal Matching Seeker
public static class LMSeeker2D_Retangular
{
	public class SeekContext
	{
		public List<LMRecord2D_Retangular> result;
		public Container2D_Rectangular container;
		public RuleMatchBasic2D_Rectangular[] matchRules;
	}
	public static void Seek(SeekContext ctx)
	{
		foreach (var rule in ctx.matchRules)
		{
			Seek_Internal(ctx, rule);
		}
	}

	private static void Seek_Internal(SeekContext ctx, RuleMatchBasic2D_Rectangular rule)
	{
		var slots = ctx.container.WrapperRect;

		for (int y = 0; y < ctx.container.Height - rule.maskHeight + 1; y++)
		{
			for (int x = 0; x < ctx.container.Width - rule.maskWidth + 1; x++)
			{
				var r = Match_Internal(slots, x, y, rule);
				if (null != r) ctx.result.Add(r);
			}
		}
	}

	private static LMRecord2D_Retangular Match_Internal(SlotWrapper2D[,] slots,
	                                                    int x,
	                                                    int y,
	                                                    RuleMatchBasic2D_Rectangular matchRule)
	{
		var sample = slots[y + matchRule.scatters[0].y, x + matchRule.scatters[0].x];
		if (!sample.IsTarget)
		{
			return null;
		}
		for (int i = 1; i < matchRule.scatters.Length; ++i)
		{
			var sniff = slots[y + matchRule.scatters[i].y, x + matchRule.scatters[i].x];
			if (!sniff.IsTarget)
			{
				return null;
			}
			if (!sample.slotAttribute.trait.AbsoluteEqual(sniff.slotAttribute.trait))
			{
				return null;
			}
		}
		var record = new LMRecord2D_Retangular();
		record.x = x;
		record.y = y;
		record.rule = matchRule;
		return record;
	}
}