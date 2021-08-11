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


public class VersusCombatManager : GameModeController
{


	[SerializeField]
	HPBar hpBarL;
	[SerializeField]
	HPBar hpBarR;

	[SerializeField]
	PlayerController playerPrefab;
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




	private void Awake() {
		roundState = RoundState.MatchPreStart;
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



	////////////////
	/// Spawning ///
	////////////////

	public void SpawnPlayers()
	{
		gameplayPaused = true;
		for(int i = 0; i < 2; i++)
		{
			PlayerController player = Instantiate(playerPrefab) as PlayerController;
			player.transform.parent = playerContainer;
			players.Add(player);
			player.AddInputLock(InputLockType.System);
			player.SetActionState(ActionState.Victory);
			player.playerID = i;
		}
		players[0].SetPalette(PaletteIndex.Player1);
		players[0].transform.localPosition = p1Spawn;
		players[0].SetHPBar(hpBarL);
		players[0].SetOpponent(players[1]);
		players[0].SetFacing(Direction.E);
		players[0].GetComponent<PlayerInput>().SwitchCurrentControlScheme("Player 1 Keyboard");

		players[1].SetPalette(PaletteIndex.Player2);
		players[1].transform.localPosition = p2Spawn;
		players[1].SetHPBar(hpBarR);
		players[1].SetOpponent(players[0]);
		players[1].SetFacing(Direction.W);
		players[1].GetComponent<PlayerInput>().SwitchCurrentControlScheme("Player 2 Keyboard");
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
		GameController.Instance.RestartGame();
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


	//////////////
	/// Rounds ///
	//////////////

	public void RoundStart()
	{
		roundState = RoundState.Starting;
		StartCoroutine(RoundReadyCoroutine());
	}

	public IEnumerator RoundReadyCoroutine()
	{
		GameController.uiManager.Overlay.ShowScreen(OverlayScreen.Ready);
		for(int i = 0; i < 20; i++)
		{
			GameController.uiManager.Overlay.SetReadyScale((19-i)*2);
			yield return new WaitForSecondsRealtime(0.075f);
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





}
