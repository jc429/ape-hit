using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VersusController : MonoBehaviour
{
	
	public void ConfirmButtonPressed()
	{
		GetComponent<CombatManager>().MatchStart();
	}
	
	public void CancelButtonPressed()
	{

	}

	public void DirectionPressed()
	{

	}
}
