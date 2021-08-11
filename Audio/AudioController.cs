using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
	public Sound[] sounds;

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
	}

	public void PlaySound(string name){
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
}
