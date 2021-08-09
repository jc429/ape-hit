using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	const float camDepth = -10;				// how far back the camera is in the z axis
	const float pixelWidth = 0.0625f;
	private Vector3 shakeOffset;

	private void Awake() {
		GameController.gameCamera = this;
	}

	private void FixedUpdate() {
		if(!GameController.GamePaused())
		{
			UpdateCamera();
		}
	}

	public void ShakeCamera(int force, Vector2 shakedir)
	{
		force = Mathf.Clamp(force, 0, 5);
		transform.localPosition = new Vector3(shakedir.x * pixelWidth * force, shakedir.y * pixelWidth * force, camDepth);
	}



	public void UpdateCamera()
	{
		UpdateCameraPos();
	}

	private void UpdateCameraPos()
	{
		Vector3 pos = transform.localPosition;
		if(pos != Vector3.zero)
		{
			if(pos.x > 0)
			{
				pos.x = Mathf.Max(pos.x - pixelWidth, 0);
			}
			else if(pos.x < 0)
			{
				pos.x = Mathf.Min(pos.x + pixelWidth, 0);
			}
			if(pos.y > 0)
			{
				pos.y = Mathf.Max(pos.y - pixelWidth, 0);
			}
			else if(pos.y < 0)
			{
				pos.y = Mathf.Min(pos.y + pixelWidth, 0);
			}
			transform.localPosition = pos;
		}
	}

}
