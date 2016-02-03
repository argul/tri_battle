using UnityEngine;
using System;
using System.Collections.Generic;

public class Game2DR_Hardcoded : MonoBehaviour 
{
	public ViewCell prefab;
	private PlayableEnv2DR env;
	public PlayableView2DR view;
	private ViewLayout2DR layout;
	public TapRecognizer tap;
	void Start()
	{
		rand.Seed(0);
		view.cellProvider = (sa, size)=>{
			var ret = GameObject.Instantiate(prefab);
			ret.Init(sa);
			return ret;
		};
		var builder = new PlayableEnvBuilder();
		env = builder.BuildHardcodedEnv();
		env.InitPlayableContainer();
		layout = new ViewLayout2DR(10, 10, env.Foreground.Width, env.Foreground.Height);
		view.Init(env.Foreground, layout);

		tap.OnGesture += OnTap;
	}

	void OnDestroy()
	{
		tap.OnGesture -= OnTap;
	}

	private event Action ticker;
	void Update()
	{
		IsAutoPlay = IsAutoPlayForEditor;
		if (null != ticker)
		{
			ticker.Invoke();
		}
	}

	private bool isPlaying = false;
	private Pos2D selection;
	void OnTap(TapGesture gesture)
	{
		if (IsAutoPlay) return;
		if (isPlaying) return;
		var worldPos = Camera.main.ScreenToWorldPoint(gesture.Position);
		worldPos.z = 0;
		var localPos = transform.InverseTransformPoint(worldPos);
		var logicPos = layout.View2Logic(new Vector2(localPos.x, localPos.y));
		if (logicPos.x < 0 || logicPos.x >= env.Foreground.Width ||
		    logicPos.y < 0 || logicPos.y >= env.Foreground.Height)
		{
			return;
		}

		if (null == selection)
		{
			selection = logicPos;
			view.HighlightCell(selection.x, selection.y, true);
		}
		else if (logicPos != selection)
		{
			view.HighlightCell(selection.x, selection.y, false);
			var tmp = selection;
			selection = null;
			PlayInput(tmp, logicPos);
		}
	}

	void PlayInput(Pos2D a, Pos2D b)
	{
		var input = new OperationInput();
		input.x1 = a.x;
		input.y1 = a.y;
		input.x2 = b.x;
		input.y2 = b.y;
		if (!env.IsLegalOperation(input))
		{
			return;
		}
		var output = env.PerformOperation(input);
		if (!output.IsRejected)
		{
			isPlaying = true;
			view.Play(input, output, ()=>{
				isPlaying = false;
			});
		}
	}

	public enum GizmoContent
	{
		NONE,
		PLM,
		RefillMap,
		LogicSlots,
	}

	public GizmoContent gizmoContent = GizmoContent.NONE;
	void OnDrawGizmos()
	{
		if (null == env || null == env.PlmRecords) return;
		Gizmos.matrix = transform.localToWorldMatrix;
		if (gizmoContent == GizmoContent.PLM)
		{
			foreach (var plm in env.PlmRecords)
			{
				var pos1 = layout.Logic2View(new Pos2D(plm.x1, plm.y1));
				var pos2 = layout.Logic2View(new Pos2D(plm.x2, plm.y2));
				Gizmos.DrawLine(pos1, pos2);
			}
		}
		else if (gizmoContent == GizmoContent.RefillMap)
		{
			object dbg = null;
			if (env.Foreground.UserData.TryGetValue("RuleRefillDownward-RefillInfoMap", out dbg))
			{
				var fillInfos = dbg as RuleRefillDownward.FillInfo[,];
				for (int y = 0; y < fillInfos.GetLength(0); y++)
				{
					for (int x = 0; x < fillInfos.GetLength(1); x++)
					{
						if (null != fillInfos[y, x].ancestorPos)
						{
							Gizmos.DrawLine(layout.Logic2View(new Pos2D(x, y)),
							                layout.Logic2View(new Pos2D(fillInfos[y, x].ancestorPos.x, fillInfos[y, x].ancestorPos.y)));
						}
					}
				}
			}
		}
		else if (gizmoContent == GizmoContent.LogicSlots)
		{
			Gizmos.color = Color.red;
			env.Foreground.ForeachSlot((x, y, slot)=>{
				Gizmos.DrawCube(layout.Logic2View(new Pos2D(x, y)), new Vector3(0.5f, 0.5f, 0.5f));
			});
		}
	}

	Randomizer rand = new Randomizer();
	private bool autoPlay = false;

	public bool IsAutoPlay {
		get { return autoPlay; }
		set {
			if (autoPlay == value) return;
			autoPlay = value;
			if (autoPlay)
			{
				ticker += AutoPlayTicker;
			}
			else
			{
				ticker -= AutoPlayTicker;
			}	
		}
	}
	public bool IsAutoPlayForEditor = false;

	private void AutoPlayTicker()
	{
		if (isPlaying) return;
		var plm = env.PlmRecords[rand.NextInt(0, env.PlmRecords.Count)];
		PlayInput(new Pos2D(plm.x1, plm.y1), new Pos2D(plm.x2, plm.y2));
	}
}
