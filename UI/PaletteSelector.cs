using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteSelector : MonoBehaviour
{
	[SerializeField]
	PaletteSprite paletteSprite;
	[SerializeField][Range(0,1)]
	int playerID;

	public PaletteIndex currentPalette;

	const int numPalettes = (int)PaletteIndex.NUMPALETTES;
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

	private void OnEnable() {
		PaletteIndex pal = (playerID == 0) ? GameController.GetPaletteP1() : GameController.GetPaletteP2();
		SetPalette(pal);
	}
}
