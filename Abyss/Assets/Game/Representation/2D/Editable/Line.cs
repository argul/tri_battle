using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour
{
	public float thickness = 0.05f;
	public SpriteRenderer render;

	public void SetToHorizontal(float length)
	{
		transform.localScale = new Vector3(length, thickness, 1f);
	}

	public void SetToVertical(float length)
	{
		transform.localScale = new Vector3(thickness, length, 1f);
	}
}
