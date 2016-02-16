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

	public static T[] ConvertArray<T>(T[,] arr)
	{
		var ret = new T[arr.GetLength(0) * arr.GetLength(1)];
		for (int i = 0; i < arr.GetLength(0); i++)
		{
			for (int j = 0; j < arr.GetLength(1); j++)
			{
				ret[i * arr.GetLength(1) + j] = arr[i, j];
			}
		}
		return ret;
	}

	public static T[,] InverseConvertArray<T>(T[] arr, int dimension0, int dimension1)
	{
		Assert.AssertIsTrue(dimension0 > 1 && dimension1 > 1);
		Assert.AssertIsTrue(arr.Length == dimension0 * dimension1);
		var ret = new T[dimension0, dimension1];
		for (int i = 0; i < dimension0; i++)
		{
			for (int j = 0; j < dimension1; j++)
			{
				ret[i, j] = arr[i * dimension1 + j];
			}
		}
		return ret;
	}
}
