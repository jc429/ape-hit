using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public enum CancelState{
	FreeCancel,
	NoCancel,
	HeavyCancel
}

[SelectionBase]
public class PlayerController : MonoBehaviour
{
	SpriteRenderer _spriteRenderer{
		get { return GetComponentInChildren<SpriteRenderer>(); }
	}
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
	private bool inputLocked;
	CancelState cancelState;
	private Direction facingDir = Direction.E;
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

	public void ResetPlayer()
	{
		stunTimer = 0;
		curHP = maxHP;
		SetActionState(ActionState.Idle);
		cancelState = CancelState.FreeCancel;
		_playerMovement.SetMovementLocked(false);
	}
	
	/// Updates ///

	private void FixedUpdate()
	{
		if(stunTimer > 0)
		{
			stunTimer--;
			if(stunTimer == 0)
			{
				_playerMovement.SetMovementLocked(false);
			}
		}
		else
		{
			_playerMovement.GroundCheck();
			if(!inputLocked)
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
		//_spriteRenderer.flipX = (gameObject.transform.position.x > opponent.position.x);
		SetFacing((gameObject.transform.position.x > opponent.transform.position.x) ? Direction.W : Direction.E);
	}

	public Direction GetFacing()
	{
		return facingDir;
	}

	public void SetFacing(Direction dir)
	{
		facingDir = dir;
		Vector3 rot = Vector3.zero;
		if(facingDir == Direction.W)
			rot.y = 180;
		transform.rotation = Quaternion.Euler(rot);
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

	public void ReceiveHit(AttackHit hit)
	{
		SetActionState(ActionState.HitStun);
		_playerMovement.SetMovementLocked(true);
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
		return curHP <= 0;
	}


	/// Locks ///


	public void SetInputsLocked(bool locked)
	{
		inputLocked = locked;
	}

	/// Input System ///

	public void MovePressed(InputAction.CallbackContext ctx)
	{
		Vector2 inputs = ctx.ReadValue<Vector2>();
		moveInputs = new Vector2Int( System.Math.Sign(inputs.x), System.Math.Sign(inputs.y));
	}

	public void LightPunchPressed(InputAction.CallbackContext ctx)
	{
		if(ctx.action.triggered && !inputLocked)
			PerformLightPunch();
	}

	public void HeavyPunchPressed(InputAction.CallbackContext ctx)
	{
		if(ctx.action.triggered && !inputLocked)
			PerformHeavyPunch();
	}

}
