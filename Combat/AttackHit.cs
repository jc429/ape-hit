using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit
{
	public EntityController attacker;
	public Hurtbox target;
	public int damage;
	public int hitStun;
	public float knockbackForce;
	public Vector2 knockbackAngle;

	public AttackHit(EntityController attacker, Hurtbox target, int dmg, int hitstun, float kb_force, Vector2 kb_angle)
	{
		this.attacker = attacker;
		this.target = target;
		this.damage = dmg;
		this.hitStun = hitstun;
		this.knockbackForce = kb_force;
		this.knockbackAngle = kb_angle;
	}

	public bool Matches(AttackHit other)
	{
		return(this.attacker == other.attacker && this.target.owner == other.target.owner);
	}
}