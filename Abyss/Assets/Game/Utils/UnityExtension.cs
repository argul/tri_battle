using UnityEngine;
using System;
using System.Collections.Generic;

public static class UnityExtension
{
	public static void DestroyAllChildren(this Transform tr)
	{
		int len = tr.childCount;
		for (int i = len - 1; i >= 0; i--)
		{
			GameObject.Destroy(tr.GetChild(i).gameObject);
		}
	}

	public static void DestroyAllChildren(this GameObject go)
	{
		DestroyAllChildren(go.transform);
	}

	public static void SetParent(this Transform child, Transform parent,
	                             Vector3 localPosition,
	                             Vector3 localScale)
	{
		child.SetParent(parent);
		child.localPosition = localPosition;
		child.localScale = localScale;
	}

	public static void SetActive(bool active, GameObject go)
	{
		if (go.activeSelf != active)
			go.SetActive(active);
	}

	public static void SetActiveBatch(bool active, params GameObject[] gos)
	{
		foreach (var go in gos)
		{
			SetActive(active, go);
		}
	}
}
