using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayMode2DR : MonoBehaviour 
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
			ret.Init(sa, size);
			return ret;
		};

		var builder = new PlayableEnvBuilder();
		if (Game.IsClassicScheme)
		{
			env = builder.Build_2DR_Hardcoded();
		}
		else
		{
			env = builder.Build_2DR(new PlayableScheme(Game.Dumps[Game.Selection]));
		}

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
	public int RefillMapStartX = -1;
	public int RefillMapStartY = -1;
	void OnDrawGizmos()
	{
		if (null == env || null == env.PlmRecords) return;
		Gizmos.matrix = transform.localToWorldMatrix;
		if (gizmoContent == GizmoContent.PLM)
		{
			if (!isPlaying)
			{
				foreach (var plm in env.PlmRecords)
				{
					var pos1 = layout.Logic2View(new Pos2D(plm.x1, plm.y1));
					var pos2 = layout.Logic2View(new Pos2D(plm.x2, plm.y2));
					Gizmos.DrawLine(pos1, pos2);
				}
			}
		}
		else if (gizmoContent == GizmoContent.RefillMap)
		{
			if (null != GlobalDebug.refillInfoMap)
			{
				var width = GlobalDebug.refillInfoMap.GetLength(1);
				var height = GlobalDebug.refillInfoMap.GetLength(0);
				if (RefillMapStartX >= 0 
				    && RefillMapStartX < width
				    && RefillMapStartY >= 0
				    && RefillMapStartY < height)
				{
					var curX = RefillMapStartX;
					var curY = RefillMapStartY;
					while (curX >= 0 && curX < width && curY >= 0 && curY < height
					       && null != GlobalDebug.refillInfoMap[curY, curX]
					       && null != GlobalDebug.refillInfoMap[curY, curX].ancestorPos)
					{
						var ancestorPos = GlobalDebug.refillInfoMap[curY, curX].ancestorPos;
						Gizmos.DrawLine(layout.Logic2View(new Pos2D(curX, curY)),
						                layout.Logic2View(new Pos2D(ancestorPos.x, ancestorPos.y)));
						curX = ancestorPos.x;
						curY = ancestorPos.y;
					}
				}
				else
				{
					for (int y = 0; y < GlobalDebug.refillInfoMap.GetLength(0); y++)
					{
						for (int x = 0; x < GlobalDebug.refillInfoMap.GetLength(1); x++)
						{
							if (null == GlobalDebug.refillInfoMap[y, x]
							    || null == GlobalDebug.refillInfoMap[y, x].ancestorPos)
							{
								continue;
							}
							Gizmos.DrawLine(layout.Logic2View(new Pos2D(x, y)),
							                layout.Logic2View(new Pos2D(GlobalDebug.refillInfoMap[y, x].ancestorPos.x, GlobalDebug.refillInfoMap[y, x].ancestorPos.y)));
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

		Gizmos.color = Color.black;
		env.Foreground.ForeachSlot((x, y, s)=>{

		});
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

	private void AutoPlayTicker()
	{
		if (isPlaying) return;
		var plm = env.PlmRecords[rand.NextInt(0, env.PlmRecords.Count)];
		PlayInput(new Pos2D(plm.x1, plm.y1), new Pos2D(plm.x2, plm.y2));
	}
}
