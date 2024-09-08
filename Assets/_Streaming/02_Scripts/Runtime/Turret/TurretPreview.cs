using System;
using UnityEngine;

public class TurretPreview : MonoBehaviour {
	
	private bool isColliding;
	private Renderer renderer;
	private Collider collider;
	
	public bool IsColliding { get => isColliding; }
	


	private void Awake() {
		renderer = GetComponent<Renderer>();
		collider = GetComponent<Collider>();
	}
	
	

	private void OnTriggerEnter(Collider other) {
		isColliding = true;
	}
	
	

	private void OnTriggerExit(Collider other) {
		isColliding = false;
	}



	public void SetColor(Color color) {

		renderer.material.color = color;

	}


	private void OnDrawGizmos() {
		
	}
}
