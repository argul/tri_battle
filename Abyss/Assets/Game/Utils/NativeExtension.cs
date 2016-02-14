using System;
using System.Collections.Generic;

public static class NativeExtension
{
	public static List<T> SchemeStyleMap<K, T>(this List<K> raw, Func<K, T> proc)
	{
		var ret = new List<T>();
		raw.ForEach((k)=>{
			ret.Add(proc.Invoke(k));
		});
		return ret;
	}

	public static T[] SchemeStyleMap<K, T>(this K[] raw, Func<K, T> proc)
	{
		var len = raw.Length;
		var ret = new T[len];
		for (int i = 0; i < len; i++)
		{
			ret[i] = proc.Invoke(raw[i]);
		}
		return ret;
	}	
}
