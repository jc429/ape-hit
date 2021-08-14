using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;





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
	protected static WaitForSecondsRealtime hitStop = new WaitForSecondsRealtime(hitStopDuration);

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
		targetObj.ReceiveHit(hit);
		if(hit.knockbackForce > 5)
			AudioController.instance.PlaySound("heavyHit");
		else
			AudioController.instance.PlaySound("lightHit");
		Vector3 burstDir = targetObj.transform.position - hit.attacker.transform.position;
		_combatParticles?.CreateHitBurstAtPosition(targetObj.transform.position, burstDir, Color.white);
		
		StartCoroutine(HitStopCoroutine());
		//Vector2 shakeVec = new Vector2(hit.attacker.GetFacing().ToInt(), 0);
		Vector2 shakeVec = Vector2.down;
		GameController.gameCamera.ShakeCamera(1, shakeVec);
		HitPostProcess(hit);
	}


	protected IEnumerator HitStopCoroutine()
	{
		FreezeTime(TimeFreezeSouce.HitStop);
		yield return hitStop;
		UnfreezeTime(TimeFreezeSouce.HitStop);
	}

	protected virtual void HitPostProcess(AttackHit hit){}


	/// Pause Menu ///

	
	[SerializeField]
	protected MenuController pauseMenu;
	[SerializeField]
	protected MenuController optionsMenu;
	[SerializeField]
	protected PaletteSelector[] paletteSelectors;

	const float holdThreshold = 1f;
	bool pauseDown = false;
	protected float pauseInputHoldTime = 0;

	public void ConfirmButtonPressed(InputAction.CallbackContext ctx)
	{
		if(ctx.performed && optionsMenu.GetMenuState() == MenuState.Open)
		{
			optionsMenu.SelectButton();
		}
		else if(ctx.performed && optionsMenu.GetMenuState() == MenuState.Closed && pauseMenu.GetMenuState() == MenuState.Open)
		{
			pauseMenu.SelectButton();
		}
		else if(ctx.performed)
		{
			StartButtonPressed(ctx);
			PausePressed();
		}
		if(ctx.started)
		{
		}
		if(ctx.canceled)
		{
			PauseReleased();
		}
	}
	
	public void CancelButtonPressed(InputAction.CallbackContext ctx)
	{
		if(ctx.canceled)
			return;
		if(ctx.performed && optionsMenu.GetMenuState() == MenuState.Open)
		{
			optionsMenu.StartCloseMenu();
		}
		else if(ctx.performed && optionsMenu.GetMenuState() == MenuState.Closed && pauseMenu.GetMenuState() == MenuState.Open)
		{
			pauseMenu.StartCloseMenu();
		}
	}

	public void DirectionPressed(InputAction.CallbackContext ctx)
	{
		if(!ctx.performed)
			return;
		if(optionsMenu.GetMenuState() == MenuState.Open)
		{
			optionsMenu.MoveCursor(ctx.ReadValue<Vector2>());
		}
		else if(optionsMenu.GetMenuState() == MenuState.Closed && pauseMenu.GetMenuState() == MenuState.Open)
		{
			pauseMenu.MoveCursor(ctx.ReadValue<Vector2>());
		}
	}

	protected virtual void StartButtonPressed(InputAction.CallbackContext ctx){}
	protected virtual void StartButtonLongPress(){}

	void PausePressed()
	{
		pauseDown = true;
		pauseInputHoldTime = 0;
	}

	void PauseReleased()
	{
		pauseDown = false;
		pauseInputHoldTime = 0;
	}

	private void Update() {
		if(pauseDown)
		{
			pauseInputHoldTime += Time.deltaTime;
			if(pauseInputHoldTime >= holdThreshold)
			{
				pauseDown = false;
				pauseInputHoldTime = 0;
				StartButtonLongPress();
			}
		}
	}

	/// Palette Swapping ///


	
	public virtual void SavePalettes()
	{
		PaletteIndex p1pal = paletteSelectors[0].GetPalette();
		PaletteIndex p2pal = paletteSelectors[1].GetPalette();
		GameController.SetPalettes(p1pal, p2pal);
	}





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
