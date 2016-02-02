using System;
using System.Collections.Generic;
using System.Text;

public class SlotAttribute : ISerializable, IPoolUser
{
	public SlotAttribute(Category category)
	{
		this.category = category;
	}
	public enum Category
	{
		ERROR = -1,
		INSULATOR,
		TARGET,
		SPECIAL
	}
	public Category category = Category.ERROR;
	public SlotTrait trait;
	public SlotSpecialty specialty;
	public SlotAdjacencyInfo[] adjacencies;

	public string Token {
		get {
			var sb = new StringBuilder();
			sb.Append(category.ToString());
			if (null != trait)
			{
				sb.Append('-');
				sb.Append(trait.Identifer);
			}
			if (null != specialty)
			{
				sb.Append('-');
				sb.Append(specialty.Identifer);
			}
			var ret = sb.ToString();
			sb.Length = 0;
			return ret;
		}
	}

	public void OnSpawn() {}
	public void OnRecycle() {}
}