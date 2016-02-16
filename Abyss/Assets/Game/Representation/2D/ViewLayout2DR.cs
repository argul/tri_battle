using UnityEngine;
using System;
using System.Collections.Generic;

public class ViewLayout2DR
{
	private float startX;
	private float startY;
	private float cellWidth;
	private float cellHalfWidth;
	private float cellHeight;
	private float cellHalfHeight;
	public Vector2 CellSize { get { return new Vector2(cellWidth, cellHeight); } }

	public ViewLayout2DR(float viewWidth, float viewHeight, int logicWidth, int logicHeight)
	{
		startX = -viewWidth/2;
		startY = -viewHeight/2;
		cellWidth = viewWidth / logicWidth;
		cellHeight = viewHeight / logicHeight;
		cellHalfWidth = cellWidth / 2;
		cellHalfHeight = cellHeight / 2;
	}

	public Vector2 Logic2View(Pos2D pos)
	{
		return new Vector2(startX + cellHalfWidth + pos.x * cellWidth,
		                   startY + cellHalfHeight + pos.y * cellHeight);
	}

	public Pos2D View2Logic(Vector2 pos)
	{
		return new Pos2D(Mathf.FloorToInt((pos.x - startX) / cellWidth),
		                 Mathf.FloorToInt((pos.y - startY) / cellHeight));
	}
}
