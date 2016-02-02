using System;
using System.Collections.Generic;

public class PLMRecord2D_Retangular
{
	public int x1;
	public int y1;
	public int x2;
	public int y2;
	public RuleMatchBasic2D_Rectangular rule;
	public RuleOperation2D_Rectangular operation;
}

// Potential Legal Matching Seeker
public static class PLMSeeker2D_Retangular
{
	public class SeekContext
	{
		public List<PLMRecord2D_Retangular> result;
		public Container2D_Rectangular container;
		public RuleMatchBasic2D_Rectangular[] matchRules;
		public RuleOperation2D_Rectangular[] operationRules;
		// TODO : support multiply operations
		//public int operationTimes = 1;
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
		for (int y = 0; y < ctx.container.Height - rule.maskHeight + 1; y++)
		{
			for (int x = 0; x < ctx.container.Width - rule.maskWidth + 1; x++)
			{
				Match_Internal(ctx, x, y, rule, 0);
				Match_Internal(ctx, x, y, rule, rule.scatters.Length - 1);
			}
		}
	}

	private static List<PLMRecord2D_Retangular> intermediate = new List<PLMRecord2D_Retangular>();
	private static void Match_Internal(SeekContext ctx,
	                                   int x,
	                                   int y,
	                                   RuleMatchBasic2D_Rectangular matchRule,
	                                   int scatterIndex)
	{
		intermediate.Clear();
		var slots = ctx.container.WrapperRect;
		var sample = slots[y + matchRule.scatters[scatterIndex].y, x + matchRule.scatters[scatterIndex].x];
		if (!sample.IsTarget)
		{
			return;
		}
		var operationUsed = false;
		for (int i = 0; i < matchRule.scatters.Length; ++i)
		{
			if (i == scatterIndex) continue;
			var targetX = x + matchRule.scatters[i].x;
			var targetY = y + matchRule.scatters[i].y;
			var sniff = slots[targetY, targetX];
			if (!sniff.IsTarget)
			{
				return;
			}
			if (!sample.slotAttribute.trait.AbsoluteEqual(sniff.slotAttribute.trait))
			{
				if (operationUsed) return;

				foreach (var operable in ctx.operationRules)
				{
					if (JudgeOperable(sample.slotAttribute.trait,
					                  ctx.container,
					                  x, y,
					                  matchRule.scatters[i].x, matchRule.scatters[i].y,
					                  matchRule, operable, false))
					{
						var record = new PLMRecord2D_Retangular();
						record.operation = operable;
						record.rule = matchRule;
						record.x1 = targetX;
						record.y1 = targetY;
						record.x2 = targetX + operable.xRelative;
						record.y2 = targetY + operable.yRelative;
						intermediate.Add(record);
						operationUsed = true;
					}
					if (JudgeOperable(sample.slotAttribute.trait,
					                  ctx.container,
					                  x, y,
					                  matchRule.scatters[i].x, matchRule.scatters[i].y,
					                  matchRule, operable, true))
					{
						var record = new PLMRecord2D_Retangular();
						record.operation = operable;
						record.rule = matchRule;
						record.x1 = targetX;
						record.y1 = targetY;
						record.x2 = targetX - operable.xRelative;
						record.y2 = targetY - operable.yRelative;
						intermediate.Add(record);
						operationUsed = true;
					}
				}
				if (!operationUsed) return;
			}
 		}

		// A pure match!
		if (!operationUsed) return;

		ctx.result.AddRange(intermediate);
		intermediate.Clear();
	}

	private static bool JudgeOperable(SlotTrait matchingTrait,
	                                  Container2D_Rectangular container,
	                                  int xMask, int yMask,
	                                  int xInMask, int yInMask,
	                                  RuleMatchBasic2D_Rectangular match,
	                                  RuleOperation2D_Rectangular operation,
	                                  bool inverse)
	{
		int xRelative = inverse ? -operation.xRelative : operation.xRelative;
		int yRelative = inverse ? -operation.yRelative : operation.yRelative;

		int xTouch = xInMask + xRelative;
		int yTouch = yInMask + yRelative;
		if (yTouch >= 0 && yTouch < match.maskHeight &&
		    xTouch >= 0 && xTouch < match.maskWidth &&
		    match.PeekMask(xTouch, yTouch))
		{
			return false;
		}

		var sniffX = xMask + xInMask + xRelative;
		var sniffY = yMask + yInMask + yRelative;
		if (!container.IsLegalPosition(sniffX, sniffY))
		{
			return false;
		}

		var sniff = container.GetSlot(sniffX, sniffY);
		if (!sniff.IsTarget)
		{
			return false;
		}

		return matchingTrait.AbsoluteEqual(sniff.slotAttribute.trait);
	}
}
