using System;

public class SlotAdjacencyInfo2D_Rectangular : SlotAdjacencyInfo
{
	public SlotAdjacencyInfo2D_Rectangular(Direction direction) { this.direction = direction; }
	public enum Direction
	{
		ERROR,
		UP,
		LEFT,
		RIGHT,
		DOWN,
		UP_LEFT,
		UP_RIGHT,
		DOWN_LEFT,
		DOWN_RIGHT
	}
	public Direction direction = Direction.ERROR;
}