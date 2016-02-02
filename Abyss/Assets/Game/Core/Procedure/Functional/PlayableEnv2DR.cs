using System;
using System.Collections.Generic;

public class PlayableEnv2DR
{
	private PlayableEnvConfig cfg;
	public PlayableEnvConfig Cfg { set { cfg = value; } }

	private Container2D_Rectangular foreground;
	public Container2D_Rectangular Foreground {
		get { return foreground; }
		set { foreground = value; } 
	}

	private Container2D_Rectangular background;
	public Container2D_Rectangular Background { set { background = value; } }

	private RuleMatchBasic2D_Rectangular[] matchRules;
	public RuleMatchBasic2D_Rectangular[] MatchRules { set { matchRules = value; } }

	private RuleMatchExtension2D_Rectangular[] extensionRules;
	public RuleMatchExtension2D_Rectangular[] ExtensionRules { set { extensionRules = value; } }

	private RuleRefill2D_Rectangular refillRule;
	public RuleRefill2D_Rectangular RefillRule { set { refillRule = value; } }

	private RuleOperation2D_Rectangular[] operationRules;
	public RuleOperation2D_Rectangular[] OperationRules { set { operationRules = value; } }

	private List<PLMRecord2D_Retangular> plmRecords;
	// Debug only
	public List<PLMRecord2D_Retangular> PlmRecords {
		get {
			return plmRecords;
		}
	}

	public void InitPlayableContainer()
	{
		foreground.InitBlocks();
		background.FlushAsFirmware();

		foreground.RecreateSubjects(true);

		MakeContainerStable(foreground);

		plmRecords = MakeContainerPlayable(foreground);
	}

	private void MakeContainerStable(Container2D_Rectangular container)
	{
		var lmRecords = SeekContainerLM(container);
		if (lmRecords.Count <= 0)
		{
			return;
		}
		var sandbox = CollectContainerEliminate(container, lmRecords);
		container.ForeachSlot((x, y, slot)=>{
			if (sandbox[y,x]){
				container.ClearSlot(x, y);
				container.FillSlot(x, y);
			}
		});
		MakeContainerStable(container);
	}

	private List<PLMRecord2D_Retangular> MakeContainerPlayable(Container2D_Rectangular container)
	{
		var records = SeekContainerPLM(container);
		if (records.Count >= cfg.minimalPlayablePLMCount)
		{
			return records;
		}
		else
		{
			container.RecreateSubjects(true);
			MakeContainerStable(container);
			return MakeContainerPlayable(container);
		}
	}

	private List<LMRecord2D_Retangular> SeekContainerLM(Container2D_Rectangular container)
	{
		var ret = new List<LMRecord2D_Retangular>();
		var seekCtx = new LMSeeker2D_Retangular.SeekContext();
		seekCtx.container = container;
		seekCtx.matchRules = matchRules;
		seekCtx.result = ret;
		LMSeeker2D_Retangular.Seek(seekCtx);
		return ret;
	}

	private List<PLMRecord2D_Retangular> SeekContainerPLM(Container2D_Rectangular container)
	{
		var seekCtx = new PLMSeeker2D_Retangular.SeekContext();
		seekCtx.container = container;
		seekCtx.matchRules = matchRules;
		seekCtx.operationRules = operationRules;
		seekCtx.result = new List<PLMRecord2D_Retangular>();
		PLMSeeker2D_Retangular.Seek(seekCtx);
		var ret = seekCtx.result;
		return ret;
	}

	private bool[,] CollectContainerEliminate(Container2D_Rectangular container,
	                                          List<LMRecord2D_Retangular> lmRecords)
	{
		var eliminationCtx = new EliminationCollector2D_Rectangular.EliminateContext();
		eliminationCtx.container = container;
		eliminationCtx.extensionRules = extensionRules;
		eliminationCtx.sandbox = new bool[container.Height,container.Width];
		eliminationCtx.records = lmRecords;
		EliminationCollector2D_Rectangular.CollectElimination(eliminationCtx);
		return eliminationCtx.sandbox;
	}

