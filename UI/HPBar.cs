using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
	SpriteRenderer _spriteRenderer {
		get { return GetComponent<SpriteRenderer>(); }
	}

	[SerializeField]
	Transform hpMask;
	
	[SerializeField]
	HPParticles hpParticles;

	[SerializeField]
	Direction side;

	[SerializeField]
	Gradient hpGradient;

	const int maxHP = 25;
	int curHP = 25;

	public void SetHP(int hp)
	{
		Color hpColor = hpGradient.Evaluate((float)curHP/(float)maxHP);
		_spriteRenderer.color = hpColor;
		if(hp < curHP)
		{
			hpParticles?.CreateHPParticleChunk(side, hp, curHP, hpColor);
		}
		curHP = Mathf.Clamp(hp, 0, maxHP);
		UpdateBarLength();
	}

	public void ResetHP()
	{
		SetHP(maxHP);
		Color hpColor = hpGradient.Evaluate(1);
		_spriteRenderer.color = hpColor;
	}

	void UpdateBarLength()
	{
		hpMask.localScale = new Vector3(0.04f*(float)curHP, 1, 1);
	}
	

	public void ShowHPBar()
	{
		gameObject.SetActive(true);
	}

	public void HideHPBar()
	{
		gameObject.SetActive(false);
	}

}
