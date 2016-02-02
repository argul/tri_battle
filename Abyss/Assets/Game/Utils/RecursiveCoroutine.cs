using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Task will exec as a coroutine
/// </summary>

public static class RecursiveCoroutine
{	
	public static IEnumerator Recursion(IEnumerator main)
	{
		while (TryMoveEnumerator(main))
		{
			object rsl = main.Current;
			if (typeof(IEnumerator).IsInstanceOfType(rsl))
			{
				IEnumerator sub = Recursion(rsl as IEnumerator);
				while (TryMoveEnumerator(sub))
				{
					yield return sub.Current;
				}
			}
			else
			{
				yield return rsl;
			}
		}
		
		yield break;
	}
	
	private static bool TryMoveEnumerator(IEnumerator iter)
	{
		bool ret = false;
		try
		{
			ret = iter.MoveNext();
			return ret;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			return false;
		}
	}
}