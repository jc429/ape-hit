using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum ButtonID{
	SinglePlayer,
	Versus,
	Options,
	Cancel
}

public class MenuButton : MonoBehaviour
{
	public ButtonID buttonID;
	public bool selectable;

	[SerializeField]
	Image[] images;


	public UnityEvent clickEvent;

	private void Awake() {
		if(!selectable)
		{
			DeactivateButton();
		}
		images = GetComponentsInChildren<Image>();
	}

	public void ExecuteButton()
	{
		clickEvent.Invoke();
	}


	public void HighlightButton()
	{
		ColorButton(PColor.HighlightColor);
	}

	public void ClearButton()
	{
		ColorButton(Color.white);
	}

	public void DeactivateButton()
	{
		ColorButton(Color.gray);
	}

	void ColorButton(Color c)
	{
		foreach(Image img in images)
		{
			img.color = c;
		}
	}
}
