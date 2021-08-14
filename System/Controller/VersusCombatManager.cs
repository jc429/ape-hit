using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;


public enum RoundState{
	MatchPreStart,
	Starting,
	Active,
	Ended
}

public enum RoundResult{
	P1Win,
	P2Win,
	P1Perfect,
	P2Perfect,
	Draw
}

public enum VersusMode{
	Player_VS_CPU,
	Player_VS_Player,
	CPU_VS_CPU
}


public class VersusCombatManager : GameModeController
{


	[SerializeField]
	HPBar hpBarL;
	[SerializeField]
	HPBar hpBarR;

	[SerializeField]
	PlayerController playerPrefab;
	[SerializeField]
	PlayerController aiPrefab;
	[SerializeField]
	Transform playerContainer;

	static RoundState roundState;
	public static RoundState RoundState{
		get { return roundState; }
	}

	public const int RoundsToWin = 2;
	readonly Vector3 p1Spawn = new Vector3(-1, -1);
	readonly Vector3 p2Spawn = new Vector3(1, -1);
	int p1RoundWins;
	int p2RoundWins;

	WaitForSecondsRealtime readyTimerWait = new WaitForSecondsRealtime(0.075f);


	private void Awake() {
		GameController.SetGameMode(GameMode.Versus);
		roundState = RoundState.MatchPreStart;
	}

	private void Start() {
		InitMatch();
	}

	private void FixedUpdate() {

		if(players.Count == 0)
			return;
		if(!GameplayPaused)
		{
			foreach(PlayerController pc in players)
			{
				pc.CombatUpdate();
			}

			if(currentHits.Count > 0)
			{
				foreach(AttackHit hit in currentHits)
				{
					ProcessHit(hit);
				}
			}
			currentHits.Clear();

			CheckWinners();
		}	
	}

	protected void InitMatch()
	{
		GameController.uiManager.InitUI();
		AudioController.instance.PlayTrack(TrackID.Versus);
	}


	protected override void StartButtonPressed(InputAction.CallbackContext ctx)
	{
		switch(RoundState)
		{
			case RoundState.MatchPreStart:
				MatchStart();
				break;
		}
	}

	protected override void StartButtonLongPress(InputAction.CallbackContext ctx)
	{
		switch(RoundState)
		{
			case RoundState.MatchPreStart:
				MatchStart();
				break;
			case RoundState.Active:
				if(!GameplayPaused)
				{
					VersusCombatManager.PauseGameplay();
					pauseMenu.StartOpenMenu();
				}
				break;
		}
	}


	public void UnpauseMatch()
	{
		UnpauseGameplay();
	}

	private void CheckWinners()
	{
		if(winners.Count <= 0)
			return;
		RoundResult roundResult = RoundResult.Draw;
		if(winners.Count > 1)
		{
			roundResult = RoundResult.Draw;
		}
		else{
			switch(winners[0].playerID)
			{
				case 0:
					roundResult = RoundResult.P1Win;
					break;
				case 1:
					roundResult = RoundResult.P2Win;
					break;
			}
		}
		StartCoroutine(RoundEndCoroutine(roundResult));
	}

	public void GoToTitle()
	{
		ClearCombat();
		GameController.GoToScene(GameMode.Title);
	}

	////////////////
	/// Spawning ///
	////////////////

	public void SpawnPlayers()
	{
		gameplayPaused = true;
		for(int i = 0; i < 2; i++)
		{
			PlayerController player;
			switch(GameController.versusMode)
			{
				case VersusMode.Player_VS_CPU:
					player = Instantiate(i == 0 ? playerPrefab : aiPrefab) as PlayerController;
					break;
				case VersusMode.CPU_VS_CPU:
					player = Instantiate(aiPrefab) as PlayerController;
					break;
				case VersusMode.Player_VS_Player:
				default:
					player = Instantiate(playerPrefab) as PlayerController;
					break;
			}
			player.transform.parent = playerContainer;
			players.Add(player);
			player.AddInputLock(InputLockType.System);
			player.SetActionState(ActionState.Victory);
			player.playerID = i;
		}
		players[0].SetPalette(GameController.GetPaletteP1());
		players[0].transform.localPosition = p1Spawn;
		players[0].SetHPBar(hpBarL);
		players[0].SetOpponent(players[1]);
		players[0].SetFacing(Direction.E);
		players[0].SetSortingOrder(10);
		if(!players[0].isAI)
		{
			players[0].GetComponent<PlayerInput>().SwitchCurrentControlScheme("KeyboardLeft");
		}
		else
		{
			players[0].StartAIController();
		}

		players[1].SetPalette(GameController.GetPaletteP2());
		players[1].transform.localPosition = p2Spawn;
		players[1].SetHPBar(hpBarR);
		players[1].SetOpponent(players[0]);
		players[1].SetFacing(Direction.W);
		players[1].SetSortingOrder(5);
		if(!players[1].isAI)
		{
			players[1].GetComponent<PlayerInput>().SwitchCurrentControlScheme("KeyboardRight");
		}
		else
		{
			players[1].StartAIController();
		}
	}


	//////////////
	/// Combat ///
	//////////////

	protected override void HitPostProcess(AttackHit hit){
		PlayerController playerController = hit.target.owner.GetComponent<PlayerController>();
		if(playerController != null && playerController.IsDead())
		{
			winners.Add(playerController.opponent);
		}
	}

	///////////////
	/// Matches ///
	///////////////

