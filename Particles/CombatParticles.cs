using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatParticles : MonoBehaviour
{
	[SerializeField]
	PixelParticle particlePrefab;


	public void CreateBurstAtPosition(Vector3 pos, Vector3 angle, Color color)
	{
		for(int i = 0; i < 10; i++)
		{
			PixelParticle px = Instantiate(particlePrefab, pos, Quaternion.identity, transform) as PixelParticle;
			px.SetColor(color);

			Vector3 vel = angle.normalized;
			vel.y += i;
			float force = 6;
			px.Launch(force, vel);
		}
	}
}
