using System;
using System.Collections;
using System.Collections.Generic;
using Bastion.EnemyAIStates;
using UnityEngine;

public class TurretProjectile : MonoBehaviour {
	
	[Serializable]
	public enum Type { Gun, Missile, Howitzer }

	[SerializeField] private Type type;
	[Space(10)]
	[SerializeField] private int damage;
	[Space(10)]
	[SerializeField] private GameObject bullet;
	[SerializeField] private ParticleSystem impact;
	[SerializeField] private DamageCollider damageField;
	[Header("[ Gun ]")]
	[SerializeField] private float speed;
	[Header("[ Missile ]")]
	[SerializeField] private float followSpeed;
	[SerializeField] private float fireSpeed;
	[SerializeField] private float fireForceResistance;
	[SerializeField] private float rotationSpeed;
	[Header("[ Howitzer ]")]
	[SerializeField] private float parabolaSize;
	[SerializeField] private float reachTime;
	
	private Transform target;
	private Vector3 fireForce;
	private Vector3 targetPosition;
	private Vector3 linearPosition;
	private Vector3 firePosition;
	private Vector3 targetPoint;
	private float progress;
	private bool isWhileImpact;

	private Rigidbody rigidbody;

	private void OnValidate() {
		damageField.Damage = damage;
	}


	private void Awake() {

		rigidbody = GetComponent<Rigidbody>();
		
	}


	public void SetUp(Transform muzzle, Transform target) {
		
		this.target = target;
		isWhileImpact = false;
		progress = 0;
		
		bullet.SetActive(true);

		transform.position = muzzle.position;
		transform.rotation = muzzle.rotation;

		switch (type) {
			
			case Type.Gun:
				transform.forward = target.position - transform.position;
				break;
			
			case Type.Missile:
				fireForce = transform.forward * fireSpeed;
				break;
			
			case Type.Howitzer:
				targetPosition = target.position;
				firePosition = transform.position;
				break;
				
		}
		
	}



	void Update() {
		
		if (isWhileImpact) {
			rigidbody.velocity = Vector3.zero;
			return;
		}
		
		switch (type) {

			case Type.Gun:
				rigidbody.velocity = transform.forward * speed;
				
				progress += Time.deltaTime;
				if (progress > 5) StartCoroutine(Trigger());
					// 5초 이상 비행하면 터짐
				break;


			case Type.Missile:
				Vector3 targetDirx = (target.position - transform.position).normalized;
				Vector3 flyDirx = Vector3.Slerp(transform.forward, targetDirx, Time.deltaTime * rotationSpeed);
				transform.forward = flyDirx;
					// 유도 방향

				fireForce = Vector3.Lerp(fireForce, Vector3.zero, Time.deltaTime * fireForceResistance); 
				rigidbody.velocity = (flyDirx * followSpeed) + fireForce;
					// 발사 힘을 저항에 따라 줄여나감
					// 발사 힘을 Velocity에 반영함
				break;
			
			
			case Type.Howitzer:
				progress += Time.deltaTime / reachTime;
				
				linearPosition = Vector3.LerpUnclamped(firePosition, targetPosition, progress);
				Vector3 parabolaVector = Mathf.Sin(progress * Mathf.PI) * parabolaSize * Vector3.up;
					// 실제로는 직선적으로 향하고 있지만 Sin함수의 도출값을 y에 더해 포물선처럼 보이게 함

				transform.forward = (linearPosition + parabolaVector) - transform.position;
				transform.position = linearPosition + parabolaVector;
					// targetPoint에 도달할 수 있도록 velocity를 조절함

				if (progress > 1.5)
					StartCoroutine(Trigger());
				break;
		}
		
		
	}


	
	private void OnTriggerEnter(Collider other) {
		
		StartCoroutine(Trigger());
	}

	
	
	IEnumerator Trigger() {
		if (isWhileImpact) yield break;
			
		isWhileImpact = true;
		
		impact.transform.forward = Vector3.forward;
		impact.Play();
		damageField.gameObject.SetActive(true);
		
		bullet.SetActive(false);

		yield return new WaitForSeconds(1f);
			// 투사체는 트리거가 발동되고 무조건 1초 후 비활성화됨
		
		gameObject.SetActive(false);
		damageField.gameObject.SetActive(false);
	}


	private void OnDrawGizmosSelected() {

		Gizmos.color = Color.yellow;
		
		switch (type) {
			
			case Type.Gun:
				Gizmos.DrawRay(transform.position, transform.forward * 100);
				break;
			
			case Type.Missile: 
				Gizmos.DrawLine(target.position, transform.position);
				
				Gizmos.color = Color.white;
				Gizmos.DrawRay(transform.position, transform.forward);
				break;
			
			case Type.Howitzer:
				Gizmos.DrawLine(firePosition, targetPosition);
				Gizmos.DrawSphere(linearPosition, 0.2f);
				
				Gizmos.color = Color.white;
				Gizmos.DrawSphere(targetPoint, 0.2f);
				Gizmos.DrawLine(linearPosition, transform.position);

				Gizmos.color = Color.red;
				Gizmos.DrawRay(transform.position, targetPoint - transform.position);
				break;
		}
	}
}
