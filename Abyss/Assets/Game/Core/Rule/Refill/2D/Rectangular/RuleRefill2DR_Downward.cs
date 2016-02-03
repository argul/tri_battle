using System;
using System.Collections.Generic;

public class RuleRefill2DR_Downward : RuleRefill2D_Rectangular
{
	public override string SerializeUID { get { return DOWNWARD_2DR; } }
	public override string Serialize () { return ""; }
	public override void Deserialize (string str) {}

	public override RefillFlowRecord Apply (Container2D_Rectangular container)
	{
		var key = GetType().Name + "-" + "RefillInfoMap";

		FillInfo[,] fillInfoMap = null;
		if (!container.UserData.ContainsKey(key))
		{
			List<Pos2D> pores = new List<Pos2D>();
			container.ForeachSlot((x, y, slot)=>{
				if (null == slot){
					pores.Add(new Pos2D(x, y));
					container.WrapperRect[y, x] = new SlotWrapper2D();
					container.WrapperRect[y, x].pos = new Pos2D(x, y);
				}
			});

			fillInfoMap = GenerateRefillTrendMap(container);
			container.UserData.Add(key, fillInfoMap);

			foreach (var p in pores)
			{
				container.ClearSlot(p.x, p.y);
			}
		}
		else
		{
			fillInfoMap = container.UserData[key] as FillInfo[,];
		}

		return DoApply(container, fillInfoMap);
	}

	// <offsetX, offsetY, hinder>
	private static Tuple<int,int,int>[] surroundingOffsets = new Tuple<int, int, int>[8]
	{
		new Tuple<int, int, int>(0, 1, 1),
		new Tuple<int, int, int>(-1, 1, 2),
		new Tuple<int, int, int>(1, 1, 2),
		new Tuple<int, int, int>(-1, 0, 3),
		new Tuple<int, int, int>(1, 0, 3),
		new Tuple<int, int, int>(-1, -1, 4),
		new Tuple<int, int, int>(1, -1, 4),
		new Tuple<int, int, int>(0, -1, 5)
	};
	public class FillInfo
	{
		public FillInfo()
		{
			ancestorPos = null;
		}
		public bool IsOnSpot { get { return null == ancestorPos; } }
		public Pos2D ancestorPos;
		public List<Pos2D> childrenPos;
	}
	private FillInfo[,] GenerateRefillTrendMap(Container2D_Rectangular container)
	{
		var ret = new FillInfo[container.Height, container.Width];
		var slots = container.WrapperRect;
		Action<int, int, List<SlotWrapper2D>> picker = (x, y, list)=>{
			if (x < 0 || x >= container.Width) return;
			if (y < 0 || y >= container.Height) return;
			if (null != slots[y, x].slotAttribute && slots[y, x].slotAttribute.category == SlotAttribute.Category.INSULATOR) return;
			list.Add(slots[y, x]);
		};
		Action<int, int> exitMarker = (x, y)=>{
			if (null == ret[y, x])
			{
				ret[y, x] = new FillInfo();
				ret[y, x].ancestorPos = new Pos2D(x, y + 1);
			}
		};

		container.ForeachSlot((x, y, slot)=>{
			if (null != ret[y, x]) 
				return;
			if (null != slot.slotAttribute && slot.slotAttribute.category == SlotAttribute.Category.INSULATOR)
				return;
			var ctx = new AStar.Context<SlotWrapper2D>();
			ctx.start = slots[y, x];
			ctx.procTrait = (s)=>{
				return s.Trait;
			};
			ctx.procWeight = (SlotWrapper2D from, SlotWrapper2D to)=>{
				foreach (var t in surroundingOffsets){
					if (to.pos.x == from.pos.x + t.item1 && to.pos.y == from.pos.y + t.item2){
						return t.item3;
					}
				}
				throw new NotImplementedException();
			};
			ctx.procTermination = (SlotWrapper2D s)=>{
				return s.pos.y == container.Height - 1;
			};
			ctx.procDistanceEstimator = (SlotWrapper2D s)=>{
				return container.Height - 1 - s.pos.y;
			};
			ctx.procAdjacencies = (SlotWrapper2D s)=>{
				var list = new List<SlotWrapper2D>();
				foreach (var t in surroundingOffsets){
					picker.Invoke(s.pos.x + t.item1, s.pos.y + t.item2, list);
				}
				return list;
			};
			if (AStar.Evaluate(ctx))
			{
				var result = ctx.path;
				if (result.Count <= 0)
				{
					exitMarker.Invoke(x, y);
				}
				else
				{
					result.Reverse();
					for (int i = 0, len = result.Count - 1; i < len; i++)
					{
						if (null != ret[result[i].pos.y, result[i].pos.x]) continue;
						var t = new FillInfo();
						t.ancestorPos = new Pos2D(result[i + 1].pos.x, result[i + 1].pos.y);
						ret[result[i].pos.y, result[i].pos.x] = t;
					}
					exitMarker(result[result.Count - 1].pos.x, result[result.Count - 1].pos.y);
				}
			}
			else
			{
				ret[y, x] = new FillInfo();
			}
		});

		for (int y = 0; y < container.Height; y++)
		{
			for (int x = 0; x < container.Width; x++)
			{
				var fi = ret[y, x];
				if (null == fi) continue;
				if (fi.IsOnSpot) continue;
				fi.childrenPos = new List<Pos2D>();
				foreach (var t in surroundingOffsets)
				{
					var sx = x + t.item1;
					var sy = y + t.item2;
					if (!container.IsLegalPosition(sx, sy)) continue;
					var touch = ret[sy, sx];
					if (null != touch.ancestorPos && touch.ancestorPos.x == x && touch.ancestorPos.y == y)
					{
						fi.childrenPos.Add(new Pos2D(sx, sy));
					}
				}
			}
		}

		return ret;
	}


