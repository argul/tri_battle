using UnityEngine;
using System.Collections.Generic;

public class EditTraits2DR : MonoBehaviour 
{
	public System.Action<List<Color>> hdlTraitSelectionChanged;
	private Color[] traitColors = new Color[8]
	{
		Color.red, 
		Color.green, 
		Color.blue, 
		Color.yellow, 
		Color.cyan, 
		Color.magenta,
		new Color(1, 100/255f, 0),
		new Color(160/255f, 0, 1)
	};
	private ViewCell[] cells = new ViewCell[8];
	public GameObject cellRoot;
	public ViewCell pfbCell;
	public void LoadData(List<SlotTrait> colorTraits)
	{
		layout = new ViewLayout2DR(8f, 1f, 8, 1);
		UnityExtension.DestroyAllChildren(cellRoot);
		for (int i = 0; i < traitColors.Length; i++)
		{
			var cell = GameObject.Instantiate(pfbCell);
			var sa = new SlotAttribute(SlotAttribute.Category.TARGET);
			sa.trait = new SlotTraitColor((byte)(traitColors[i].r * 255),
			                              (byte)(traitColors[i].g * 255),
			                              (byte)(traitColors[i].b * 255),
			                              255);
			cell.Init(sa, layout.CellSize);
			cell.transform.SetParent(cellRoot.transform, layout.Logic2View(new Pos2D(i, 0)), cell.transform.localScale);
			cells[i] =cell;

			foreach (var ct in colorTraits)
			{
				if (sa.trait.AbsoluteEqual(ct))
				{
					cell.SetSelected(true);
				}
			}
		}
	}

	public TapRecognizer tap;

	void OnEnable()
	{
		Invoke("RegisterTap", 0.01f);
	}
	
	void RegisterTap()
	{
		tap.OnGesture += OnTap;
	}
	
	void OnDisable()
	{
		tap.OnGesture -= OnTap;
	}

	private ViewLayout2DR layout;
	private void OnTap(TapGesture gesture)
	{
		var worldPos = Camera.main.ScreenToWorldPoint(gesture.Position);
		worldPos.z = 0;
		var localPos = transform.InverseTransformPoint(worldPos);
		var logicPos = layout.View2Logic(new Vector2(localPos.x, localPos.y));
		if (logicPos.x < 0 || logicPos.x >= 8) return;
		if (logicPos.y != 0) return;

		var cell = cells[logicPos.x];
		cell.SetSelected(!cell.IsSelected);

		var selection = new List<Color>();
		for (int i = 0; i < traitColors.Length; i++)
		{
			if (cells[i].IsSelected)
			{
				selection.Add(traitColors[i]);
			}
		}
		hdlTraitSelectionChanged.Invoke(selection);
	}
}
