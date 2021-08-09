using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hurtbox : CollisionBox
{
	Color hurtboxColor = Color.green;
	public bool showHurtbox;


	private void Awake() {
		UpdateColBox(boxPos, boxSize);
	}


	public void SetHurtboxInfo(Vector2 pos, Vector2 size)
	{
		UpdateColBox(pos, size);
	}

	public void SetHurtboxInfo(float posX, float posY, float sizeX, float sizeY)
	{
		Vector2 pos = new Vector2(posX, posY);
		Vector2 size = new Vector2(sizeX, sizeY);
		UpdateColBox(pos, size);
	}


	private void OnDrawGizmos() {
		if(showHurtbox)
			DrawColBox(hurtboxColor);
	}

	private void OnValidate()
	{
		UpdateColBox(boxPos, boxSize);
	}
}
