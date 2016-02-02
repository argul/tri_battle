using System;
using System.Collections.Generic;

public abstract class RuleMatchExtension2D_Rectangular : RuleMatchExtension
{
	public abstract bool Apply(Container2D_Rectangular container,
	                           LMRecord2D_Retangular matchRecord,
	                           List<Pos2D> results);
}