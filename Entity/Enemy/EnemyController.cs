using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
	[SerializeField][Range(1,50)]
	int maxHP = 6;
	public int currentHP;
	
	private void Awake() {
		currentHP = maxHP;
	}

	public override void ReceiveHit(AttackHit hit)
	{
		currentHP = Mathf.Clamp(currentHP - hit.damage, 0, maxHP);
		if(currentHP <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		Destroy(this.gameObject);
	}
}
