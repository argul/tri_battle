using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
		dumps.AddRange(SchemeIO.ReadAllSchemes());
	}

	public static void DeleteSelectedScheme()
	{
		Assert.AssertIsTrue(selection > 0);
		SchemeIO.DeleteSchemeFile(dumps[selection].name);
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

	public static void CancelEditingScheme()
	{
		globalEdit = null;
	}

	public static void ConfirmEditingScheme()
	{
		Assert.AssertNotNull(globalEdit);
		if (globalEdit.isNewScheme)
		{
			var d = globalEdit.editingScheme.Dump();
			SchemeIO.WriteToFile(d);
			dumps.Add(d);
		}
		else
		{
			SchemeIO.DeleteSchemeFile(dumps[globalEdit.indexInSchemes].name);
			var d = globalEdit.editingScheme.Dump();
			SchemeIO.WriteToFile(d);
			dumps[globalEdit.indexInSchemes] = d;
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
	private const string SCHEME_EXTENSION = ".scheme";
	private static string RootPath { get { return Application.persistentDataPath; } }
	private static string GetPath(string name)
	{
		return Path.Combine(RootPath, name + SCHEME_EXTENSION);
	}

	public static void WriteToFile(DumpWrapper schemeDump)
	{
		var path = GetPath(schemeDump.name);
		File.WriteAllText(path, JsonHelper.Serialize(schemeDump));
	}

	public static void DeleteSchemeFile(string name)
	{
		var path = GetPath(name);
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	public static DumpWrapper ReadFromFile(string name)
	{
		var path = GetPath(name);
		var str = File.ReadAllText(path);
		return JsonHelper.Deserialize<DumpWrapper>(str);
	}

	public static List<DumpWrapper> ReadAllSchemes()
	{
		var ret = new List<DumpWrapper>();
		var di = new DirectoryInfo(RootPath);
		foreach (var fi in di.GetFiles())
		{
			if (fi.Extension != SCHEME_EXTENSION) continue;
			ret.Add(JsonHelper.Deserialize<DumpWrapper>(File.ReadAllText(fi.FullName)));
		}
		return ret;
	}
}
