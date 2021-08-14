using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
	private static GameInputs gameInputs;

	public UnityEvent<InputAction.CallbackContext> confirmEvent;
	public UnityEvent<InputAction.CallbackContext> cancelEvent;
	public UnityEvent<InputAction.CallbackContext> directionEvent;

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

		//gameInputs.Menu.Confirm.performed += _ => GameController.Instance.StartButtonPressed();
		gameInputs.Menu.Confirm.performed += ctx => confirmEvent.Invoke(ctx);
		gameInputs.Menu.Cancel.performed += ctx => cancelEvent.Invoke(ctx);
		gameInputs.Menu.Direction.performed += ctx => directionEvent.Invoke(ctx);
		gameInputs.Menu.TakeScreenshot.performed += _ => GameController.TakeScreenshot();
	}

}
