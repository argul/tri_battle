using System;
using System.Collections.Generic;

public abstract class RuleRefill2D_Rectangular : RuleRefill
{
	public abstract RefillFlowRecord Apply(Container2D_Rectangular container);
}

public class RefillFlowRecord
{
	public RefillFlowRecord()
	{
		OnSpotList = new List<Pos2D>();
		NonFillMovements = new List<Path>();
		FillMovements = new List<Path>();
	}
	public class Path
	{
		public Pos2D src = new Pos2D();
		public Pos2D dst = new Pos2D();
		public List<Pos2D> movements = new List<Pos2D>();
	}
	public List<Pos2D> OnSpotList { get; set; }
	public List<Path> NonFillMovements { get; set; }
	public List<Path> FillMovements { get; set; }
}

public class RuleRefillOnSpot : RuleRefill2D_Rectangular
{
	// Do nothing
	public override RefillFlowRecord Apply (Container2D_Rectangular container) 
	{
		throw new NotImplementedException();
	}
}