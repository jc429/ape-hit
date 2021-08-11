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
		DisplayTitleOverlay();
	}

	public void ConfirmButtonPressed()
	{
		switch(mainMenu.GetMenuState())
		{
			case MenuState.Closed:
				HideTitleOverlay();
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
		if(mainMenu.GetMenuState() == MenuState.Open)
		{
			mainMenu.MoveCursor(ctx.ReadValue<Vector2>());
		}
	}

	public void DisplayTitleOverlay()
	{
		titleOverlay.SetActive(true);
	}

	public void HideTitleOverlay()
	{
		titleOverlay.SetActive(false);
	}

	public void GoToVersus()
	{
		SceneManager.LoadScene(1);
	}
}
