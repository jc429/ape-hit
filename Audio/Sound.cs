using UnityEngine;
using UnityEngine.Audio;



public enum TrackID{
	Null,
	Title,
	SinglePlayer,
	Versus,
	Options
}

[System.Serializable]
public class Sound {
	
	public string name;
	public TrackID trackID;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = 1;

	[Range(0.1f, 3f)]
	public float pitch = 1;

	public bool looping;

	[HideInInspector]
	public AudioSource source;
}
