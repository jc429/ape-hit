using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
	public Transform cameraTarget;
	[Range(0,1)]
	public float parallaxStrength;
	[SerializeField]
	float snapInterval;

	private void FixedUpdate() {
		if(cameraTarget == null)
			return;
		Vector3 camPos = cameraTarget.position;
		Vector3 pos = transform.position;
		pos.x = camPos.x * parallaxStrength;

		if(pos.x - camPos.x >= snapInterval)
		{
			pos.x -= snapInterval;
		}
		
		if(camPos.x - pos.x >= snapInterval)
		{
			pos.x += snapInterval;
		}

		transform.position = pos;
	}
}
