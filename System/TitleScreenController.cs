using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleScreenController : MonoBehaviour
{
	[SerializeField]
	MenuController mainMenu;
	[SerializeField]
	GameObject titleOverlay;

	private void Awake() {
		GameController.SetGameMode(GameMode.Title);
	}

	public void ConfirmButtonPressed()
	{
		switch(mainMenu.GetMenuState())
		{
			case MenuState.Closed:
				titleOverlay.SetActive(false);
				mainMenu.StartOpenMenu();
				break;
			case MenuState.Open:
				mainMenu.SelectButton();
				break;
		}
	}
	
	public void CancelButtonPressed()
	{
		if(mainMenu.GetMenuState() == MenuState.Open)
		{
			mainMenu.StartCloseMenu();
		}
	}

	public void DirectionPressed(InputAction.CallbackContext ctx)
	{
	
	}



	public void GoToVersus()
	{
		SceneManager.LoadScene(1);
	}
}
