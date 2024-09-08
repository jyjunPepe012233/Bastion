using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

	[SerializeField] private Transform target;
	[Space(5)]
	[SerializeField] private float aimFollowSpeed;
	[SerializeField] private float aimBlendTime;
	[Space(10)]
	[SerializeField] private Transform muzzle;
	[Space(5)]
	[SerializeField] private GameObject lazer;
	[SerializeField] private float lazerlifeTime;
	[SerializeField] private AnimationCurve lazerWidth;
	[SerializeField] private float damage;
	[Space(5)]
	[SerializeField] private GameObject impactObj;
	[Space(5)]
	[SerializeField] private float attackCoolTime;
	[Space(10)]
	[SerializeField] private bool showGizmos;

	[Space(20)]
	[SerializeField] private bool isAim;
	private Transform camTransform;
	private Vector3 targetPoint;
	private Vector3 aimDirx;
	private float aimBlend;
	private float attackCool;
	private int playerLayerMask;

	private PlayerCamera camCtrl;
	private PlayerMove moveCtrl;
	
	public bool IsAim { get => isAim; }
	public float AimBlend { get => aimBlend; }


	void Awake() {
		showGizmos = true;
		
		playerLayerMask = LayerMask.GetMask("Player");
		
		camCtrl = GetComponent<PlayerCamera>();
		moveCtrl = GetComponent<PlayerMove>();
		
		camTransform = camCtrl.CamTransform;
	}
	
	
	
	void Update() {

		// 기본 조준 제어
		isAim = Input.GetMouseButton(1);
		if (moveCtrl.CurState != PlayerMoveState.Ground || !moveCtrl.IsGrounded)
			isAim = false;
		
		
		if (isAim) {
			aimBlend += Time.deltaTime / aimBlendTime;
		} else aimBlend -= Time.deltaTime / aimBlendTime;
		
		aimBlend = Mathf.Clamp(aimBlend, 0, 1);
		
		
		// 타겟 제어
		aimDirx = Vector3.Slerp(aimDirx, camTransform.forward, aimFollowSpeed * Time.deltaTime);
			// 레이캐스트 방향
		
		Ray ray = new Ray(camTransform.position, aimDirx);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 50, ~playerLayerMask)) {
			targetPoint = hitInfo.point;

		} else targetPoint = (ray.direction * 50) + ray.origin;
			// 레이캐스트가 닿았을 때는 hitPoint를 타겟으로 지정
			// 닿지 않았을 때는 레이가 바라보는 방향(50만큼 떨어짐)이 타겟이 됨
		target.position = targetPoint;
		
		
		// 공격
		if (Input.GetMouseButton(0) && isAim && attackCool == 0) Shoot();

		attackCool -= Time.deltaTime;
		if (attackCool < 0) attackCool = 0;
			// 공격 쿨타임
	}



	void Shoot() {
		
		SoundManager.Instance.PlayGunSound(1);
		
		var lazerData = PoolManager.Instance.InstantiateLazer(lazer) .GetComponent<Lazer>();
		lazerData.InIt(lazerlifeTime, lazerWidth, muzzle.position, targetPoint);

		attackCool = attackCoolTime;
		
		Ray ray = new Ray(camTransform.position, aimDirx);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 50, ~playerLayerMask)) {
			Attack(hitInfo);
		}
	}


	
	void Attack(RaycastHit hitInfo) {
		
		var victim = hitInfo.transform;

		GameObject impact = PoolManager.Instance.InstantiateImpact(impactObj);
		
		if (victim.gameObject.CompareTag("Enemy")) {
			impact.GetComponent<Impact>().Init(0.5f, ImpactType.Enemy);
			victim.GetComponent<Enemy>().GetHit(damage);
		} else {
			impact.GetComponent<Impact>().Init(0.5f, ImpactType.Default);
		}

		impact.transform.position = hitInfo.point;
		impact.transform.up = hitInfo.normal;
	} 


	private void OnDrawGizmos() {
		if (!showGizmos) return;
		
		Gizmos.color = Color.green;
		
		Gizmos.DrawSphere(targetPoint, 0.1f);
		Gizmos.DrawLine(camTransform.position, targetPoint);
		Gizmos.DrawRay(transform.position, aimDirx * 3);
		
		Gizmos.DrawSphere(muzzle.position, 0.15f);
	}
}
