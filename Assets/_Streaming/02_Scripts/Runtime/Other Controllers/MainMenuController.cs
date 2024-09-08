using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MainMenuController : MonoBehaviour {

	[SerializeField] private Transform camera;
	[SerializeField] private float cameraMoveIntensity;
	[SerializeField] private float cameraMoveSpeed;
	[Space(5)]
	[SerializeField] private ParticleSystem[] jetpackParticle;
	
	private Vector3 camOriginPos;
	private Vector3 camTargetPos;
	

	void Start() {
		
		SoundManager.Instance.PlayBlip(0, 1);

		foreach (ParticleSystem ps in jetpackParticle) {
			ps.Simulate(3);
		}

		camOriginPos = camera.position;
		
		UIManager.Instance.SetCanvas(CanvasType.MainMenu);
		UIManager.Instance.ChangeMenu(0);
	}



	void Update() {

		// 마우스 움직임에 따른 카메라 이동
		Vector3 mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
		camTargetPos = camOriginPos + (mousePosition * cameraMoveIntensity);

		camera.position = Vector3.Lerp(camera.position, camTargetPos, cameraMoveSpeed * Time.deltaTime);
		
	}
}
