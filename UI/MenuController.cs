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

	public RectTransform panel;
	public GameObject cursor;
	[SerializeField]
	MenuButton[] buttons;
	[SerializeField]
	UnityEvent menuClosedEvent;

	MenuState menuState;
	int selectedButton = 0;

	private void Awake() {
		ForceCloseMenu();
	}


	/// Opening ///

	public void StartOpenMenu()
	{
		menuState = MenuState.Opening;
		panel.sizeDelta = new Vector2(0.5f, 0.5f);
		panel.gameObject.SetActive(true);
		StartCoroutine(OpenCoroutine());
	}

	IEnumerator OpenCoroutine()
	{
		float speed = 0.01f;
		float inc = 0.125f;
		while(panel.sizeDelta.x < _rectTransform.sizeDelta.x)
		{
			float width = Mathf.Clamp(panel.sizeDelta.x + inc, 0, _rectTransform.sizeDelta.x);
			panel.sizeDelta = new Vector2(width, panel.sizeDelta.y);
			yield return new WaitForSeconds(speed);
		}
		
		while(panel.sizeDelta.y < _rectTransform.sizeDelta.y)
		{
			float height = Mathf.Clamp(panel.sizeDelta.y + inc, 0, _rectTransform.sizeDelta.y);
			panel.sizeDelta = new Vector2(panel.sizeDelta.x, height);
			yield return new WaitForSeconds(speed);
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
		cursor.SetActive(true);
	}


	/// Closing ///

	public void StartCloseMenu()
	{
		menuState = MenuState.Closing;
		foreach(MenuButton b in buttons)
		{
			b.gameObject.SetActive(false);
		}
		cursor.SetActive(false);
	}

	IEnumerator CloseCoroutine()
	{
		yield return null;
	}
	
	void FinishCloseMenu()
	{
		menuState = MenuState.Closed;
		panel.sizeDelta = new Vector2(0.5f, 0.5f);
		panel.gameObject.SetActive(false);
		menuClosedEvent.Invoke();
	}

	void ForceCloseMenu()
	{
		foreach(MenuButton b in buttons)
		{
			b.gameObject.SetActive(false);
		}
		cursor.SetActive(false);
		menuState = MenuState.Closed;
		panel.sizeDelta =  new Vector2(0.5f, 0.5f);
		panel.gameObject.SetActive(false);
	}


	/// Buttons ///

	public void SelectButton(){
		buttons[selectedButton].ExecuteButton();
	}

	
	public MenuState GetMenuState(){
		return menuState;
	}
}
