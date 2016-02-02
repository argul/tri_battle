using System;

public abstract class SlotSpecialty
{
	public const string TRIGGER_STAGE_ELIMINATION = "elimination";
	public abstract string Identifer { get; }
	public virtual void OnSpawn() {}
	public virtual void OnRecycle() {}

	public abstract void Trigger(string stage, Container container, object param);
}
