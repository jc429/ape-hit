using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
	private static GameInputs gameInputs;

	public UnityEvent confirmEvent;
	public UnityEvent cancelEvent;
	public UnityEvent directionEvent;

	private void Awake() {
		GameController.inputController = this;
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
		//gameInputs.Menu.Confirm.performed += _ => GameController.Instance.StartButtonPressed();
		gameInputs.Menu.Confirm.performed += _ => confirmEvent.Invoke();
		gameInputs.Menu.Cancel.performed += _ => cancelEvent.Invoke();
		
	}
}
