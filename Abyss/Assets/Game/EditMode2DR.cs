using UnityEngine;
using System.Collections;

public class EditMode2DR : MonoBehaviour 
{
	public EditModeUI ui;
	public EditCanvas2DR editCanvas;
	public EditOperationRule2DR editOperation;
	public EditTraits2DR editTraits;
	public EditMatchRule2DR editMatch;

	void Start()
	{
		editTraits.hdlTraitSelectionChanged += (colors)=>{
			var slotConfig = Game.globalEdit.editingScheme.slotConfig;
			slotConfig.Traits.Clear();
			slotConfig.Traits.AddRange(colors.SchemeStyleMap<Color, SlotTrait>((c)=>{
				return new SlotTraitColor((byte)(c.r * 255),
				                          (byte)(c.g * 255),
				                          (byte)(c.b * 255),
				                          255);
			}));
		};
	}

	public void StartEditCanvas()
	{
		UnityExtension.SetActive(true, editCanvas.gameObject);
		UnityExtension.SetActiveBatch(false, editTraits.gameObject, editMatch.gameObject, editOperation.gameObject);

		ResetCanvas();
	}

	public void StartEditMatchRules()
	{
		UnityExtension.SetActive(true, editMatch.gameObject);
		UnityExtension.SetActiveBatch(false, editTraits.gameObject, editCanvas.gameObject, editOperation.gameObject);

		matchRuleIndex = 0;
		SyncMatchRuleState();
	}

	public void StartEditOperationRules()
	{
		UnityExtension.SetActive(true, editOperation.gameObject);
		UnityExtension.SetActiveBatch(false, editTraits.gameObject, editMatch.gameObject, editCanvas.gameObject);

		operationRuleIndex = 0;
		SyncOperationRuleState();
	}

	public void StartEditTraits()
	{
		UnityExtension.SetActive(true, editTraits.gameObject);
		UnityExtension.SetActiveBatch(false, editCanvas.gameObject, editMatch.gameObject, editOperation.gameObject);
		editTraits.LoadData(Game.globalEdit.editingScheme.slotConfig.Traits);
	}

	#region Canvas
	public const int MIN_CANVAS_SIZE = 4;
	public const int MAX_CANVAS_SIZE = 20;
	public void ModifyCanvasSize(int dx, int dy)
	{
		var width = editCanvas.Width + dx;
		var height = editCanvas.Height + dy;
		if (width < MIN_CANVAS_SIZE || 
		    width > MAX_CANVAS_SIZE || 
		    height < MIN_CANVAS_SIZE || 
		    height > MAX_CANVAS_SIZE)
		{
			return;
		}
		else
		{
			editCanvas.Reset(width, height);
			ui.txtCanvasWidth.text = width.ToString();
			ui.txtCanvasHeight.text = height.ToString();
		}
	}

	private void ResetCanvas()
	{
		var canvasCfg = Game.globalEdit.editingScheme.canvasConfig as CanvasConfig2DR;
		editCanvas.LoadData(canvasCfg);
		ui.txtCanvasWidth.text = canvasCfg.mapWidth.ToString();
		ui.txtCanvasHeight.text = canvasCfg.mapHeight.ToString();
	}

	public void ApplyCanvas()
	{
		Game.globalEdit.editingScheme.canvasConfig = editCanvas.SerializeData();
		ResetCanvas();
	}

	public void RevertCanvas()
	{
		ResetCanvas();
	}
	#endregion

	#region match rule
	private int matchRuleIndex = 0;
	public bool HasPrevMatchRule { get { return matchRuleIndex > 0; } }
	public bool HasNextMatchRule { get { return (matchRuleIndex + 1) < Game.globalEdit.editingScheme.matchRules.Count; } }
	public bool HasNewMatchRule { get { return  (Game.globalEdit.editingScheme.matchRules.Count <= 0) || ((matchRuleIndex + 1) == Game.globalEdit.editingScheme.matchRules.Count); } }
	public void NextMatchRule()
	{
		Assert.AssertIsTrue(HasNextMatchRule);
		++matchRuleIndex;
		var rule = Game.globalEdit.editingScheme.matchRules[matchRuleIndex];
		editMatch.LoadData(rule as RuleMatchBasic2D_Rectangular);
	}

