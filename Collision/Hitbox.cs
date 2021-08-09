using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderState {
	Inactive,
	Active,
	Colliding
}

public class Hitbox : CollisionBox
{
	public bool showHitbox;
	public LayerMask collisionMask;

	private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);
	private Color activeColor = new Color(1f, 0.0f, 0.0f);
	private Color collidingColor = new Color(1f, 0.4f, 0.8f);

	private ColliderState colliderState;
	private IHitboxResponder collisionResponder = null;

	int baseDamage;
	int baseHitStun;
	float kbForce;
	Vector2 kbAngle;


	private void Awake()
	{
		UpdateColBox(boxPos, boxSize);
	}





	public void HitboxUpdate()
	{
		if(colliderState == ColliderState.Inactive)
			return;

		UpdateColBox(boxPos, boxSize);

		int facing = owner.GetFacing().ToInt();
		Vector3 pos = transform.position + new Vector3(boxPos.x * facing, boxPos.y, 0);
		Quaternion rotation = Quaternion.identity;
		Collider[] colliders = Physics.OverlapBox(pos, halfSize, rotation, collisionMask);

		bool hit = false;
		foreach(Collider col in colliders)
		{
			//collisionResponder?.CollisionWith(col);
			Hurtbox hb = col.gameObject.GetComponent<Hurtbox>();
			if(hb.owner != owner)
			{
				CombatManager.RegisterAttackHit(owner, hb, baseDamage, baseHitStun, kbForce, kbAngle);
				hit = true;
			}
		}
		colliderState = hit ? ColliderState.Colliding : ColliderState.Active;
	}

	public void SetCollisionResponder(IHitboxResponder responder) {
		collisionResponder = responder;
	}

	public void SetHitboxInfo(Vector2 pos, Vector2 size)
	{
		UpdateColBox(pos, size);
	}

	public void SetHitboxInfo(float posX, float posY, float sizeX, float sizeY)
	{
		Vector2 pos = new Vector2(posX, posY);
		Vector2 size = new Vector2(sizeX, sizeY);
		UpdateColBox(pos, size);
	}

	public void SetHitInfo(int damage, int hitstun, float force, Vector2 angle)
	{
		baseDamage = damage;
		baseHitStun = hitstun;
		kbForce = force;
		kbAngle = angle;
	}
	
	public void ClearHitboxInfo()
	{
		UpdateColBox(Vector2.zero, new Vector2(0.2f, 0.2f));
		baseDamage = 0;
		baseHitStun = 0;
		kbForce = 0;
		kbAngle = Vector2.zero;
	}
	
	public void SetHitboxActive()
	{
		colliderState = ColliderState.Active;
	}

	public void SetHitboxInactive()
	{
		colliderState = ColliderState.Inactive;
	}

	private void OnValidate()
	{
		UpdateColBox(boxPos, boxSize);
	}

	/// Drawing ///

	private void OnDrawGizmos() {
		if(showHitbox)
			DrawColBox(GetHitboxColor());
	}

	private Color GetHitboxColor() {
		switch(colliderState) {
			case ColliderState.Active:
				return activeColor;
			case ColliderState.Colliding:
				return collidingColor;
			case ColliderState.Inactive:
			default:
				return inactiveColor;
		}
	}

}



