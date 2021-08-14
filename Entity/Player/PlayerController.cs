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

	public bool isAI;

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
			if(stunTimer <= 0)
			{
				stunTimer = 0;
				RemoveInputLock(InputLockType.HitStun);
				_playerMovement.RemoveMoveLock(MoveLockType.HitStun);
				_playerMovement.GroundCheck();
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

	public void SetSortingOrder(int priority)
	{
		_spriteRenderer.sortingOrder = priority;
	}



	/// Actions ///

	public void SetActionState(ActionState state)
	{
		_hitboxManager.DisableAllHitboxes();
		currentState = state;
		_playerAnim.SetAnimationState(state);
	}

	void PerformLightAttack()
	{
		if(CheckCancelState(CancelState.FreeCancel))
		{
			if(moveInputs.y < 0)
				SetActionState(ActionState.Uppercut);
			else if(!_playerMovement.IsGrounded())
				SetActionState(ActionState.AirLight);
			else
				SetActionState(ActionState.LightPunch);
		}
	}

	void PerformHeavyAttack()
	{
		if(CheckCancelState(CancelState.HeavyCancel))
		{
			CancelAttackRecovery();
			if(moveInputs.y < 0)
				SetActionState(ActionState.Lariat);
			else if(!_playerMovement.IsGrounded())
				SetActionState(ActionState.AirHeavy);
			else
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
		_playerMovement.RemoveMoveLock(MoveLockType.Animation);
		_playerMovement.AddMoveLock(MoveLockType.HitStun);
		_playerMovement.ForceNotGrounded();
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
		if(isAI)
			return;
		Vector2 inputs = ctx.ReadValue<Vector2>();
		moveInputs = new Vector2Int( System.Math.Sign(inputs.x), System.Math.Sign(inputs.y));
	}

	public void LightPunchPressed(InputAction.CallbackContext ctx)
	{
		if(isAI)
			return;
		if(ctx.action.triggered && !InputLocked())
			PerformLightAttack();
	}

	public void HeavyPunchPressed(InputAction.CallbackContext ctx)
	{
		if(isAI)
			return;
		if(ctx.action.triggered && !InputLocked())
			PerformHeavyAttack();
	}















	WaitForSeconds aiTickWait = new WaitForSeconds(0.75f);
	Coroutine aiController;
	public void StartAIController()
	{
		if(!isAI)
			return;
		if(aiController != null)
		{
			EndAIController();
		}
		aiController = StartCoroutine(AIControl());
	}

	IEnumerator AIControl()
	{
		if(opponent == null)
		{
			Debug.LogWarning("No opponent found!");
		}
		while(true)
		{
			if(InputLocked())
			{
				yield return aiTickWait;
				continue;
			}
			
			float walkThreshold = 1;
			if(transform.position.x - opponent.transform.position.x > walkThreshold)
			{
				moveInputs = new Vector2Int(-1, 0);
			}
			else if(opponent.transform.position.x - transform.position.x > walkThreshold)
			{
				moveInputs = new Vector2Int(1, 0);
			}
			else
			{
				moveInputs = new Vector2Int(0, 0);
			}

			if(_playerMovement.IsGrounded() && CheckCancelState(CancelState.FreeCancel))
			{

				// randomly choose an action
				int numActions = 8;
				int r = Random.Range(0,numActions);
				switch(r)
				{	
					case 4:
						int x = Random.Range(0,3);
						_playerMovement.Jump(x-1);
						break;
					case 3:
						SetActionState(ActionState.Lariat);
						break;
					case 2:
						SetActionState(_playerMovement.IsGrounded() ? ActionState.HeavyPunch : ActionState.AirHeavy);
						break;
					case 1:
						SetActionState(ActionState.Uppercut);
						break;
					case 0:
						SetActionState(_playerMovement.IsGrounded() ? ActionState.LightPunch : ActionState.AirLight);
						break;
					default:
						break;
				}
			}

			yield return aiTickWait;
		}
	}


	public void EndAIController()
	{
		StopCoroutine(aiController);
		aiController = null;
	}

}
