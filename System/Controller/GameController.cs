using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
	Title			= 0,
	Versus			= 1,
	SinglePlayer	= 2,
}


public class GameController : MonoBehaviour
{
	private static GameController instance;
	public static GameController Instance{
		get { return instance; }
	}
	public static InputController inputController;
	public static UIManager uiManager;
	public static VersusCombatManager combatManager;
	public static GameCamera gameCamera;

	private static GameMode gameMode = GameMode.Title;

	private static bool gamePaused;
	private static float pausedTimeScale = 1;

	public static VersusMode versusMode;
	
	static PaletteIndex player1Pal = PaletteIndex.Player1;
	static PaletteIndex player2Pal = PaletteIndex.Player2;


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
		uiManager.ResetUI();
		uiManager.Overlay.HideScreen();
		GoToScene(GameMode.Title);
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

	public static void GoToScene(GameMode scene)
	{
		AudioController.instance.StopAllTracks();
		SceneManager.LoadScene((int)scene);
		Time.timeScale = 1;
	}

		
	private void OnEnable() {
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}


	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode){

	}

	public static void SetPalettes(PaletteIndex p1pal, PaletteIndex p2pal)
	{
		player1Pal = p1pal;
		player2Pal = p2pal;
	}

	public static PaletteIndex GetPaletteP1()
	{
		return player1Pal;
	}

	public static PaletteIndex GetPaletteP2()
	{
		return player2Pal;
	}


	public static void TakeScreenshot()
	{
		string time = System.DateTime.Now.ToString("yyyy'-'MM'-'dd'--'HH'-'mm'-'ss");
		string path = System.IO.Path.Combine(Application.dataPath, "../Export/screenshot " + time + ".png");
		ScreenCapture.CaptureScreenshot(path);
		Debug.Log("Screen capture saved! " + path);
	}
}
