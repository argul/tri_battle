using UnityEngine;
using System.Collections;

public class ViewCell : MonoBehaviour 
{
	public SpriteRenderer render;
	public void SetAlpha(float alpha)
	{
		render.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
	}

	private Color origColor;
	private Vector2 origSize;
	public void Init(SlotAttribute sa, Vector2 size)
	{
		if (sa.category == SlotAttribute.Category.TARGET)
		{
			var sc = sa.trait as SlotTraitColor;
			origColor = new Color(sc.r / 255f, sc.g / 255f, sc.b / 255f, sc.a / 255f);
			render.color = origColor;
		}
		else if (sa.category == SlotAttribute.Category.INSULATOR)
		{

		}
		else
		{
			throw new System.NotImplementedException();
		}
		origSize = size;
		transform.localScale = new Vector3(origSize.x, origSize.y, 1f);
	}

	public void SetColor(Color c)
	{
		render.color = c * origColor;
	}

	public void SetScale(float scale)
	{
		transform.localScale = new Vector3(origSize.x * scale, origSize.y * scale, 1f);
	}
}
