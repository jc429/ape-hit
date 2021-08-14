using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatParticles : MonoBehaviour
{
	[SerializeField]
	PixelParticle particlePrefab;


	public void CreateHitBurstAtPosition(Vector3 pos, Vector3 angle, Color color)
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


	public void CreateRingBurstAtPosition(Vector3 pos, Color color)
	{
		Vector3 vel = Vector3.right;
		for(int i = 0; i < 24; i++)
		{
			PixelParticle px = Instantiate(particlePrefab, pos, Quaternion.identity, transform) as PixelParticle;
			px.gameObject.layer = 9;
			px.GetComponent<SpriteRenderer>().sortingLayerName = "Players";
			px.GetComponent<SpriteRenderer>().sortingOrder = -100;
			px.SetColor(color);
			float force = 6;
			px.Launch(force, vel);
			vel = Quaternion.Euler(0, 0, -15) * vel;
		}
	}
}
