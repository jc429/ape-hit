using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState 
{
	Idle,
	Walk,
	WalkBack,
	Jump,
	Fall,
	LightPunch,
	HeavyPunch,
	Uppercut,
	Lariat,
	AirLight,
	AirHeavy,
	HitStun,
	Launch,
	Knockdown,
	Grounded,
	Dead,
	Intro,
	Victory
}


public static class ActionStateExtensions {
	public static string GetStateAnimString(this ActionState actionState){
		switch(actionState){
			case ActionState.Idle:
				return "idle";
			case ActionState.Walk:
				return "walk";
			case ActionState.WalkBack:
				return "walk_back";
			case ActionState.Jump:
				return "jump";
			case ActionState.Fall:
				return "fall";
			case ActionState.LightPunch:
				return "punch_light";
			case ActionState.HeavyPunch:
				return "punch_heavy";
			case ActionState.Uppercut:
				return "uppercut";
			case ActionState.Lariat:
				return "lariat";
			case ActionState.AirLight:
				return "air_light";
			case ActionState.AirHeavy:
				return "air_heavy";
			case ActionState.HitStun:
				return "hitstun";
			case ActionState.Intro:
			case ActionState.Victory:
				return "victory";
			case ActionState.Dead:
				return "dead";
			default:
				return "Null";
		}
	}
}