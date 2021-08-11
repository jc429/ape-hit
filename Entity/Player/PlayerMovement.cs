using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum MoveLockType{
	Pause			= 0x0001,
	Dead			= 0x0002,
	HitStun			= 0x0004,
	Animation		= 0x0008,

	ALL				= 0xFFFF
}

public class PlayerMovement : MonoBehaviour
{
	PlayerController _playerController{
		get { return GetComponent<PlayerController>(); }
	}
	PlayerAnim _playerAnim{
		get { return GetComponent<PlayerAnim>(); }
	}
	Rigidbody _rigidbody{
		get { return GetComponent<Rigidbody>(); }
	}
	Direction movingDir;

	const float TIMESTEP = 0.016667f;
	const float walkSpeed = TIMESTEP * 2f;
	const float halfWidth = 0.5f;
	LayerMask stageMask = 1<<3;

	private bool grounded;
	private int moveLocks;



	private void Awake()
	{
		movingDir = Direction.None;
	}
	
	public void UpdateMovement(Vector2Int dir)
	{
		if(!MovementLocked() && IsGrounded())
		{
			// check for jump input
			if(dir.y > 0)
			{
				Jump(dir.x);
				return;
			}
			
			if(dir.x == 0){
				movingDir = Direction.None;
				if(grounded)
				{
					_playerController.SetActionState(ActionState.Idle);
				}
				return;
			}
			else{
				movingDir = (dir.x > 0) ? Direction.E : Direction.W;
				_playerController.SetActionState(movingDir == _playerController.GetFacing() ? ActionState.Walk : ActionState.WalkBack);

				Vector3 pos = transform.position;

				float wallCast = walkSpeed;
				RaycastHit wallHitInfo;
				if(Physics.Raycast(pos, Vector3.right * dir.x, out wallHitInfo, halfWidth+wallCast, stageMask))
				{
					wallCast = wallHitInfo.distance - halfWidth;
					Debug.DrawRay(pos, Vector3.right * dir.x * (halfWidth+wallCast), Color.red);
				}
				else
				{
					Debug.DrawRay(pos, Vector3.right * dir.x * (halfWidth+wallCast), Color.green);
				}
				pos.x += dir.x * wallCast;
				transform.position = pos;
			}	
		}
		// check if falling
		if(_playerAnim.CurrentState == ActionState.Jump && _rigidbody.velocity.y < 0)
		{
			_playerAnim.SetAnimationState(ActionState.Fall);
		}
	}

	public void Jump(int dir)
	{
		if(!grounded)
			return;
		Vector3 jumpVec = Vector3.up * 5;
		jumpVec.x = dir;
		_rigidbody.velocity = jumpVec;
		_playerAnim.SetAnimationState(ActionState.Jump);
		SetGrounded(false);
	}



	/////////////////////
	/// Ground Checks ///
	/////////////////////

	public void GroundCheck()
	{
		float raylen = halfWidth+0.1f;
		if(_rigidbody.velocity.y <= 0)
		{
			SetGrounded(Physics.Raycast(transform.position, Vector3.down, raylen, stageMask));
		}
		Debug.DrawRay(transform.position, Vector3.down * raylen, (grounded ? Color.yellow : Color.black));
	}

	void SetGrounded(bool grd)
	{
		grounded = grd;
		_playerAnim.SetGroundedFlag(grd);
	}

	public bool IsGrounded()
	{
		return grounded;
	}

	
	/// Locks ///

	public bool MovementLocked()
	{
		return (moveLocks != 0);
	}

	public void AddMoveLock(MoveLockType lockType)
	{
		moveLocks |= (int)lockType;
	}

	public void RemoveMoveLock(MoveLockType lockType)
	{
		moveLocks &= ~(int)lockType;
	}

	public void ClearMovementLocks()
	{
		moveLocks = 0;
	}


}
