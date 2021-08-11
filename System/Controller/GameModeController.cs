using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public enum TimeFreezeSouce{
	System 			= 0x0001,
	Pause			= 0x0002,
	HitStop			= 0x0004,

	ALL 			= 0xFFFF
}



public class GameModeController : MonoBehaviour
{	
	protected CombatParticles _combatParticles{
		get { return GetComponent<CombatParticles>(); }
	}

	protected static List<PlayerController> players = new List<PlayerController>();
	protected static List<PlayerController> winners = new List<PlayerController>();			// list of players who have killed a foe this frame

	////////////
	/// Hits ///
	////////////
	protected const float hitStopDuration =  0.125f;
	protected static List<AttackHit> currentHits = new List<AttackHit>();

	public static void RegisterAttackHit(EntityController attacker, Hurtbox target, int dmg, int hitstun, float kbForce, Vector2 kbAngle)
	{
		AttackHit hit = new AttackHit(attacker, target, dmg, hitstun, kbForce, kbAngle);
		foreach(AttackHit ah in currentHits)
		{
			if(ah.Matches(hit))
				return;
		}
		currentHits.Add(hit);
		//Debug.Log("Hit! " + currentHits.Count);
	}

	protected void ProcessHit(AttackHit hit)
	{
		hit.attacker.HitSucceedEvent();
		EntityController targetObj = hit.target.owner;
		if(targetObj == null){
			Debug.LogWarning("Hit a hurtbox without an owner!");
			return;
		}
		AudioController.instance.PlaySound("lightPunch");
		Vector3 burstDir = targetObj.transform.position - hit.attacker.transform.position;
		_combatParticles.CreateBurstAtPosition(targetObj.transform.position, burstDir, Color.white);
		
		StartCoroutine(HitStopCoroutine(hitStopDuration));
		//Vector2 shakeVec = new Vector2(hit.attacker.GetFacing().ToInt(), 0);
		Vector2 shakeVec = Vector2.down;
		GameController.gameCamera.ShakeCamera(1, shakeVec);
		HitPostProcess(hit);
	}


	protected IEnumerator HitStopCoroutine(float duration)
	{
		FreezeTime(TimeFreezeSouce.HitStop);
		yield return new WaitForSecondsRealtime(duration);
		UnfreezeTime(TimeFreezeSouce.HitStop);
		
	}

	protected virtual void HitPostProcess(AttackHit hit){}


	/// Pause ///

	protected static bool gameplayPaused = false;
	public static bool GameplayPaused{
		get { return gameplayPaused; }
	}

	public static void PauseGameplay()
	{
		gameplayPaused = true;
		FreezeTime(TimeFreezeSouce.Pause);
		foreach(PlayerController p in players)
		{
			p.AddInputLock(InputLockType.Pause);
		}
	}
	
	public static void UnpauseGameplay()
	{
		gameplayPaused = false;
		UnfreezeTime(TimeFreezeSouce.Pause);
		foreach(PlayerController p in players)
		{
			p.RemoveInputLock(InputLockType.Pause);
		}
	}

	/// Time Freeze ///
	protected static int timeFreeze;

	protected static void FreezeTime(TimeFreezeSouce source)
	{
		timeFreeze |= (int)source;
		TimeCheck();
	}

	protected static void UnfreezeTime(TimeFreezeSouce source)
	{
		timeFreeze &= ~(int)source;
		TimeCheck();
	}

	protected static void TimeCheck()
	{
		Time.timeScale = (timeFreeze == 0) ? 1 : 0;
	}
}
