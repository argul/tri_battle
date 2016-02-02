using System;

public class Tuple<T1, T2>
{
	public Tuple()
	{
		item1 = default(T1);
		item2 = default(T2);
	}
	public Tuple(T1 item1, T2 item2)
	{
		this.item1 = item1;
		this.item2 = item2;
	}
	public T1 item1;
	public T2 item2;
}

public class Tuple<T1, T2, T3>
{
	public Tuple()
	{
		item1 = default(T1);
		item2 = default(T2);
		item3 = default(T3);
	}
	public Tuple(T1 item1, T2 item2, T3 item3)
	{
		this.item1 = item1;
		this.item2 = item2;
		this.item3 = item3;
	}
	public T1 item1;
	public T2 item2;
	public T3 item3;
}

public class Tuple<T1, T2, T3, T4>
{
	public Tuple()
	{
		item1 = default(T1);
		item2 = default(T2);
		item3 = default(T3);
		item4 = default(T4);
	}
	public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
	{
		this.item1 = item1;
		this.item2 = item2;
		this.item3 = item3;
		this.item4 = item4;
	}
	public T1 item1;
	public T2 item2;
	public T3 item3;
	public T4 item4;
}