	public void MatchStart()
	{
		if(roundState != RoundState.MatchPreStart)
			return;
		if(players.Count == 0)
		{
			SpawnPlayers();
		}
		GameController.uiManager.ResetUI();
		GameController.uiManager.Overlay.HideScreen();
		GameController.uiManager.SetHUDActive(true);
		winners.Clear();
		hpBarL.ResetHP();
		hpBarR.ResetHP();
		p1RoundWins = 0;
		p2RoundWins = 0;
		foreach(PlayerController p in players)
		{
			p.SetActionState(ActionState.Idle);
		}
		gameplayPaused = false;
		RoundStart();
	}

	private IEnumerator MatchEndCoroutine(RoundResult roundResult)
	{
		//Debug.Log("Match Over");
		gameplayPaused = true;
		switch(roundResult)
		{
			case RoundResult.P1Win:
			case RoundResult.P1Perfect:
				GameController.uiManager.Overlay.ShowScreen(OverlayScreen.P1Win);
				break;
			case RoundResult.P2Win:
			case RoundResult.P2Perfect:
				GameController.uiManager.Overlay.ShowScreen(OverlayScreen.P2Win);
				break;
			case RoundResult.Draw:
			default:
				GameController.uiManager.Overlay.ShowScreen(OverlayScreen.Draw);
				break;
		}
		yield return new WaitForSecondsRealtime(5);
		ClearCombat();
		RestartMatch();
	}

	
	public void ClearCombat()
	{
		gameplayPaused = true;
		winners.Clear();
		hpBarL.ResetHP();
		hpBarR.ResetHP();
		foreach(PlayerController p in players)
		{
			Destroy(p.gameObject);
		}
		players.Clear();
		roundState = RoundState.MatchPreStart;
	}

	public void RestartMatch()
	{
		//GameController.Instance.RestartGame();
		AudioController.instance.StopAllTracks();
		roundState = RoundState.MatchPreStart;
		InitMatch();
	}

	//////////////
	/// Rounds ///
	//////////////

	public void RoundStart()
	{
		roundState = RoundState.Starting;
		foreach(PlayerController p in players)
		{
			p.AddInputLock(InputLockType.System);
		}
		StartCoroutine(RoundReadyCoroutine());
	}

	public IEnumerator RoundReadyCoroutine()
	{
		GameController.uiManager.Overlay.ShowScreen(OverlayScreen.Ready);
		for(int i = 0; i < 20; i++)
		{
			GameController.uiManager.Overlay.SetReadyScale((19-i)*2);
			yield return readyTimerWait;
		}

		// round has officially started here
		roundState = RoundState.Active;
		GameController.uiManager.Overlay.ShowScreen(OverlayScreen.RoundStart);
		foreach(PlayerController p in players)
		{
			p.RemoveInputLock(InputLockType.System|InputLockType.RoundOver);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		GameController.uiManager.Overlay.HideScreen();
		gameplayPaused = false;
		UnfreezeTime(TimeFreezeSouce.ALL);
	}

	
	private IEnumerator RoundEndCoroutine(RoundResult roundResult)
	{
		gameplayPaused = true;
		roundState = RoundState.Ended;
		winners.Clear();
		//Debug.Log("Round Over");
		foreach(PlayerController p in players)
		{
			p.AddInputLock(InputLockType.RoundOver);
		}
		yield return new WaitForSecondsRealtime(1);
		switch(roundResult)
		{
			case RoundResult.P1Win:
			case RoundResult.P1Perfect:
				players[0].SetActionState(ActionState.Victory);
				players[1].SetActionState(ActionState.Dead);
				GameController.uiManager.AddWin(RoundResult.P1Win, p1RoundWins);
				p1RoundWins++;
				break;
			case RoundResult.P2Win:
			case RoundResult.P2Perfect:
				players[0].SetActionState(ActionState.Dead);
				players[1].SetActionState(ActionState.Victory);
				GameController.uiManager.AddWin(RoundResult.P2Win, p2RoundWins);
				p2RoundWins++;
				break;
			case RoundResult.Draw:
			default:
				players[0].SetActionState(ActionState.Dead);
				players[1].SetActionState(ActionState.Dead);
				GameController.uiManager.Overlay.ShowScreen(OverlayScreen.Draw);
				break;
		}
		if(p1RoundWins >= RoundsToWin && p2RoundWins >= RoundsToWin)
		{
			StartCoroutine(MatchEndCoroutine(RoundResult.Draw));
		}
		else if(p1RoundWins >= RoundsToWin && p2RoundWins < RoundsToWin)
		{
			StartCoroutine(MatchEndCoroutine(RoundResult.P1Win));
		}
		else if(p2RoundWins >= RoundsToWin && p1RoundWins < RoundsToWin)
		{
			StartCoroutine(MatchEndCoroutine(RoundResult.P2Win));
		}
		else
		{
			yield return new WaitForSecondsRealtime(3);
			RestartRound();
		}

	}

	public void RestartRound()
	{
		winners.Clear();
		roundState = RoundState.MatchPreStart;
		hpBarL.ResetHP();
		hpBarR.ResetHP();
		players[0].ResetPlayer();
		players[1].ResetPlayer();
		players[0].transform.localPosition = p1Spawn;
		players[1].transform.localPosition = p2Spawn;
		players[0].SetFacing(Direction.E);
		players[1].SetFacing(Direction.W);
		UnfreezeTime(TimeFreezeSouce.ALL);
		RoundStart();
	}


		
	public override void SavePalettes()
	{
		PaletteIndex p1pal = paletteSelectors[0].GetPalette();
		PaletteIndex p2pal = paletteSelectors[1].GetPalette();
		GameController.SetPalettes(p1pal, p2pal);
		if(players.Count > 0){
			players[0].SetPalette(p1pal);
		}
		if(players.Count > 1){
			players[1].SetPalette(p2pal);
		}
		
	}

}
