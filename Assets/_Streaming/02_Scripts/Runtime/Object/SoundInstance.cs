using System;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;

public class SoundInstance : MonoBehaviour {

	private AudioSource source;
	
	void Awake() {
		source = GetComponent<AudioSource>();
	}



	public void SetAudioData(AudioClip clip, float volume) {
		source.clip = clip;
		source.volume = volume;
		source.Play();
	}

	public void Update() {
		
		if (!source.isPlaying)
			gameObject.SetActive(false);
			
	}
}
