using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelParticle : MonoBehaviour
{
	Rigidbody _rigidbody{
		get { return GetComponent<Rigidbody>(); }
	}
	SpriteRenderer _spriteRenderer{
		get { return GetComponent<SpriteRenderer>(); }
	}
	public static readonly float PixelWidth = 0.0625f;
	const float lifeTime = 1.5f;
	float timeAlive = 0;


	private void FixedUpdate() {
		timeAlive += Time.fixedDeltaTime;
		if(timeAlive > lifeTime)
			Destroy(this.gameObject);
	}

	public void Launch(float force, Vector3 dir)
	{
		Vector3 launchVec = dir.normalized * force;
		_rigidbody.velocity = launchVec;
	}

	public void SetColor(Color c)
	{
		_spriteRenderer.color = c;
	}
}
