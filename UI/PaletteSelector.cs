using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteSelector : MonoBehaviour
{
	[SerializeField]
	PaletteSprite paletteSprite;

	public PaletteIndex currentPalette;

	const int numPalettes = 6;
	const int palOffset = 1;

	private void Awake() {
		SetPalette(currentPalette);
	}

	public void CyclePalette(int dir)
	{
		int pal = (int)currentPalette - palOffset;
		pal = (((pal+dir)+numPalettes) % numPalettes) + palOffset;
		SetPalette((PaletteIndex)pal);
	}

	void SetPalette(PaletteIndex index)
	{
		currentPalette = index;
		paletteSprite.SetPalette(index);
	}

	public PaletteIndex GetPalette()
	{
		return currentPalette;
	}
}
