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
	}

	public void StartEditOperationRules()
	{
		UnityExtension.SetActive(true, editOperation.gameObject);
		UnityExtension.SetActiveBatch(false, editTraits.gameObject, editMatch.gameObject, editCanvas.gameObject);
	}

	public void StartEditTraits()
	{
		UnityExtension.SetActive(true, editTraits.gameObject);
		UnityExtension.SetActiveBatch(false, editCanvas.gameObject, editMatch.gameObject, editOperation.gameObject);
		editTraits.LoadData(Game.globalEdit.editingScheme.slotConfig.Traits);
	}

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
}
