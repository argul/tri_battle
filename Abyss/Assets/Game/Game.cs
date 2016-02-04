using System;
using System.Collections.Generic;

public static class Game
{
	private static int schemeSelection = 0;
	public static int SchemeSelection 
	{
		set { schemeSelection = value; }
	}
	public static DumpWrapper SchemeDumpSelection
	{
		get { return dumps[schemeSelection]; }
	}
	public static bool IsClassicScheme
	{
		get { return 0 == schemeSelection; }
	}

	private static List<DumpWrapper> dumps = new List<DumpWrapper>();
	public static List<DumpWrapper> Dumps 
	{
		get { return dumps; }
	}

	public static void Launch()
	{
		dumps.Add(null);
	}

	private static PlayableScheme bufferScheme;
	public static PlayableScheme BufferScheme 
	{
		get { return bufferScheme; }
	}
	public static void CreateEmptyBuffer(string name)
	{
		bufferScheme = new PlayableScheme();
		bufferScheme.Name = name;

		bufferScheme.canvasConfig = new CanvasConfig2DR();
		var c = bufferScheme.canvasConfig as CanvasConfig2DR;
		c.mapWidth = 10;
		c.mapHeight = 10;
		c.insulators = new List<Pos2D>();

		bufferScheme.slotConfig = new SlotConfig();
		bufferScheme.slotConfig.Init(new List<string>(), new List<string>());

		bufferScheme.matchRules = new List<RuleMatchBasic>();
		bufferScheme.extensionRules = new List<RuleMatchExtension>();
		bufferScheme.operationRules = new List<RuleOperation>();
		bufferScheme.refillRule = new RuleRefill2DR_Downward();
		bufferScheme.scoreRules = new List<RuleScore>();
	}
	public static void DeleteSelectedScheme()
	{
		Assert.AssertIsTrue(schemeSelection > 0);
		dumps.RemoveAt(schemeSelection);
		schemeSelection = Math.Min(schemeSelection, dumps.Count - 1);
	}
}
