public class Pos2D
{
	public Pos2D() {}
	public Pos2D(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	public static bool operator==(Pos2D lhr, Pos2D rhr)
	{
		if (object.ReferenceEquals(lhr, null)) return object.ReferenceEquals(rhr, null);
		if (object.ReferenceEquals(rhr, null)) return object.ReferenceEquals(lhr, null);
		return lhr.x == rhr.x && lhr.y == rhr.y;
	}
	public static bool operator!=(Pos2D lhr, Pos2D rhr)
	{
		return !(lhr == rhr);
	}
	public override bool Equals (object other)
	{
		var p = other as Pos2D;
		if (null == p) return false;
		return this == p;
	}
	public override int GetHashCode ()
	{
		return (y << 16) + x;
	}
	public Pos2D Clone()
	{
		return new Pos2D(x, y);
	}
	public int x;
	public int y;
}
