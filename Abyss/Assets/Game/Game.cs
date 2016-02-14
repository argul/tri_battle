using System;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
	private static int selection = 0;
	public static int Selection 
	{
		set { selection = value; }
		get { return selection; }
	}
	public static bool IsClassicScheme
	{
		get { return 0 == selection; }
	}
	
	private static List<DumpWrapper> dumps = new List<DumpWrapper>();
	public static List<DumpWrapper> Dumps { get { return dumps; } }

	public static void Launch()
	{
		dumps.Add(null);
	}

	public static void DeleteSelectedScheme()
	{
		Assert.AssertIsTrue(selection > 0);
		dumps.RemoveAt(selection);
		selection = selection - 1;
	}

	public static EditingGlobalHoldings globalEdit { get;set; }

	public static void PrepareEditingScheme(bool isNew)
	{
		globalEdit = new EditingGlobalHoldings();
		if (isNew)
		{
			globalEdit.isNewScheme = true;
			globalEdit.indexInSchemes = -1;
			globalEdit.editingScheme = CreateEmptyScheme2DR();
		}
		else
		{
			Assert.AssertIsTrue(selection > 0);
			globalEdit.isNewScheme = false;
			globalEdit.indexInSchemes = selection;
			globalEdit.editingScheme = new PlayableScheme(dumps[selection]);
		}
	}

	public static bool IsSchemeLegal(PlayableScheme scheme, out string reason)
	{
		reason = "";
		return false;
	}

	public static void CancelEditingScheme()
	{
		globalEdit = null;
	}

	public static void ConfirmEditingScheme()
	{
		Assert.AssertNotNull(globalEdit);
		if (globalEdit.isNewScheme)
		{
			SchemeIO.WriteToFile(globalEdit.editingScheme);
			dumps.Add(globalEdit.editingScheme.Dump());
		}
		else
		{
			SchemeIO.DeleteSchemeFile(dumps[globalEdit.indexInSchemes].name);
			SchemeIO.WriteToFile(globalEdit.editingScheme);
			dumps[globalEdit.indexInSchemes] = globalEdit.editingScheme.Dump();
		}
		globalEdit = null;
	}

	private static PlayableScheme CreateEmptyScheme2DR()
	{
		var ret = new PlayableScheme();
		ret.Name = "";
		
		ret.canvasConfig = new CanvasConfig2DR();
		var c = ret.canvasConfig as CanvasConfig2DR;
		c.mapWidth = 10;
		c.mapHeight = 10;
		c.insulators = new List<Pos2D>();
		
		ret.slotConfig = new SlotConfig();
		ret.slotConfig.Init(new List<string>(), new List<string>());
		
		ret.matchRules = new List<RuleMatchBasic>();
		ret.extensionRules = new List<RuleMatchExtension>();
		ret.operationRules = new List<RuleOperation>();
		ret.refillRule = new RuleRefill2DR_Downward();
		ret.scoreRules = new List<RuleScore>();
		return ret;
	}	
}

public class EditingGlobalHoldings
{
	public bool isNewScheme = false;
	public PlayableScheme editingScheme;
	public int indexInSchemes = -1;
}

public static class SchemeIO
{
	public static void WriteToFile(PlayableScheme scheme)
	{
	}

	public static void DeleteSchemeFile(string name)
	{
	}

	public static PlayableScheme ReadFromFile(string name)
	{
		throw new NotImplementedException();
	}

	public static List<PlayableScheme> ReadAllSchemes()
	{
		throw new NotImplementedException();
	}
}
