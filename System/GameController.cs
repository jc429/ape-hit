using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
	Title,
	Versus,
	SinglePlayer
}

public class GameController : MonoBehaviour
{
	private static GameController instance;
	public static GameController Instance{
		get { return instance; }
	}
	public static InputController inputController;
	public static UIManager uiManager;
	public static CombatManager combatManager;
	public static GameCamera gameCamera;

	private static GameMode gameMode = GameMode.Title;

	private static bool gamePaused;
	private static float pausedTimeScale = 1;

	private void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if(instance != this) {
			Destroy(this.gameObject);
			return;
		}
	}

	private void Start() {
		InitGame();
	}

	public void InitGame()
	{
	}

	

	public void RestartGame()
	{
		combatManager.ClearCombat();
		uiManager.ResetUI();
		uiManager.Overlay.HideScreen();
		InitGame();
	}

	public static void PauseGame()
	{
		if(gamePaused)
			return;
		gamePaused = true;
		pausedTimeScale = Time.timeScale;
	}

	public static void UnpauseGame()
	{
		if(!gamePaused)
			return;
		gamePaused = false;
		Time.timeScale = pausedTimeScale;
	}

	public static bool GamePaused()
	{
		return gamePaused;
	}

	public static void SetGameMode(GameMode game_mode)
	{
		gameMode = game_mode;
	}
	public static GameMode GetGameMode()
	{
		return gameMode;
	}
}
