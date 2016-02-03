using System;

public abstract class Rule : ISerializable
{
	public abstract string Serialize();
	public abstract void Deserialize(string str);
}