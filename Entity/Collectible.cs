using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{




	void Collect(PlayerController player)
	{
		Destroy(this.gameObject);
	}

	private void OnTriggerEnter(Collider other) {
		PlayerController player = other.GetComponentInParent<PlayerController>();
		if(player != null)
		{
			Collect(player);
		}
	}
}
