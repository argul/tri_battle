using System;
using System.Collections.Generic;

public class Container2D_Rectangular : Container
{
	public Container2D_Rectangular(TemplateInfo template, SlotConfig config)
	{
		this.template = template;
		this.factory = new SlotFactory2D_Rectangular(config);
		this.wrapperRect = new SlotWrapper2D[template.height,template.width];
	}
	
	protected TemplateInfo template;
	public class TemplateInfo
	{
		public int width;
		public int height;
		public List<Pos2D> insulators = new List<Pos2D>();
	}

	protected SlotFactory2D_Rectangular factory;

	protected SlotWrapper2D[,] wrapperRect;
	public SlotWrapper2D[,] WrapperRect {
		get {
			return wrapperRect;
		}
	}
	public int Width { get { return template.width; } }
	public int Height { get { return template.height; } }
	protected Dictionary<string, object> userData = new Dictionary<string, object>();
	public Dictionary<string, object> UserData {
		get {
			return userData;
		}
	}
	
	public void InitBlocks()
	{
		foreach (var t in template.insulators)
		{
			wrapperRect[t.y, t.x] = WrapSlot(factory.ProduceInsulator(), t.x, t.y);
		}
	}

	public void FlushAsFirmware()
	{
		ForeachSlot((x, y, slot)=>{
			if (null == slot || slot.slotAttribute.category != SlotAttribute.Category.INSULATOR)
			{
				ClearSlot(x, y);
				wrapperRect[y, x] = WrapSlot(factory.ProduceInsulator(), x, y);
			}
		});
	}

	public void RecreateSubjects(bool ignoreExist)
	{
		for (var y = 0; y < template.height; y++)
		{
			for (var x = 0; x < template.width; x++)
			{
				var exist = wrapperRect[y, x];
				if (null == exist)
				{
					FillSlot(x, y);
				}
				else if (exist.slotAttribute.category != SlotAttribute.Category.INSULATOR
				         && ignoreExist)
				{
					FillSlot(x, y);
				}
			}
		}
	}

	public void ForeachSlot(Action<int, int, SlotWrapper2D> procedure)
	{
		for (var y = 0; y < Height; y++)
		{
			for (var x = 0; x < Width; x++)
			{
				procedure.Invoke(x, y, wrapperRect[y, x]);
			}
		}
	}
	public bool IsLegalPosition(int x, int y)
	{
		if (x < 0 || x >= Width) return false;
		if (y < 0 || y >= Height) return false;
		return true;
	}
	public SlotWrapper2D GetSlot(int x, int y)
	{
		return wrapperRect[y, x];
	}
	public void ClearSlot(int x, int y)
	{
		wrapperRect[y, x] = null;
	}
	public void FillSlot(int x, int y)
	{
		wrapperRect[y, x] = WrapSlot(factory.ProduceSubject(), x, y);
	}
	public void SetSlot(int x, int y, SlotWrapper2D slot)
	{
		ClearSlot(x, y);
		wrapperRect[y, x] = slot;
		slot.pos.x = x;
		slot.pos.y = y;
	}
	public void SwapSlot(int x1, int y1, int x2, int y2)
	{
		var s1 = wrapperRect[y1, x1];
		var s2 = wrapperRect[y2, x2];
		wrapperRect[y2, x2] = s1;
		wrapperRect[y1, x1] = s2;
		if (null != s1)
		{
			s1.pos.x = x2;
			s1.pos.y = y2;
		}
		if (null != s2)
		{
			s2.pos.x = x1;
			s2.pos.y = y1;
		}
	}
	public void CheckIsFull()
	{
		ForeachSlot((x, y, slot)=>{
			if (null == slot){
				throw new Exception();
			}
		});
	}

	private SlotWrapper2D WrapSlot(SlotAttribute s, int x, int y)
	{
		var ret = new SlotWrapper2D();
		ret.slotAttribute = s;
		ret.pos.x = x;
		ret.pos.y = y;
		return ret;
	}
}