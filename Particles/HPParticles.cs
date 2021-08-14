using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPParticles : MonoBehaviour
{
	[SerializeField]
	PixelParticle particlePrefab;


	public void CreateHPParticleChunk(Direction side, int lowHP, int highHP, Color color)
	{
		Vector3 basePos = new Vector3(0.21875f, 1.59375f);
		for(int i = lowHP; i < highHP; i++)
		{
			for(int j = 0; j < 3; j++)
			{
				if(i < 3 && j > i)
					continue;
				Vector3 offset = new Vector3(i*PixelParticle.PixelWidth, j*PixelParticle.PixelWidth);
				Vector3 pos = new Vector3((basePos.x + offset.x)*side.ToInt(), basePos.y + offset.y, -1);
				PixelParticle px = Instantiate(particlePrefab, pos, Quaternion.identity, transform) as PixelParticle;
				px.SetColor(color);
				
				Vector3 vel = new Vector3();
				vel.x = (1 + (i-lowHP))*side.ToInt();
				vel.y = j;

				float force = 0.25f + (0.25f*i);
				px.Launch(force, vel);
			}
		}
	}
}
