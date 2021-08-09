using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
	[SerializeField]
	Transform hpMask;
	
	[SerializeField]
	HPParticles hpParticles;

	[SerializeField]
	Direction side;

	const int maxHP = 25;
	int curHP = 25;

	public void SetHP(int hp)
	{
		if(hp < curHP)
		{
			hpParticles?.CreateHPParticleChunk(side, hp, curHP);
		}
		curHP = Mathf.Clamp(hp, 0, maxHP);
		UpdateBarLength();
	}

	public void ResetHP()
	{
		SetHP(maxHP);
	}

	void UpdateBarLength()
	{
		hpMask.localScale = new Vector3(0.04f*(float)curHP, 1, 1);
		
	}
}
