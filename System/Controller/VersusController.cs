using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class VersusController : MonoBehaviour
{
	VersusCombatManager _combatManager{
		get { return GetComponent<VersusCombatManager>(); }
	}

	[SerializeField]
	MenuController pauseMenu;

	
	public void ConfirmButtonPressed()
	{
		if(pauseMenu.GetMenuState() == MenuState.Open)
		{
			pauseMenu.SelectButton();
		}
		else{
			switch(VersusCombatManager.RoundState)
			{
				case RoundState.MatchPreStart:
					_combatManager.MatchStart();
					break;
				case RoundState.Active:
					if(!VersusCombatManager.GameplayPaused)
						PauseMatch();
					break;
			}
		}
	}
	
	public void CancelButtonPressed()
	{
		if(pauseMenu.GetMenuState() == MenuState.Open)
		{
			pauseMenu.StartCloseMenu();
		}
	}

	public void DirectionPressed(InputAction.CallbackContext ctx)
	{
		if(pauseMenu.GetMenuState() == MenuState.Open)
		{
			pauseMenu.MoveCursor(ctx.ReadValue<Vector2>());
		}
	}



	public void GoToTitle()
	{
		_combatManager.ClearCombat();
		SceneManager.LoadScene(0);
	}


	public void PauseMatch()
	{
		VersusCombatManager.PauseGameplay();
		pauseMenu.StartOpenMenu();
		
	}

	public void UnpauseMatch()
	{
		VersusCombatManager.UnpauseGameplay();
	}
}
