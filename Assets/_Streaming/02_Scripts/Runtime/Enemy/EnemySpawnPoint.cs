using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemySpawnPoint : MonoBehaviour {

	[SerializeField] private Vector2 scale;

	private Quaternion rotation;
	private Vector3 halfScale;
	


	private void OnValidate() {
		rotation = transform.rotation;
		halfScale = scale / 2;
	}



	public Vector3? GetSpawnPosition() {

		Vector3 rayOrigin;
		rayOrigin.x = Random.Range(-halfScale.x, halfScale.x);
		rayOrigin.y = 0;
		rayOrigin.z = Random.Range(-halfScale.y, halfScale.y);
		
		Ray ray = new Ray(transform.TransformPoint(rayOrigin), Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
			return hitInfo.point;
		
		return null; // 레이케이스트에 닿지 않음. EnemySpawner에서 에러메세지를 호출할 것임
	}

	

	private void OnDrawGizmosSelected() {


		Vector3 vertexFL = transform.TransformPoint(new Vector3(-halfScale.x, 0, halfScale.y));
		Vector3 vertexFR = transform.TransformPoint(new Vector3(halfScale.x, 0, halfScale.y));
		Vector3 vertexBL = transform.TransformPoint(new Vector3(-halfScale.x, 0, -halfScale.y));
		Vector3 vertexBR = transform.TransformPoint(new Vector3(halfScale.x, 0, -halfScale.y));

		Gizmos.color = Color.white;
		
		Gizmos.DrawLine(vertexFL, vertexFR);
		Gizmos.DrawLine(vertexFR, vertexBR);
		Gizmos.DrawLine(vertexBR, vertexBL);
		Gizmos.DrawLine(vertexBL, vertexFL);
		
		Gizmos.DrawLine(vertexFL, vertexBR);
		Gizmos.DrawLine(vertexFR, vertexBL);
		
		
		
		Vector3 gravityVec = Vector3.down * 999;
		Gizmos.DrawRay(vertexFL, gravityVec);
		Gizmos.DrawRay(vertexFR, gravityVec);
		Gizmos.DrawRay(vertexBL, gravityVec);
		Gizmos.DrawRay(vertexBR, gravityVec);
	}
}