	private RefillFlowRecord DoApply(Container2D_Rectangular container, FillInfo[,] fillInfoMap)
	{
		var ret = new RefillFlowRecord();
		var slots = container.WrapperRect;

		container.ForeachSlot((x, y, slot)=>{
			if (null != slot) return;
			var fi = fillInfoMap[y, x];
			if (fi.IsOnSpot){
				ret.OnSpotList.Add(new Pos2D(x, y));
			}
		});
		var ends = CollectRefillEnds(container, fillInfoMap);
		var dir = new Dictionary<int, Tuple<SlotWrapper2D, int>>();
		foreach (var e in ends)
		{
			var pos = fillInfoMap[e.y, e.x].ancestorPos;
			int inverseDepth = 0;
			while (container.IsLegalPosition(pos.x, pos.y))
			{
				var s = slots[pos.y, pos.x];
				if (null != s)
				{
					inverseDepth++;
					if (!dir.ContainsKey(s.Trait))
					{
						dir.Add(s.Trait, new Tuple<SlotWrapper2D, int>(s, inverseDepth));
					}
					else if (dir[s.Trait].item2 < inverseDepth)
					{
						dir[s.Trait].item2 = inverseDepth;
					}
				}
				pos = fillInfoMap[pos.y, pos.x].ancestorPos;
			}
		}
		var list = new List<Tuple<SlotWrapper2D, int>>();
		foreach (var kvp in dir)
		{
			list.Add(kvp.Value);
		}
		list.Sort((lhr, rhr)=>{
			return (lhr.item2 < rhr.item2) ? -1 : 1;
		});

		foreach (var t in list)
		{
			var src = t.item1.pos;
			var moveTo = DepthFirstSearch(slots, fillInfoMap, src.x, src.y);
			var dst = moveTo.item1;

			var path = new RefillFlowRecord.Path();
			path.src = src.Clone();
			path.dst = dst.Clone();

			container.SwapSlot(src.x, src.y, dst.x, dst.y);
			var cur = path.dst;
			do {
				path.movements.Add(cur);
				cur = fillInfoMap[cur.y, cur.x].ancestorPos;
			} while (cur != path.src);

			path.movements.Add(path.src);
			path.movements.Reverse();

			ret.NonFillMovements.Add(path);
		}

		ret.FillMovements = CollectFillPathList(container, fillInfoMap);

		return ret;
	}

