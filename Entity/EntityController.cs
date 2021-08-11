using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour, IHitboxResponder
{
	protected SpriteRenderer _spriteRenderer{
		get { return GetComponentInChildren<SpriteRenderer>(); }
	}

	protected Direction facingDir = Direction.E;
	
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

	public virtual void HitSucceedEvent(){}
	public virtual void ReceiveHit(AttackHit hit){}
}
