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

	public UnityEvent clickEvent;

	private void Awake() {
		if(GetComponent<Image>())
			GetComponent<Image>().color = selectable ? Color.white : Color.gray;
	}

	public void ExecuteButton()
	{
		clickEvent.Invoke();
	}
}
