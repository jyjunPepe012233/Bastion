using System;
using UnityEditor;
using UnityEngine;

public enum ImpactType {
	Default,
	Enemy
}
public class Impact : MonoBehaviour {

	[SerializeField] private float lifeTime;
	[Space(5)]
	[SerializeField] private GameObject default_;
	[SerializeField] private GameObject enemy;

	private GameObject currentImpact;
	private float time;

	
	
	public void Init(float lifeTime, ImpactType type) {
		
		this.lifeTime = lifeTime;
		
		time = 0;

		switch (type) {
			case (ImpactType.Default):
				currentImpact = default_;
				break;
			
			case (ImpactType.Enemy):
				currentImpact = enemy;
				break;
		}
		currentImpact.SetActive(true);
	}



	void Update() {

		time += Time.deltaTime;
		if (time > lifeTime) gameObject.SetActive(false);
		
	}
	
	

	private void OnDisable() {
		
		if (currentImpact != null)
			currentImpact.SetActive(false);
		
	}
}
