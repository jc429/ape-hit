using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OverlayScreen{
	PressStart	= 0,
	Ready		= 1,
	RoundStart	= 2,
	P1Win		= 3,
	P2Win		= 4,
	Draw		= 5,
	None
}

public class Overlay : MonoBehaviour
{
	SpriteRenderer _spriteRenderer{
		get { return GetComponent<SpriteRenderer>(); }
	}
	
	[SerializeField]
	[NamedArrayAttribute(new string[]{"Press Start", "Ready", "Start!", "P1 Win", "P2 Win", "Draw"})]
	Sprite[] overlayScreens;

	[SerializeField]
	GameObject readyBar;
	[SerializeField]
	GameObject readyMask;


	public void ShowScreen(OverlayScreen screen)
	{
		if(screen == OverlayScreen.None)
			HideScreen();
		else
		{
			_spriteRenderer.sprite = overlayScreens[(int)screen];
			if(screen == OverlayScreen.Ready)
			{
				ShowReadyBar();
			}
			else
			{
				HideReadyBar();
			}
		}
	}

	public void HideScreen()
	{
		_spriteRenderer.sprite = null;
		HideReadyBar();
	}

	public void SetReadyScale(float xScale)
	{
		readyMask.transform.localScale = new Vector3(xScale, 2, 1);
	}

	void ShowReadyBar()
	{
		readyBar.SetActive(true);
		readyMask.transform.localScale = new Vector3(40, 2, 1);
	}

	void HideReadyBar()
	{
		readyBar.SetActive(false);
		readyMask.transform.localScale = new Vector3(40, 2, 1);
	}
}
