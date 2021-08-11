using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnim : MonoBehaviour
{
	SpriteRenderer _spriteRenderer{
		get { return GetComponentInChildren<SpriteRenderer>(); }
	}
	Animator _animator{
		get { return GetComponentInChildren<Animator>(); }
	}
	PlayerHitboxManager _hitboxManager{
		get { return GetComponentInChildren<PlayerHitboxManager>(); }
	}

	ActionState currentState;
	public ActionState CurrentState{
		get { return currentState; }
	}

	public void SetAnimationState(ActionState actionState)
	{
		_hitboxManager.SetHurtboxState(actionState);
		string str = actionState.GetStateAnimString();
		if(str != "Null")
			_animator.Play(str);
		currentState = actionState;
	}
	
	public void SetGroundedFlag(bool grounded)
	{
		_animator.SetBool("isGrounded", grounded);
	}

}
