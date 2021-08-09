using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
	private static InputController instance;
	public static InputController Instance{
		get { return instance; }
	}

	private static GameInputs gameInputs;



	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if(instance != this) {
			Destroy(this.gameObject);
		}
		GenerateInputs();
	}


	
	private void OnEnable() {
		gameInputs?.Enable();
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnDisable() {
		gameInputs?.Disable();
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}


	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode){
		GenerateInputs();
	}

	void GenerateInputs()
	{
		gameInputs = new GameInputs();
		//gameInputs.Player.LightPunch.performed += _ => player1Controller.PerformLightPunch();
		//gameInputs.Player.HeavyPunch.performed += _ => player1Controller.PerformHeavyPunch();
		gameInputs.Menu.Confirm.performed += _ => GameController.Instance.StartButtonPressed();
	}
}
