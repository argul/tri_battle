using UnityEngine;
using System.Collections;

public class EditCanvas2DR : MonoBehaviour 
{
	public float viewWidth = 10f;
	public float viewHeight = 10f;
	public GameObject cellRoot;
	public GameObject lineRoot;
	public Line pfbLine;
	public ViewCell pfbCell;
	public TapRecognizer tap;

	private bool[,] canvas;
	private GameObject[,] blocks;
	private ViewLayout2DR layout;

	public int Width { get { return canvas.GetLength(1); } }
	public int Height { get { return canvas.GetLength(0); } }

	public CanvasConfig2DR SerializeData()
	{
		var ret = new CanvasConfig2DR();
		ret.mapWidth = canvas.GetLength(1);
		ret.mapHeight = canvas.GetLength(0);
		ret.insulators = new System.Collections.Generic.List<Pos2D>();
		for (int y = 0; y < ret.mapHeight; y++)
		{
			for (int x = 0; x < ret.mapWidth; x++)
			{
				if (canvas[y, x])
				{
					ret.insulators.Add(new Pos2D(x, y));
				}
			}
		}
		return ret;
	}

	public void LoadData(CanvasConfig2DR cc)
	{
		Reset(cc.mapWidth, cc.mapHeight);
		foreach (var b in cc.insulators)
		{
			Mark(b.x, b.y);
		}
	}
	public void Reset(int width, int height)
	{
		canvas = new bool[height, width];
		blocks = new GameObject[height, width];
		cellRoot.DestroyAllChildren();
		lineRoot.DestroyAllChildren();
		layout = new ViewLayout2DR(viewWidth, viewHeight, width, height);
		var halfCellSize = layout.CellSize / 2;
		for (int i = 0; i <= width; i++)
		{
			DrawVerticalLine(layout.Logic2View(new Pos2D(i, 0)).x - halfCellSize.x);
		}
		for (int j = 0; j <= height; j++)
		{
			DrawHorizontalLine(layout.Logic2View(new Pos2D(0, j)).y - halfCellSize.y);
		}
	}

	public void Mark(int x, int y)
	{
		if (canvas[y, x])
		{
			GameObject.Destroy(blocks[y, x]);
			blocks[y, x] = null;
			canvas[y, x] = false;
		}
		else
		{
			var c = DrawBlock(layout.Logic2View(new Pos2D(x, y)));
			blocks[y, x] = c;
			canvas[y, x] = true;
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

	private void OnTap(TapGesture gesture)
	{
		var worldPos = Camera.main.ScreenToWorldPoint(gesture.Position);
		worldPos.z = 0;
		var localPos = transform.InverseTransformPoint(worldPos);
		var logicPos = layout.View2Logic(new Vector2(localPos.x, localPos.y));
		if (logicPos.x < 0 || logicPos.x >= canvas.GetLength(1)) return;
		if (logicPos.y < 0 || logicPos.y >= canvas.GetLength(0)) return;
		Mark(logicPos.x, logicPos.y);
	}
}
