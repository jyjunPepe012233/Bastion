using System;
using UnityEditor;
using UnityEngine;

public class Lazer : MonoBehaviour {

	[SerializeField] private float lifeTime;
	[SerializeField] private AnimationCurve width;

	private float time;

	[SerializeField] private LineRenderer line;

	
	
	public void InIt(float lifeTime, AnimationCurve width, Vector3 startPosition, Vector3 endPosition) {
		
		this.lifeTime = lifeTime;
		this.width = width;
		
		time = 0;

		line = GetComponent<LineRenderer>();
		line.SetPosition(0, startPosition);
		line.SetPosition(1, endPosition);
	}



	void Update() {

		time += Time.deltaTime;
		if (time > lifeTime) gameObject.SetActive(false);

		line.startWidth = width.Evaluate(time / lifeTime / 2);
		line.endWidth = width.Evaluate(time / lifeTime);

		line.positionCount = 2;
	}
}
