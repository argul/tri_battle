using UnityEngine;
using System.Collections;

public class EditContainer2DR : MonoBehaviour 
{
	public GameObject cellRoot;
	public GameObject lineRoot;
	public Line pfbLine;
	public ViewCell pfbCell;
	protected bool[,] selection;
	protected GameObject[,] cells;
	public TapRecognizer tap;
	
	private float viewWidth;
	private float viewHeight;
	private int logicWidth;
	private int logicHeight;

	public void Draw(float viewWidth, float viewHeight, int logicWidth, int logicHeight)
	{
		this.viewWidth = viewWidth;
		this.viewHeight = viewHeight;
		this.logicWidth = logicWidth;
		this.logicHeight = logicHeight;

		layout = new ViewLayout2DR(viewWidth, viewHeight, logicWidth, logicHeight);
		selection = new bool[logicHeight, logicWidth];
		cells = new GameObject[logicHeight, logicWidth];
		cellRoot.DestroyAllChildren();
		lineRoot.DestroyAllChildren();
		var halfCellSize = layout.CellSize / 2;
		for (int i = 0; i <= logicWidth; i++)
		{
			DrawVerticalLine(layout.Logic2View(new Pos2D(i, 0)).x - halfCellSize.x);
		}
		for (int j = 0; j <= logicHeight; j++)
		{
			DrawHorizontalLine(layout.Logic2View(new Pos2D(0, j)).y - halfCellSize.y);
		}
	}

	private void DrawHorizontalLine(float atY)
	{
		var line = GameObject.Instantiate(pfbLine);
		line.SetToHorizontal(viewWidth);
		line.transform.SetParent(lineRoot.transform, new Vector3(0, atY, 0), line.transform.localScale);
	}
	
	private void DrawVerticalLine(float atX)
	{
		var line = GameObject.Instantiate(pfbLine);
		line.SetToVertical(viewHeight);
		line.transform.SetParent(lineRoot.transform, new Vector3(atX, 0, 0), line.transform.localScale);
	}
	
	private GameObject DrawBlock(Vector2 at)
	{
		var cell = GameObject.Instantiate(pfbCell);
		cell.Init(new SlotAttribute(SlotAttribute.Category.INSULATOR), layout.CellSize);
		cell.transform.SetParent(cellRoot.transform, new Vector3(at.x, at.y, 0), cell.transform.localScale);
		return cell.gameObject;
	}

	protected void Mark(int x, int y)
	{
		if (selection[y, x])
		{
			GameObject.Destroy(cells[y, x]);
			cells[y, x] = null;
			selection[y, x] = false;
		}
		else
		{
			var c = DrawBlock(layout.Logic2View(new Pos2D(x, y)));
			cells[y, x] = c;
			selection[y, x] = true;
		}
	}

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
		if (null == layout) return;
		var worldPos = Camera.main.ScreenToWorldPoint(gesture.Position);
		worldPos.z = 0;
		var localPos = transform.InverseTransformPoint(worldPos);
		var logicPos = layout.View2Logic(new Vector2(localPos.x, localPos.y));
		if (logicPos.x < 0 || logicPos.x >= logicWidth) return;
		if (logicPos.y < 0 || logicPos.y >= logicHeight) return;
		
		OnSelection(logicPos.x, logicPos.y);
	}

	protected virtual void OnSelection(int logicX, int logicY)
	{
		Mark(logicX, logicY);
	}
}
