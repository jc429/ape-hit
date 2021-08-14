using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteImage : MonoBehaviour
{
	Image _image{
		get { return GetComponent<Image>(); }
	}
	private Texture2D paletteTex;
	private Color[] mSpriteColors;

	const float flashDuration = 0.1f;
	float flashTimer;

	[SerializeField]
	PaletteIndex pIndex;
	public Texture2D backupPalette;

	// Start is called before the first frame update
	void Awake() {
		InitPaletteTex();
		SetPalette(pIndex);
	}

	// Update is called once per frame
	void Update() {
	}

	public void InitPaletteTex(){
		//Texture2D colorSwapTex = (Texture2D)_spriteRenderer.material.GetTexture("_SwapTex");
		Texture2D tex = (Texture2D)_image.material.GetTexture("_SwapTex");
		if(tex == null){
			tex = new Texture2D(64, 64, TextureFormat.RGBA32, false, false);
			tex.filterMode = FilterMode.Point;
			_image.material.SetTexture("_SwapTex", tex);
		}
		Texture2D colorSwapTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false, false);
		colorSwapTex.filterMode = FilterMode.Point;
		colorSwapTex.SetPixels(tex.GetPixels());
		mSpriteColors = new Color[colorSwapTex.width];
		paletteTex = colorSwapTex;
	}

	public void SetPalette(PaletteIndex index){
		Texture2D colorSwapTex;
		if(paletteTex == null)
			InitPaletteTex();
		colorSwapTex = paletteTex;
		for (int i = 0; i < colorSwapTex.width; ++i){
			Color c = colorSwapTex.GetPixel(i,(2 * (int)index) + 1);
			colorSwapTex.SetPixel(i, 0, c);
		}
		colorSwapTex.Apply();
		_image.material.SetTexture("_SwapTex", colorSwapTex);
	}

	public void SwapColor(int index, Color color){
		mSpriteColors[index] = color;
		paletteTex.SetPixel(index, 0, color);
	}

	void FlashColor(Color color){
		for (int i = 0; i < paletteTex.width; ++i){
			mSpriteColors[i] = paletteTex.GetPixel(i,0);
			paletteTex.SetPixel(i, 0, color);
		}
		paletteTex.Apply();
	}

	public void ResetColors(){
		for (int i = 0; i < paletteTex.width; ++i){
			paletteTex.SetPixel(i, 0, mSpriteColors[i]);
		}
		paletteTex.Apply();
	}

}
