using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PColor{
	public static Color HighlightColor = Color255(224,129,28);



	public static Color Color255(float r, float g, float b, float a)
	{
		return new Color(r/255f, g/255f, b/255f, a/255f);
	}

	public static Color Color255(float r, float g, float b)
	{
		return new Color(r/255f, g/255f, b/255f);
	}
}
