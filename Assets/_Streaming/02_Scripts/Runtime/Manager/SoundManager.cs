using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {

	[SerializeField, Range(0, 1)] private float masterVolume;
	[SerializeField] private GameObject soundInstance;
	[Header("[ Audio Library ]")]
	[Space(5)]
	[SerializeField] private AudioClip gunSound;
	[Space(5)]
	[SerializeField] private AudioClip[] blip;
	[Space(5)]
	[SerializeField] private AudioClip[] boom;
	[Space(5)]
	[SerializeField] private AudioClip[] zap;

	private void PlaySound(AudioClip clip, float volume) {
		var instance = PoolManager.Instance.InstantiateSound(soundInstance);
		instance.GetComponent<SoundInstance>().SetAudioData(clip, volume * masterVolume);
	}



	public void PlayGunSound(float volume) {
		PlaySound(gunSound, volume);
	}

	public void PlayBlip(int index, float volume) {
		PlaySound(blip[index], volume);
	}

	public void PlayBoom(int index, float volume) {
		PlaySound(boom[index], volume);
	}

	public void PlayZap(int index, float volume) {
		PlaySound(zap[index], volume);
	}
}
