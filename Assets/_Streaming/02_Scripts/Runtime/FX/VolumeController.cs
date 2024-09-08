using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class VolumeController : MonoBehaviour {

	private Volume volume;

	private Vignette vignette;

	private float hitFxIntensity;
	

	void Awake() {

		volume = GetComponent<Volume>();
		volume.profile.TryGet(out vignette);

	}
	
	

	void Update() {

		if (hitFxIntensity > 0) 
			hitFxIntensity -= Time.deltaTime;

		vignette.intensity.value = hitFxIntensity;

	}
	
	

	public void HitEffect() {

		hitFxIntensity = 0.4f;
	}
}