	public bool IsLegalOperation(OperationInput op)
	{
		var xRelative = op.x1 - op.x2;
		var yRelative = op.y1 - op.y2;
		foreach (var r in operationRules)
		{
			if (xRelative == r.xRelative && yRelative == r.yRelative) return true;
			if (xRelative == -r.xRelative && yRelative == -r.yRelative) return true;
		}
		return false;
	}

	public OperationOutput PerformOperation(OperationInput op)
	{
		var ret = new OperationOutput();
		ret.IsRejected = true;
		foreach (var plm in plmRecords)
		{
			if ((plm.x1 == op.x1 && plm.y1 == op.y1 && plm.x2 == op.x2 && plm.y2 == op.y2) ||
			    (plm.x1 == op.x2 && plm.y1 == op.y2 && plm.x1 == op.x2 && plm.y1 == op.y2))
			{
				ret.IsRejected = false;
				break;
			}
		}
		if (ret.IsRejected)
		{
			return ret;
		}
		else
		{
			ret.episodes = new List<OperationOutput.Episode>();
		}

		foreground.SwapSlot(op.x1, op.y1, op.x2, op.y2);

		var lmRecords = new List<LMRecord2D_Retangular>();
		lmRecords = SeekContainerLM(foreground);

		do 
		{
			var elimination = new OperationOutput.Episode(OperationOutput.EpisodeType.ELIMINATION);
			elimination.elimination = new List<Pos2D>();
			ret.episodes.Add(elimination);

			var sandbox = CollectContainerEliminate(foreground, lmRecords);
			foreground.ForeachSlot((x, y, slot)=>{
				if (sandbox[y, x]){
					elimination.elimination.Add(new Pos2D(x, y));
					foreground.ClearSlot(x, y);
				}
			});
			ret.episodes.Add(DoRefill());

			lmRecords = SeekContainerLM(foreground);
			if (0 == lmRecords.Count)
			{
				plmRecords = SeekContainerPLM(foreground);
				if (plmRecords.Count >= cfg.minimalPlayablePLMCount)
				{
					break;
				}
				else
				{
					foreground.RecreateSubjects(true);
					MakeContainerStable(foreground);
					MakeContainerPlayable(foreground);
					var shuffle = new OperationOutput.Episode(OperationOutput.EpisodeType.SHUFFLE);
					shuffle.shuffle = new SlotAttribute[foreground.Height,foreground.Width];
					foreground.ForeachSlot((x, y, slot)=>{
						shuffle.shuffle[y, x] = slot.slotAttribute;
					});
					ret.episodes.Add(shuffle);
					break;
				}
			}
		}
		while (true);

		return ret;
	}

	private OperationOutput.Episode DoRefill()
	{
		var ret = new OperationOutput.Episode(OperationOutput.EpisodeType.REFILL);
		ret.refillFlow = refillRule.Apply(foreground);
		ret.refillSlots = new SlotAttribute[foreground.Height, foreground.Width];

		foreground.ForeachSlot((x, y, slot)=>{
			if (null == foreground.WrapperRect[y, x]){
				background.ClearSlot(x, y);
				background.FillSlot(x, y);
			}
		});
		MakeContainerStable(background);
		background.ForeachSlot((x, y, slot)=>{
			if (slot.slotAttribute.category != SlotAttribute.Category.INSULATOR){
				foreground.SetSlot(x, y, slot);
			}
		});
		background.FlushAsFirmware();
		foreground.ForeachSlot((x, y, slot)=>{
			ret.refillSlots[y, x] = slot.slotAttribute;
		});
		return ret;
	}
}

public class PlayableEnvConfig
{
	public int minimalPlayablePLMCount;
}

public class OperationInput
{
	public int x1;
	public int y1;
	public int x2;
	public int y2;
}

public class OperationOutput
{
	public bool IsRejected { get; set; }
	public List<Episode> episodes;

	public enum EpisodeType
	{
		ERROR = -1,
		ELIMINATION,
		REFILL,
		SHUFFLE
	}
	public class Episode
	{
		public Episode(EpisodeType etype)
		{
			this.etype = etype;
		}
		public EpisodeType etype = EpisodeType.ERROR;

		public Tuple<Pos2D, Pos2D> exchange;

		public List<Pos2D> elimination;

		public SlotAttribute[,] shuffle;

		public SlotAttribute[,] refillSlots;
		public RefillFlowRecord refillFlow;
	}
}