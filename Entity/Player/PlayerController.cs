using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputLockType{
	System			= 0x0001,
	Pause			= 0x0002,
	RoundOver		= 0x0004,
	HitStun			= 0x0008,

	ALL				= 0xFFFF
}

public enum CancelState{
	FreeCancel,
	NoCancel,
	HeavyCancel
}

[SelectionBase]
public class PlayerController : EntityController, IHitboxResponder
{
	
	PlayerMovement _playerMovement{
		get { return GetComponent<PlayerMovement>(); }
	}
	PlayerAnim _playerAnim{
		get { return GetComponent<PlayerAnim>(); }
	}
	PlayerHitboxManager _hitboxManager{
		get { return GetComponentInChildren<PlayerHitboxManager>(); }
	}
	public PlayerHitboxManager GetHitboxManager()
	{
		return _hitboxManager;
	}

	public PlayerController opponent;

	public int playerID;
	private int inputLocks;
	CancelState cancelState;
	private Vector2Int moveInputs;
	private ActionState currentState;
	private int stunTimer;
	const int maxHP = 25;
	private int curHP = 25;
	private HPBar hpBar;

	/// Setup ///

	private void Awake() {
		
	}

	public void SetOpponent(PlayerController target)
	{
		opponent = target;
	}

	public void SetHPBar(HPBar hpbar)
	{
		hpBar = hpbar;
	}

	public void SetPalette(PaletteIndex palette)
	{
		GetComponentInChildren<PaletteSprite>().SetPalette(palette);
	}

	public void ResetPlayer()
	{
		stunTimer = 0;
		curHP = maxHP;
		SetActionState(ActionState.Idle);
		cancelState = CancelState.FreeCancel;
		_playerMovement.ClearMovementLocks();
		ClearInputLocks();
	}
	
	/// Updates ///

	private void FixedUpdate()
	{
		if(stunTimer > 0)
		{
			stunTimer--;
			if(stunTimer == 0)
			{
				_playerMovement.RemoveMoveLock(MoveLockType.HitStun);
			}
		}
		else
		{
			_playerMovement.GroundCheck();
			if(!InputLocked())
			{
				_playerMovement.UpdateMovement(moveInputs);
				UpdateFacing();
			}
		}
	}


	public void CombatUpdate()
	{
		_hitboxManager.CombatUpdate();
	}


	/// Facing Dir ///
	
	void UpdateFacing()
	{
		if(opponent == null)
			return;
		SetFacing((gameObject.transform.position.x > opponent.transform.position.x) ? Direction.W : Direction.E);
	}





	/// Actions ///

	public void SetActionState(ActionState state)
	{
		_hitboxManager.DisableAllHitboxes();
		currentState = state;
		_playerAnim.SetAnimationState(state);
	}

	void PerformLightPunch()
	{
		if(CheckCancelState(CancelState.FreeCancel))
		{
			SetActionState(ActionState.LightPunch);
		}
	}

	public void PerformHeavyPunch()
	{
		if(CheckCancelState(CancelState.HeavyCancel))
		{
			CancelAttackRecovery();
			SetActionState(ActionState.HeavyPunch);
		}
	}


	/// IHitboxResponder

	public override void HitSucceedEvent()
	{
		_hitboxManager.DisableAllHitboxes();
	}

	public override void ReceiveHit(AttackHit hit)
	{
		SetActionState(ActionState.HitStun);
		_playerMovement.AddMoveLock(MoveLockType.HitStun);
		Vector3 launchVel = Vector3.Normalize(hit.knockbackAngle) * hit.knockbackForce;
		launchVel.x *= hit.attacker.GetFacing().ToInt();
		GetComponent<Rigidbody>().velocity = launchVel;
		curHP = Mathf.Clamp(curHP - hit.damage, 0, maxHP);
		hpBar.SetHP(curHP);
		// TODO: scale with combo?
		stunTimer = hit.hitStun;
		cancelState = CancelState.FreeCancel;
	}
	
	

	/// Cancels ///

	public void SetCancelState(CancelState cs)
	{
		cancelState = cs;
	}

	bool CheckCancelState(CancelState state)
	{
		return ((cancelState == CancelState.FreeCancel) || (cancelState == state));
	}

	void CancelAttackRecovery()
	{
	}
	

	/// HP ///

	public int GetHP()
	{
		return curHP;
	}

	public bool IsDead()
	{
		return (curHP <= 0);
	}


	/// Locks ///

	public bool InputLocked()
	{
		return (inputLocks != 0);
	}

	public void AddInputLock(InputLockType lockType)
	{
		inputLocks |= (int)lockType;
	}

	public void RemoveInputLock(InputLockType lockType)
	{
		inputLocks &= ~(int)lockType;
	}

	void ClearInputLocks()
	{
		inputLocks = 0;
	}

	/// Input System ///

	public void MovePressed(InputAction.CallbackContext ctx)
	{
		Vector2 inputs = ctx.ReadValue<Vector2>();
		moveInputs = new Vector2Int( System.Math.Sign(inputs.x), System.Math.Sign(inputs.y));
	}

	public void LightPunchPressed(InputAction.CallbackContext ctx)
	{
		if(ctx.action.triggered && !InputLocked())
			PerformLightPunch();
	}

	public void HeavyPunchPressed(InputAction.CallbackContext ctx)
	{
		if(ctx.action.triggered && !InputLocked())
			PerformHeavyPunch();
	}

}
