using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
	[SerializeField]
	Overlay overlay;
	public Overlay Overlay{
		get { return overlay; }
	}
	
	[SerializeField]
	GameObject hud;

	[SerializeField]
	[NamedArrayAttribute(new string[]{"P1 R1", "P1 R2", "P2 R1", "P2 R2"})]
	RoundMarker[] roundMarkers;


	private void Awake() {
		GameController.uiManager = this;
		InitUI();
	}

	public void InitUI()
	{
		ResetUI();
		SetHUDActive(false);
		overlay.ShowScreen(OverlayScreen.PressStart);
	}


	public void ResetUI()
	{
		overlay.HideScreen();
		foreach(RoundMarker marker in roundMarkers)
		{
			marker.ResetMarker();
		}
	}

	public void AddWin(RoundResult roundState, int winNo)
	{
		if(winNo >= VersusCombatManager.RoundsToWin)
			return;
		if(roundState == RoundResult.Draw)
			return;
		switch(roundState)
		{
			case RoundResult.P1Win:
				roundMarkers[winNo].SetMarkerState(RoundMarker.MarkerState.Victory);
				break;
			case RoundResult.P2Win:
				roundMarkers[VersusCombatManager.RoundsToWin + winNo].SetMarkerState(RoundMarker.MarkerState.Victory);
				break;
		}
	}

	public void SetHUDActive(bool active)
	{
		hud?.SetActive(active);
	}
}
