using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayableView2DR : MonoBehaviour 
{
	public float moveUnitLengthTime = 1f;
	public float eliminationTime = 1f;
	public float shuffleTime = 4f;
	public GameObject goCellRoot;

	public Func<SlotAttribute, Vector2, ViewCell> cellProvider;

	void Update ()
	{
	}

	private ViewLayout2DR layout;
	private ViewCell[,] cells;
	public void Init(Container2D_Rectangular container, ViewLayout2DR layout)
	{
		this.layout = layout;
		cells = new ViewCell[container.Height,container.Width];
		container.ForeachSlot((x, y, s)=>{
			InsertCell(s.slotAttribute, new Pos2D(x, y));
		});
	}

	private void InsertCell(SlotAttribute sa, Pos2D pos)
	{
		if (null != cells[pos.y, pos.x])
		{
			GameObject.Destroy(cells[pos.y, pos.x]);
			cells[pos.y, pos.x] = null;
		}

		var cell = cellProvider.Invoke(sa, layout.CellSize);
		cells[pos.y, pos.x] = cell;
		cell.transform.parent = goCellRoot.transform;
		cell.transform.localPosition = layout.Logic2View(pos);
	}

	private void MoveCell(int fromX, int fromY, int toX, int toY)
	{
		if (null != cells[toY, toX])
		{
			GameObject.Destroy(cells[toY, toX]);
			cells[toY, toX] = null;
		}
		cells[toY, toX] = cells[fromY, fromX];
		cells[fromY, fromX] = null;
		cells[toY, toX].transform.localPosition = layout.Logic2View(new Pos2D(toX, toY));
	}

	public void Play(OperationInput input, OperationOutput output, Action afterFinished)
	{
		StartCoroutine(RecursiveCoroutine.Recursion(PlayCor(input, output, afterFinished)));
	}
	public void HighlightCell(int x, int y, bool isHighlight)
	{
		var cell = cells[y, x];
		if (null == cell) return;
		cell.SetScale(isHighlight ? 1.1f : 1f);
	}

	private IEnumerator PlayCor(OperationInput input, OperationOutput output, Action afterFinished)
	{
		yield return PlayInput(input);

		foreach (var episode in output.episodes)
		{
			switch (episode.etype)
			{
			case OperationOutput.EpisodeType.ELIMINATION:
				yield return PlayEminination(episode);
				break;
			case OperationOutput.EpisodeType.REFILL:
				yield return PlayRefill(episode);
				break;
			case OperationOutput.EpisodeType.SHUFFLE:
				yield return PlayShuffle(episode);
				break;
			default:
				break;
			}
		}

		afterFinished.Invoke();

		yield break;
	}

	private IEnumerator PlayInput(OperationInput input)
	{
		var time = Time.time;
		var list = new List<MoveCellHelper>();

		var move1 = new List<Pos2D>(){ new Pos2D(input.x1, input.y1), new Pos2D(input.x2, input.y2) };
		var move2 = new List<Pos2D>(){ new Pos2D(input.x2, input.y2), new Pos2D(input.x1, input.y1) };
		var a = cells[input.y1, input.x1];
		var b = cells[input.y2, input.x2];

		list.Add(CreateMoveCellHelper(time, a, move1));
		list.Add(CreateMoveCellHelper(time, b, move2));
		yield return PlayMovements(list);

		cells[input.y1, input.x1] = b;
		cells[input.y2, input.x2] = a;
		yield break;
	}

	private IEnumerator PlayEminination(OperationOutput.Episode ep)
	{
		var start = Time.time;
		var end = start + eliminationTime;
		yield return PlayDuration(start, end, (now)=>{
			var alpha = Mathf.Max(0, (end - now) / eliminationTime);
			foreach (var p in ep.elimination)
			{
				cells[p.y, p.x].SetAlpha(alpha);
			}
		});

		foreach (var p in ep.elimination)
		{
			GameObject.Destroy(cells[p.y, p.x].gameObject);
			cells[p.y, p.x] = null;
		}

		yield break;
	}

	private IEnumerator PlayRefill(OperationOutput.Episode ep)
	{
		foreach (var f in ep.refillFlow.NonFillMovements)
		{
			MoveCell(f.src.x, f.src.y, f.dst.x, f.dst.y);
		}
		foreach (var f in ep.refillFlow.FillMovements)
		{
			InsertCell(ep.refillSlots[f.dst.y, f.dst.x], new Pos2D(f.dst.x, f.dst.y));
		}
		foreach (var f in ep.refillFlow.OnSpotList)
		{
			InsertCell(ep.refillSlots[f.y, f.x], new Pos2D(f.x, f.y));
		}

		if (ep.refillFlow.OnSpotList.Count > 0)
		{
			var start = Time.time;
			yield return PlayDuration(start, start + eliminationTime, (now)=>{
				var alpha = Mathf.Min(1, (now - start) / eliminationTime);
				foreach (var p in ep.refillFlow.OnSpotList)
				{
					cells[p.y, p.x].SetAlpha(alpha);
				}
			});
		}

		var moves = new List<MoveCellHelper>();
		foreach (var f in ep.refillFlow.FillMovements)
		{
			moves.Add(CreateMoveCellHelper(Time.time, cells[f.dst.y, f.dst.x], f.movements));
		}
		foreach (var f in ep.refillFlow.NonFillMovements)
		{
			moves.Add(CreateMoveCellHelper(Time.time, cells[f.dst.y, f.dst.x], f.movements));
		}

		yield return PlayMovements(moves);

		yield break;
	}

	private IEnumerator PlayShuffle(OperationOutput.Episode ep)
	{
		var start = Time.time;
		var middle = start + shuffleTime / 2;
		var end = start + shuffleTime;
		yield return PlayDuration(start, middle, (now)=>{
			var gray = Math.Max(0, (middle - now) / (middle - start));
			var color = new Color(gray, gray, gray);
			for (int y = 0; y < ep.refillSlots.GetLength(0); y++)
			{
				for (int x = 0; x < ep.refillSlots.GetLength(1); x++)
				{
					if (ep.refillSlots[y, x].category == SlotAttribute.Category.TARGET)
					{
						cells[y, x].SetColor(color);
					}
				}
			}
		});

		for (int y = 0; y < ep.refillSlots.GetLength(0); y++)
		{
			for (int x = 0; x < ep.refillSlots.GetLength(1); x++)
			{
				if (ep.refillSlots[y, x].category == SlotAttribute.Category.TARGET)
				{
					InsertCell(ep.refillSlots[y, x], new Pos2D(x, y));
				}
			}
		}

		yield return PlayDuration(middle, end, (now)=>{
			var gray = Math.Min(1, (now - middle) / (end - middle));
			var color = new Color(gray, gray, gray);
			for (int y = 0; y < ep.refillSlots.GetLength(0); y++)
			{
				for (int x = 0; x < ep.refillSlots.GetLength(1); x++)
				{
					if (ep.refillSlots[y, x].category == SlotAttribute.Category.TARGET)
					{
						cells[y, x].SetColor(color);
					}
				}
			}
		});

		yield break;
	}

	private IEnumerator PlayDuration(float startTime, float endTime, Action<float> callback)
	{
		bool finished = false;
		while (!finished)
		{
			var now = Time.time;
			callback.Invoke(now);
			finished = now > endTime;
			yield return null;
		}
		yield break;
	}

	private IEnumerator PlayMovements(List<MoveCellHelper> helpers)
	{
		bool finished = false;
		while (!finished)
		{
			finished = true;
			var now = Time.time;
			foreach (var h in helpers)
			{
				var pos = CalculateCurrentPos(h, now);
				h.cellGo.transform.localPosition = new Vector3(pos.x, pos.y, 0);
				if (now < h.endTime)
				{
					finished = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	private Vector3 CalculateCurrentPos(MoveCellHelper h, float time)
	{
		if (time <= h.startTime) return h.trajectory[0];
		if (time >= h.endTime) return h.trajectory[h.trajectory.Count - 1];
		var diff = (time - h.startTime) * (h.trajectory.Count - 1) / moveUnitLengthTime;
		var a = Mathf.FloorToInt(diff);
		var b = diff - a;
		if (a + 1 >= h.trajectory.Count)
		{
			return h.trajectory[h.trajectory.Count - 1];
		}
		else
		{
			return h.trajectory[a] * (1 - b) + h.trajectory[a + 1] * b;
		}
	}

	private MoveCellHelper CreateMoveCellHelper(float startTime, ViewCell go, List<Pos2D> list)
	{
		var ret = new MoveCellHelper();
		ret.startTime = startTime;
		ret.endTime = (list.Count - 1) * moveUnitLengthTime + startTime;
		ret.cellGo = go;
		foreach (var p in list)
		{
			ret.trajectory.Add(layout.Logic2View(p));
		}
		return ret;
	}

	private class MoveCellHelper
	{
		public float startTime;
		public float endTime;
		public ViewCell cellGo;
		public List<Vector2> trajectory = new List<Vector2>();
	}
}
