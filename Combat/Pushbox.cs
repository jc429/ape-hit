using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushbox : CollisionBox
{
	public bool displayPushbox;

	private void Awake()
	{
		UpdateColBox(boxPos, boxSize);
	}


	private void OnValidate()
	{
		UpdateColBox(boxPos, boxSize);
	}


	private void OnDrawGizmos() {
		if(displayPushbox)
		{
			DrawColBox(Color.yellow);
		}
	}
}
