using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum HurtboxID
{
	Body = 0,
	Head = 1
}

public enum AttackID
{
	LightPunch = 0,
	HeavyPunch = 1,
	Uppercut = 2,
	Lariat = 3,
	AirLight = 4,
	AirHeavy = 5,
}


public class PlayerHitboxManager : MonoBehaviour
{
	

	public Hurtbox[] hurtboxes;
	public Hitbox[] hitboxes;
	
	public void CombatUpdate()
	{
		foreach(Hitbox hb in hitboxes)
		{
			hb.HitboxUpdate();
		}
	}



	public void DisableAllHurtboxes()
	{
		foreach(Hurtbox hb in hurtboxes)
		{
		
		}
	}

	public void SetHurtboxState(ActionState actionState)
	{
		switch(actionState){
			case ActionState.Idle:
			case ActionState.Walk:
			case ActionState.WalkBack:
				hurtboxes[(int)HurtboxID.Body].SetHurtboxInfo(0.0f, -0.1f, 1.0f, 0.8f);
				hurtboxes[(int)HurtboxID.Head].SetHurtboxInfo(0.4f, 0.2f, 0.4f, 0.4f);
				break;
			case ActionState.Jump:
			case ActionState.Fall:
				hurtboxes[(int)HurtboxID.Body].SetHurtboxInfo(0.0f, 0.0f, 0.8f, 0.8f);
				hurtboxes[(int)HurtboxID.Head].SetHurtboxInfo(0.2f, 0.6f, 0.4f, 0.4f);
				break;
			case ActionState.LightPunch:
				hurtboxes[(int)HurtboxID.Body].SetHurtboxInfo(-0.1f, 0.0f, 0.5f, 0.9f);
				hurtboxes[(int)HurtboxID.Head].SetHurtboxInfo(0.0f, 0.5f, 0.4f, 0.5f);
				break;
			case ActionState.HeavyPunch:
				hurtboxes[(int)HurtboxID.Body].SetHurtboxInfo(-0.1f, 0.0f, 0.5f, 0.9f);
				hurtboxes[(int)HurtboxID.Head].SetHurtboxInfo(0.0f, 0.5f, 0.4f, 0.5f);
				break;
			case ActionState.Victory:
			case ActionState.Lariat:
				hurtboxes[(int)HurtboxID.Body].SetHurtboxInfo(0.0f, -0.1f, 1.0f, 0.8f);
				hurtboxes[(int)HurtboxID.Head].SetHurtboxInfo(0.0f, 0.5f, 0.4f, 0.4f);
				break;
			case ActionState.AirLight:
			case ActionState.AirHeavy:
				hurtboxes[(int)HurtboxID.Body].SetHurtboxInfo(0.0f, 0.0f, 0.6f, 0.8f);
				hurtboxes[(int)HurtboxID.Head].SetHurtboxInfo(0.0f, 0.0f, 0.4f, 0.4f);
				break;
			default:
				DisableAllHurtboxes();
				break;
		}
	}


	public void DisableAllHitboxes()
	{
		foreach(Hitbox hb in hitboxes)
		{
			hb.ClearHitboxInfo();
			hb.SetHitboxInactive();
		}
	}


	public void SetHitboxState(AttackID attackID)
	{
		switch(attackID){
			case AttackID.LightPunch:
				hitboxes[0].SetHitboxInfo(0.35f, 0.25f, 0.8f, 0.4f);
				hitboxes[0].SetHitInfo(1, 20, 2f, new Vector2(1,1f));
				hitboxes[0].SetHitboxActive();
				break;
			case AttackID.HeavyPunch:
				hitboxes[0].SetHitboxInfo(0.35f, 0.25f, 0.8f, 0.4f);
				hitboxes[0].SetHitInfo(3, 30, 5f, new Vector2(1,3));
				hitboxes[0].SetHitboxActive();
				break;
			case AttackID.Uppercut:
				hitboxes[0].SetHitboxInfo(0.35f, 0.25f, 0.8f, 0.4f);
				hitboxes[0].SetHitInfo(1, 20, 6f, new Vector2(0.2f,1));
				hitboxes[0].SetHitboxActive();
				break;
			case AttackID.Lariat:
				hitboxes[0].SetHitboxInfo(0.0f, 0.3f, 2f, 0.4f);
				hitboxes[0].SetHitInfo(2, 20, 6f, new Vector2(1,0.1f));
				hitboxes[0].SetHitboxActive();
				break;
			case AttackID.AirLight:
				hitboxes[0].SetHitboxInfo(0.25f, 0.2f, 1f, 0.8f);
				hitboxes[0].SetHitInfo(1, 15, 3f, new Vector2(1,1f));
				hitboxes[0].SetHitboxActive();
				break;
			case AttackID.AirHeavy:
				hitboxes[0].SetHitboxInfo(0.25f, -0.4f, 1f, 0.8f);
				hitboxes[0].SetHitInfo(2, 25, 4f, new Vector2(1,1f));
				hitboxes[0].SetHitboxActive();
				break;
			default:
				DisableAllHitboxes();
				break;
		}
	}
}