	private List<Pos2D> CollectRefillEnds(Container2D_Rectangular container, FillInfo[,] fillInfoMap)
	{
		var slots = container.WrapperRect;
		var ret = new List<Pos2D>();

		container.ForeachSlot((x, y, slot)=>{
			if (null != slot) return;
			if (fillInfoMap[y, x].IsOnSpot) return;
			foreach (var t in surroundingOffsets){
				if (!container.IsLegalPosition(x + t.item1, y + t.item2)) continue;
				if (null != slots[y + t.item2, x + t.item1]) continue;
				var fi = fillInfoMap[y + t.item2, x + t.item1];
				if (!fi.IsOnSpot && fi.ancestorPos.x == x && fi.ancestorPos.y == y)
				{
					return;
				}
			}
			ret.Add(new Pos2D(x, y));
		});
		return ret;
	}

	private Tuple<Pos2D, int> DepthFirstSearch(SlotWrapper2D[,] slots,
	                                           FillInfo[,] infos, 
	                                           int fromX, int fromY)
	{
		Tuple<Pos2D, int> ret = null;
		var fi = infos[fromY, fromX];
		foreach (var t in fi.childrenPos)
		{
			if (null == slots[t.y, t.x])
			{
				var branchSearch = DepthFirstSearch(slots, infos, t.x, t.y);
				if (null == ret || branchSearch.item2 > ret.item2)
				{
					ret = branchSearch;
				}
			}
		}
		if (null == ret)
		{
			ret = new Tuple<Pos2D, int>(new Pos2D(fromX, fromY), 0);
		}
		else
		{
			ret.item2 = ret.item2 + 1;
		}
		return ret;
	}

	private List<RefillFlowRecord.Path> CollectFillPathList(Container2D_Rectangular container, FillInfo[,] fillInfos)
	{
		var ret = new List<RefillFlowRecord.Path>();
		for (int x = 0; x < container.Width; x++)
		{
			var sub = CollectFromPos(container, fillInfos, x, container.Height - 1);
			if (null == sub) continue;
			sub.Sort((lhr, rhr)=>{
				return (lhr.movements.Count > rhr.movements.Count) ? -1 : 1;
			});
			var len = sub.Count;
			for (int i = 0; i < len; i++)
			{
				var index = i + 1;
				var s = sub[i];
				s.src = new Pos2D(x, container.Height - 1 + index);
				for (int j = 1; j <= index; j++)
				{
					s.movements.Add(new Pos2D(x, container.Height - 1 + j));
				}
				s.movements.Reverse();
			}
			ret.AddRange(sub);
		}
		return ret;
	}

	private List<RefillFlowRecord.Path> CollectFromPos(Container2D_Rectangular container, FillInfo[,] fillInfos, int fromX, int fromY)
	{
		if (!container.IsLegalPosition(fromX, fromY) || null != container.GetSlot(fromX, fromY)) return null;
		var fi = fillInfos[fromY, fromX];
		var ret = new List<RefillFlowRecord.Path>();

		var selfPos = new Pos2D(fromX, fromY);
		var self = new RefillFlowRecord.Path();
		self.dst = selfPos;
		ret.Add(self);

		foreach (var child in fi.childrenPos)
		{
			var sub = CollectFromPos(container, fillInfos, child.x, child.y);
			if (null == sub) continue;
			ret.AddRange(sub);
		}
		foreach (var r in ret)
		{
			r.src = selfPos;
			r.movements.Add(selfPos);
		}
		return ret;
	}
}
