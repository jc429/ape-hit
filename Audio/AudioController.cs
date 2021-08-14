using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;



public class AudioController : MonoBehaviour
{
	public bool muted;
	public Sound[] sounds;		// sound effects
	public Sound[] tracks;		// bgm tracks

	public static AudioController instance;

	private void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if(instance != this) {
			Destroy(this.gameObject);
			return;
		}

		foreach(Sound s in sounds){
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.looping;
		}
		foreach(Sound s in tracks){
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.looping;
		}
	}

	public void PlaySound(string name){
		if(muted)
			return;
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if(s != null){
			s.source.Play();
		}
		else{
			Debug.Log("Sound not found: " + name);
		}
	}

	public void StopAllSounds(){
		foreach(Sound s in sounds){
			s.source.Stop();
		}
	}

	public void PlayTrack(TrackID trackID){
		if(muted)
			return;
		Sound s = Array.Find(tracks, track => track.trackID == trackID);
		if(s != null){
			s.source.Play();
		}
		else{
			Debug.Log("Sound not found: " + name);
		}
	}

	public void StopAllTracks(){
		foreach(Sound s in tracks){
			s.source.Stop();
		}
	}
}
