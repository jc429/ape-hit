using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
	Image _image{
		get { return GetComponent<Image>(); }
	}

	const float cycleSpeed = 1.6f;
	Color c = Color.white;
	static float offset;

	private void Update() {
		float time = (Time.time+2) * cycleSpeed;

		if(time < Mathf.PI)
			c.a = 0;
		else
			c.a = Mathf.Max((Mathf.Cos(time)/2)+0.5f, 0);
		_image.color = c;
	}

	private void OnEnable() {
	}
}
