using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PaletteIndex{
	DEBUG		= 0,
	Player1		= 1,
	Player2		= 2,
	Gray		= 1,
	Brown		= 2,
	Red			= 3,
	Citrus		= 4,
	Banana		= 5,
	Melon		= 6,
	Jungle		= 7,
	Aqua		= 8,
	Grape		= 9,
	Lavender	= 10,
	Natural		= 14,
	RedRamp		= 15,
	NUMPALETTES	= 10
}


public class PaletteSprite : MonoBehaviour
{
	SpriteRenderer _spriteRenderer{
		get { return GetComponent<SpriteRenderer>(); }
	}
	private Texture2D paletteTex;
	private Color[] mSpriteColors;

	const float flashDuration = 0.1f;
	float flashTimer;

	PaletteIndex pIndex;
//	public Texture2D

	// Start is called before the first frame update
	void OnEnable() {
		InitPaletteTex();
		SetPalette(pIndex);
	}

	// Update is called once per frame
	void Update() {
	}

	public void InitPaletteTex(){
		//Texture2D colorSwapTex = (Texture2D)_spriteRenderer.material.GetTexture("_SwapTex");
		Texture2D tex = (Texture2D)_spriteRenderer.material.GetTexture("_SwapTex");
		if(tex == null){
			tex = new Texture2D(64, 64, TextureFormat.RGBA32, false, false);
			tex.filterMode = FilterMode.Point;
			_spriteRenderer.material.SetTexture("_SwapTex", tex);
		}
		Texture2D colorSwapTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false, false);
		colorSwapTex.filterMode = FilterMode.Point;
		colorSwapTex.SetPixels(tex.GetPixels());
		mSpriteColors = new Color[colorSwapTex.width];
		paletteTex = colorSwapTex;
	}

	public void SetPalette(PaletteIndex index){
		if(paletteTex == null)
		{
			paletteTex = (Texture2D)_spriteRenderer.material.GetTexture("_SwapTex");
		}
		Texture2D colorSwapTex = paletteTex;
		for (int i = 0; i < colorSwapTex.width; ++i){
			Color c = colorSwapTex.GetPixel(i,(2 * (int)index) + 1);
			colorSwapTex.SetPixel(i, 0, c);
		}
		colorSwapTex.Apply();
		_spriteRenderer.material.SetTexture("_SwapTex", colorSwapTex);
		paletteTex = colorSwapTex;
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

	

	protected void InitializeFlashTimer(){
	}

	protected void UpdateFlashTimer(){

		float f = Mathf.Lerp(1.0f,0.0f,0.2f);
		_spriteRenderer.material.SetFloat("_FlashAmount", f);
	}
	
	public virtual void StartFlash(){
		if(_spriteRenderer == null){
			Debug.Log("No Sprite Renderer :(");
			return;
		}
		_spriteRenderer.material.SetFloat("_FlashAmount", 1);

	}
	
	public virtual void FinishFlash(){
	}
}
