using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TitleScreenController : GameModeController
{
	[SerializeField]
	GameObject titleOverlay;

	private void Awake() {
		GameController.SetGameMode(GameMode.Title);
		DisplayTitleOverlay();
	}

	private void Start() {
		AudioController.instance.PlayTrack(TrackID.Title);
	}



	protected override void StartButtonPressed()
	{
		HideTitleOverlay();
		pauseMenu.StartOpenMenu();
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
		GameController.versusMode = VersusMode.Player_VS_Player;
		GameController.GoToScene(GameMode.Versus);
	}

	public void GoToSinglePlayer()
	{
		GameController.versusMode = VersusMode.Player_VS_CPU;
		GameController.GoToScene(GameMode.Versus);
	}


}
