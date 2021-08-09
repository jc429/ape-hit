using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoundMarker : MonoBehaviour
{
	public enum MarkerState{
		Blank		= 0,
		Victory		= 1,
		Perfect		= 2
	}

	SpriteRenderer _spriteRenderer{
		get { return GetComponent<SpriteRenderer>(); }
	}

	[SerializeField]
	Sprite[] markerSprites;

	public void SetMarkerState(MarkerState m)
	{
		_spriteRenderer.sprite = markerSprites[(int)m];
	}

	public void ResetMarker()
	{
		SetMarkerState(MarkerState.Blank);
	}
}
