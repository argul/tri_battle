using System;

public abstract class SlotSpecialty : ISerializable
{
	public abstract string Name { get; }
	public const string TRIGGER_STAGE_ELIMINATION = "elimination";
	public abstract string Identifer { get; }
	public virtual void OnSpawn() {}
	public virtual void OnRecycle() {}

	public abstract void Trigger(string stage, Container container, object param);

	public abstract string Serialize();
	public abstract void Deserialize(string str);
	public static string StaticSerialize(SlotSpecialty s)
	{
		throw new NotImplementedException();
	}

	public static SlotSpecialty StaticDeserialize(string str)
	{
		throw new NotImplementedException();
	}
}
