using System;
using System.Collections.Generic;

public abstract class CanvasConfig : ISerializable
{
	public abstract string SerializeUID { get; }
	public abstract string Serialize ();
	public abstract void Deserialize (string str);
	protected const string RECTANGULAR_2D = "Rectangular_2D";
	public static string StaticSerialize(CanvasConfig c)
	{
		return JsonHelper.Serialize(new Tuple<string, string>(c.SerializeUID, c.Serialize()));
	}
	public static CanvasConfig StaticDeserialize(string str)
	{
		var tuple = JsonHelper.Deserialize<Tuple<string, string>>(str);
		switch (tuple.item1)
		{
		case RECTANGULAR_2D:
			var ret = new CanvasConfig2DR();
			ret.Deserialize(tuple.item2);
			return ret;
		default:
			throw new NotSupportedException();
		}
	}
}

public class CanvasConfig2DR : CanvasConfig
{
	public int mapWidth;
	public int mapHeight;
	public List<Pos2D> insulators;
	public override string SerializeUID { get { return RECTANGULAR_2D; } }
	public override string Serialize ()
	{
		return JsonHelper.Serialize(this);
	}
	public override void Deserialize (string str)
	{
		var obj = JsonHelper.Deserialize<CanvasConfig2DR>(str);
		mapWidth = obj.mapWidth;
		mapHeight = obj.mapHeight;
		insulators = obj.insulators;
	}
}