	public void PrevMatchRule()
	{
		Assert.AssertIsTrue(HasPrevMatchRule);
		--matchRuleIndex;
		var rule = Game.globalEdit.editingScheme.matchRules[matchRuleIndex];
		SyncMatchRuleState();
		editMatch.LoadData(rule as RuleMatchBasic2D_Rectangular);
	}

	public void NewMatchRule()
	{
		Assert.AssertIsTrue(HasNewMatchRule);
		editMatch.LoadDefault();
		Game.globalEdit.editingScheme.matchRules.Add(editMatch.SerializeData());
		matchRuleIndex = Game.globalEdit.editingScheme.matchRules.Count - 1;
	}

	public void SaveMatchRule()
	{
		var rules = Game.globalEdit.editingScheme.matchRules;
		if (rules.Count <= 0) return;
		rules[matchRuleIndex] = editMatch.SerializeData();
	}

	public void DeleteMatchRule()
	{
		var rules = Game.globalEdit.editingScheme.matchRules;
		if (rules.Count <= 0) return;
		rules.RemoveAt(matchRuleIndex);
		matchRuleIndex = System.Math.Min(matchRuleIndex, rules.Count - 1);
		SyncMatchRuleState();
	}

	private void SyncMatchRuleState()
	{
		var rules = Game.globalEdit.editingScheme.matchRules;
		if (rules.Count <= 0)
		{
			editMatch.LoadEmpty();
		}
		else
		{
			editMatch.LoadData(rules[matchRuleIndex] as RuleMatchBasic2D_Rectangular);
		}
	}
	#endregion

	#region operation rule
	private int operationRuleIndex = 0;
	public bool HasPrevOperationRule { get { return operationRuleIndex > 0; } }
	public bool HasNextOperationRule { get { return (operationRuleIndex + 1) < Game.globalEdit.editingScheme.operationRules.Count; } }
	public bool HasNewOperationRule { get { return  (Game.globalEdit.editingScheme.operationRules.Count <= 0) || ((operationRuleIndex + 1) == Game.globalEdit.editingScheme.operationRules.Count); } }
	public void NextOperationRule()
	{
		Assert.AssertIsTrue(HasNextOperationRule);
		++operationRuleIndex;
		var rule = Game.globalEdit.editingScheme.operationRules[operationRuleIndex];
		editOperation.LoadData(rule as RuleOperation2D_Rectangular);
	}
	
	public void PrevOperationRule()
	{
		Assert.AssertIsTrue(HasPrevOperationRule);
		--operationRuleIndex;
		var rule = Game.globalEdit.editingScheme.operationRules[operationRuleIndex];
		SyncOperationRuleState();
		editOperation.LoadData(rule as RuleOperation2D_Rectangular);
	}
	
	public void NewOperationRule()
	{
		Assert.AssertIsTrue(HasNewOperationRule);
		editOperation.LoadDefault();
		Game.globalEdit.editingScheme.operationRules.Add(new RuleOperation2D_Rectangular());
		operationRuleIndex = Game.globalEdit.editingScheme.operationRules.Count - 1;
	}
	
	public void SaveOperationRule()
	{
		var rules = Game.globalEdit.editingScheme.operationRules;
		if (rules.Count <= 0) return;
		if (!editOperation.IsLegal)
		{
			ui.ShowHintText("Illegal operation rule!");
			return;
		}
		rules[operationRuleIndex] = editOperation.SerializeData();
	}
	
	public void DeleteOperationRule()
	{
		var rules = Game.globalEdit.editingScheme.operationRules;
		if (rules.Count <= 0) return;
		rules.RemoveAt(operationRuleIndex);
		operationRuleIndex = System.Math.Min(operationRuleIndex, rules.Count - 1);
		SyncMatchRuleState();
	}
	
	private void SyncOperationRuleState()
	{
		var rules = Game.globalEdit.editingScheme.operationRules;
		if (rules.Count <= 0)
		{
			editOperation.LoadEmpty();
		}
		else
		{
			editOperation.LoadData(rules[operationRuleIndex] as RuleOperation2D_Rectangular);
		}
	}
	#endregion
}
