using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bastion.EnemyAIStates;
using UnityEngine;

public class TurretAI : MonoBehaviour {
	
	[Serializable]
	private enum MuzzleType { Single, Double }

	private struct DetectedEnemy {
		public Collider collider;
		public float distance;

		public DetectedEnemy(Collider collider, float distance) {
			this.collider = collider;
			this.distance = distance;
		}
	}
	
	[SerializeField] private Transform body;
	[SerializeField] private Transform barrel;
	[SerializeField] private GameObject projectile;
	[Space(10)]
	[SerializeField] private ParticleSystem fireFX;
	[SerializeField] private Transform minRangeQuad;
	[SerializeField] private Transform maxRangeQuad;
	[Header("[ Status ]")]
	[SerializeField] private float maxDistance;
	[SerializeField] private float minDistance;
	[SerializeField] private float attackTime;
	[Header("[ Muzzle ]")]
	[SerializeField] private MuzzleType muzzleType;
	[SerializeField] private Transform muzzleMain;
	[SerializeField] private Transform muzzleSub;

	private GameObject target;
	private float attackCool;

	void OnValidate() {
		minRangeQuad.localScale = Vector3.one * minDistance * 2.3f;
		maxRangeQuad.localScale = Vector3.one * maxDistance * 2f;
	}


	void Update() {

		CalculateTarget();
		
		if (attackCool > 0) attackCool -= Time.deltaTime;
		
		if (attackCool <= 0 && target != null)
			Attack();

		if (target != null) {
			Vector3 targetDirx = (target.transform.position - transform.position).normalized;
			body.forward = new Vector3(targetDirx.x, 0f, targetDirx.z);
			if (barrel != null)
				barrel.forward = targetDirx;
		}
	}
	


	void Attack() {
		attackCool = attackTime;

		GameObject proj;
		
		switch (muzzleType) {
			
			case MuzzleType.Single:
				proj = PoolManager.Instance.InstantiateProjectile(projectile);
				proj.GetComponent<TurretProjectile>().SetUp(muzzleMain.transform, target.transform);
				break;
			
			case MuzzleType.Double:
				proj = PoolManager.Instance.InstantiateProjectile(projectile);
				proj.GetComponent<TurretProjectile>().SetUp(muzzleMain.transform, target.transform);
				
				proj = PoolManager.Instance.InstantiateProjectile(projectile);
				proj.GetComponent<TurretProjectile>().SetUp(muzzleSub.transform, target.transform);
				break;
		}

		fireFX.Play();
		
	}



	void CalculateTarget() {
		
		// 타겟 제거 처리
		if (target != null) {
			float targetDistance = Vector3.Distance(transform.position, target.transform.position);
			if (targetDistance < minDistance || targetDistance > maxDistance || !target.activeSelf)
				target = null;
		}


		// 타겟이 없을 때 설정
		if (target == null) {

			List<DetectedEnemy> detectedEnemys = new List<DetectedEnemy>();

			Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance, LayerMask.GetMask("Enemy"));
			foreach (Collider coll in colliders) {
				
				float distance = Vector3.Distance(transform.position, coll.ClosestPointOnBounds(transform.position));
				if (distance < minDistance) continue; // 배열에 추가하지 않음

				detectedEnemys.Add(new DetectedEnemy(coll, distance));

			} // DetectedEnemy 배열에 범위 조건에 해당하는 적들을 구조체 형태로 저장함


			// 거리에 따라 DetectedEnemy 배열을 정렬함
			for (int i=0; i < detectedEnemys.Count; i++) {
				float min = detectedEnemys[i].distance;

				for (int j = i; j < detectedEnemys.Count; j++) {

					if (detectedEnemys[j].distance < min)
						(detectedEnemys[j], detectedEnemys[i]) = (detectedEnemys[i], detectedEnemys[j]);
				}
			}

			if (detectedEnemys.Any())
				target = detectedEnemys[0].collider.gameObject;
		}
	}
	
	
	
	
	


	public void OnDrawGizmos() {

		if (target != null) {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, target.transform.position);
		}
	}
}
