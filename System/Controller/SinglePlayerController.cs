using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum StageState{
	StagePreStart,
	Starting,
	Active,
	Ended
}

public class SinglePlayerController : GameModeController
{
	static StageState stageState;

	[SerializeField]
	HPBar hpBar;

	[SerializeField]
	PlayerController playerPrefab;
	[SerializeField]
	Transform playerContainer;
	readonly Vector3 playerSpawn = new Vector3(-1, -1);

	[SerializeField]
	TrackingCamera trackingCamera;




	private void Awake() {
		stageState = StageState.StagePreStart;
	}

	private void FixedUpdate() {

		if(players.Count == 0)
			return;
		if(!gameplayPaused)
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

		}
	}







	////////////////
	/// Spawning ///
	////////////////

	public void SpawnPlayers()
	{
		gameplayPaused = true;

		PlayerController player = Instantiate(playerPrefab) as PlayerController;
		player.transform.parent = playerContainer;
		player.AddInputLock(InputLockType.System);
		player.SetActionState(ActionState.Victory);
		player.playerID = 0;
		player.SetPalette(PaletteIndex.Player1);
		player.transform.localPosition = playerSpawn;
		player.SetHPBar(hpBar);
		player.SetFacing(Direction.E);
		player.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Player 1 Keyboard");
		trackingCamera.SetTarget(player.transform);
		players.Add(player);
	}



	
	public void ClearCombat()
	{
		gameplayPaused = true;
		hpBar.ResetHP();
		foreach(PlayerController p in players)
		{
			Destroy(p.gameObject);
		}
		players.Clear();
		stageState = StageState.StagePreStart;
	}


	//////////////
	/// Rounds ///
	//////////////

	public void StageStart()
	{
		if(stageState != StageState.StagePreStart)
			return;
		if(players.Count == 0)
		{
			SpawnPlayers();
		}
		GameController.uiManager.ResetUI();
		GameController.uiManager.Overlay.HideScreen();
		hpBar.ResetHP();
		foreach(PlayerController p in players)
		{
			p.SetActionState(ActionState.Idle);
		}
		gameplayPaused = false;
		stageState = StageState.Starting;
		StartCoroutine(StageReadyCoroutine());
	}

	public IEnumerator StageReadyCoroutine()
	{
		GameController.uiManager.Overlay.ShowScreen(OverlayScreen.Ready);
		for(int i = 0; i < 20; i++)
		{
			GameController.uiManager.Overlay.SetReadyScale((19-i)*2);
			yield return new WaitForSecondsRealtime(0.075f);
		}

		// round has officially started here
		stageState = StageState.Active;
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

	
	private IEnumerator StageEndCoroutine(RoundResult roundResult)
	{
		gameplayPaused = true;
		stageState = StageState.Ended;
		//Debug.Log("Round Over");
		players[0].AddInputLock(InputLockType.RoundOver);
		yield return new WaitForSecondsRealtime(1);

		gameplayPaused = true;
		ClearCombat();
		GameController.Instance.RestartGame();
		yield return new WaitForSecondsRealtime(3);
		RestartStage();
		

	}

	public void RestartStage()
	{
		stageState = StageState.StagePreStart;
		hpBar.ResetHP();
		players[0].ResetPlayer();
		players[0].transform.localPosition = playerSpawn;
		players[0].SetFacing(Direction.E);
		UnfreezeTime(TimeFreezeSouce.ALL);
		StageStart();
	}

	


}
