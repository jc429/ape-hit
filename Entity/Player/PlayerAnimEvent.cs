using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackID
{
	LightPunch = 0,
	HeavyPunch = 1
}


public class PlayerAnimEvent : MonoBehaviour
{
	PlayerController _playerController{
		get { return GetComponentInParent<PlayerController>(); }
	}
	PlayerMovement _playerMovement{
		get { return GetComponentInParent<PlayerMovement>(); }
	}
	public PlayerHitboxManager _hitboxManager;


	void SetFreeCancel()
	{
		_playerController.SetCancelState(CancelState.FreeCancel);
	}



	void AttackStartup(AttackID id)
	{
		_playerController.SetCancelState(CancelState.NoCancel);
		_playerMovement.AddMoveLock(MoveLockType.Animation);
	}

	void AttackActive(AttackID id)
	{
		_playerController.SetCancelState(CancelState.NoCancel);
		// set hitboxes for attack
		_hitboxManager.SetHitboxState(id);
	}

	void AttackRecovery(AttackID id)
	{
		_hitboxManager.DisableAllHitboxes();
		switch(id){
			case AttackID.LightPunch:
				_playerController.SetCancelState(CancelState.FreeCancel);
				break;
			default:
				_playerController.SetCancelState(CancelState.FreeCancel);
				break;
		}
	}
	
	void AttackEnd(AttackID id)
	{
		SetFreeCancel();
		_playerMovement.RemoveMoveLock(MoveLockType.Animation);
	}
}
