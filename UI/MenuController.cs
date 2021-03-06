using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum MenuState{
	Closed,
	Opening,
	Open,
	Closing
}

[RequireComponent(typeof(RectTransform))]
public class MenuController : MonoBehaviour
{
	RectTransform _rectTransform{
		get { return GetComponent<RectTransform>(); }
	}

	public GameObject menuObj;
	public RectTransform panel;
	public GameObject cursor;
	[SerializeField]
	GameObject displayWhenOpen;

	[SerializeField]
	MenuButton[] buttons;
	[SerializeField]
	UnityEvent menuClosedEvent;

	const float refreshRate = 0.0075f;
	private WaitForSecondsRealtime refreshWait = new WaitForSecondsRealtime(refreshRate);
	const float pixelsPerUpdate = 0.125f;
	readonly Vector2 baseVec = new Vector2(0.5f, 0.5f);

	string menuSelect = "menuSelect";
	string softBlip = "menuBlipSoft";

	MenuState menuState;
	int selectedButton = 0;

	private void Awake() {
		ForceCloseMenu();
	}


	/// Opening ///

	public void StartOpenMenu()
	{
		menuState = MenuState.Opening;
		panel.sizeDelta = baseVec;
		menuObj.SetActive(true);
		panel.gameObject.SetActive(true);
		gameObject.SetActive(true);
		if(displayWhenOpen != null)
			displayWhenOpen.SetActive(false);
		AudioController.instance.PlaySound(menuSelect);
		StartCoroutine(OpenCoroutine());
	}

	IEnumerator OpenCoroutine()
	{
		
		while(panel.sizeDelta.x < _rectTransform.sizeDelta.x)
		{
			float width = Mathf.Clamp(panel.sizeDelta.x + pixelsPerUpdate, baseVec.x, _rectTransform.sizeDelta.x);
			panel.sizeDelta = new Vector2(width, panel.sizeDelta.y);
			yield return refreshWait;
		}
		
		while(panel.sizeDelta.y < _rectTransform.sizeDelta.y)
		{
			float height = Mathf.Clamp(panel.sizeDelta.y + pixelsPerUpdate, baseVec.y, _rectTransform.sizeDelta.y);
			panel.sizeDelta = new Vector2(panel.sizeDelta.x, height);
			yield return refreshWait;
		}
		FinishOpenMenu();
	}

	void FinishOpenMenu()
	{
		menuState = MenuState.Open;
		foreach(MenuButton b in buttons)
		{
			b.gameObject.SetActive(true);
		}
		selectedButton = GetFirstSelectableItem();
		if(selectedButton < 0)
		{
			StartCloseMenu();
			return;
		}
		if(displayWhenOpen != null)
			displayWhenOpen.SetActive(true);
		UpdateCursorPosition();
		//cursor.SetActive(true);
		buttons[selectedButton].HighlightButton();
	}


	/// Closing ///

	public void StartCloseMenu()
	{
		menuState = MenuState.Closing;
		foreach(MenuButton b in buttons)
		{
			b.ClearButton();
			b.gameObject.SetActive(false);
		}
		if(displayWhenOpen != null)
			displayWhenOpen.SetActive(false);
		cursor.SetActive(false);
		StartCoroutine(CloseCoroutine());
	}

	IEnumerator CloseCoroutine()
	{
		
		while(panel.sizeDelta.y > baseVec.y)
		{
			float height = Mathf.Clamp(panel.sizeDelta.y - pixelsPerUpdate, baseVec.y, _rectTransform.sizeDelta.y);
			panel.sizeDelta = new Vector2(panel.sizeDelta.x, height);
			yield return refreshWait;
		}

		while(panel.sizeDelta.x > baseVec.x)
		{
			float width = Mathf.Clamp(panel.sizeDelta.x - pixelsPerUpdate, baseVec.x, _rectTransform.sizeDelta.x);
			panel.sizeDelta = new Vector2(width, panel.sizeDelta.y);
			yield return refreshWait;
		}
		
		FinishCloseMenu();
	}
	
	void FinishCloseMenu()
	{
		menuState = MenuState.Closed;
		panel.sizeDelta = new Vector2(0.5f, 0.5f);
		panel.gameObject.SetActive(false);
		menuObj.SetActive(false);
		menuClosedEvent.Invoke();
	}

	void ForceCloseMenu()
	{
		foreach(MenuButton b in buttons)
		{
			b.ClearButton();
			b.gameObject.SetActive(false);
		}
		if(displayWhenOpen != null)
			displayWhenOpen.SetActive(false);
		cursor.SetActive(false);
		menuState = MenuState.Closed;
		panel.sizeDelta =  new Vector2(0.5f, 0.5f);
		panel.gameObject.SetActive(false);
		menuObj.SetActive(false);
	}


	/// Buttons ///

	public void MoveCursor(Vector2 input)
	{
		
		if(input.x != 0)
		{
			PaletteSelector ps = buttons[selectedButton].GetComponent<PaletteSelector>();
			if(ps != null)
			{
				ps.CyclePalette(Mathf.RoundToInt(input.x));
				AudioController.instance.PlaySound(softBlip);
				//return;
			}
		}

		if(input.y < 0)
		{
			buttons[selectedButton].ClearButton();
			do{
				selectedButton++;
				selectedButton %= buttons.Length;
			}while(!buttons[selectedButton].selectable);
			buttons[selectedButton].HighlightButton();
			UpdateCursorPosition();
		}
		if(input.y > 0)
		{
			buttons[selectedButton].ClearButton();
			do{
				selectedButton += (buttons.Length-1);
				selectedButton %= buttons.Length;
			}while(!buttons[selectedButton].selectable);
			buttons[selectedButton].HighlightButton();
			UpdateCursorPosition();
		}
		AudioController.instance.PlaySound(softBlip);
	}

	void UpdateCursorPosition()
	{
		//Vector3 pos = buttons[selectedButton].GetComponent<RectTransform>().localPosition;
		//cursor.GetComponent<RectTransform>().localPosition = pos;
	}

	int GetFirstSelectableItem()
	{
		for(int i = 0; i < buttons.Length; i++)
		{
			if(buttons[i].selectable)
				return i;
		}
		return -1;
	}

	public void SelectButton(){
		buttons[selectedButton].ExecuteButton();
		AudioController.instance.PlaySound(menuSelect);
	}

	
	public MenuState GetMenuState(){
		return menuState;
	}
}
