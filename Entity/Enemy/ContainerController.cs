using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : EntityController
{
	[SerializeField][Range(1,50)]
	int maxHP = 6;
	public int currentHP;

	[SerializeField]
	GameObject contents;
	
	private void Awake() {
		currentHP = maxHP;
	}

	public override void ReceiveHit(AttackHit hit)
	{
		currentHP = Mathf.Clamp(currentHP - hit.damage, 0, maxHP);
		if(currentHP <= 0)
		{
			Break();
		}
	}

	void Break()
	{
		if(contents != null)
		{
			GameObject spawn = Instantiate(contents, transform.position, Quaternion.identity, transform.parent);
			if(spawn.GetComponent<Rigidbody>() != null)
				spawn.GetComponent<Rigidbody>().velocity = Vector3.up * 5f;
		}
		Destroy(this.gameObject);
	}
}
