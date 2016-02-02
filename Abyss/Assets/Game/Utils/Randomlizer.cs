using System;
using UnityEngine;

public class Randomizer
{
	public void Seed(int seed)
	{
		UnityEngine.Random.seed = seed;
	}
	public int NextInt()
	{
		return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
	}
	public int NextInt(int min, int max)
	{
		return UnityEngine.Random.Range(min, max);
	}
}