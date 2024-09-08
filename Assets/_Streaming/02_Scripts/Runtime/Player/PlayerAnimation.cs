using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimation : MonoBehaviour {
	
	[SerializeField] private MultiAimConstraint aimSpineRig;
	[Space(10)]
	[SerializeField] private float moveBlendTime;
	[SerializeField] private float runBlendTime;
	[SerializeField] private float flyBlendTime;
	[Space(5)]
	[SerializeField] private float landTime;
	[Space(20)]
	[SerializeField] private bool isLanding;
	[Space(5)]
	[SerializeField] private bool showGizmos;


	private RaycastHit landRayHitInfo;
	private float moveBlend;
	private float runBlend;
	private float horizMoveBlend;
	private float vertiMoveBlend;
	private float flyBlend;

	private PlayerCamera camCtrl;
	private PlayerMove moveCtrl;
	private PlayerWeapon wpCtrl;
	private Animator animator;
	private Rigidbody rigid;

	void Awake() {
		showGizmos = true;

		camCtrl = GetComponent<PlayerCamera>();
		moveCtrl = GetComponent<PlayerMove>();
		wpCtrl = GetComponent<PlayerWeapon>();
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody>();
	}

	void Update() {
		
		animator.SetBool("IsFly", moveCtrl.IsFly);
		animator.SetBool("IsJumping", moveCtrl.IsJumping);


		// 기본 블랜드 제어
		BlendCalculate(moveCtrl.IsMove, moveBlendTime, ref moveBlend);
		BlendCalculate(moveCtrl.IsRun, runBlendTime, ref runBlend);
		
		animator.SetFloat("MoveBlend", moveBlend);
		animator.SetFloat("RunBlend", runBlend);
		animator.SetFloat("AimBlend", wpCtrl.AimBlend);
		
		
		// 축 블랜드 제어
		AxisBlendCalcuate(Input.GetAxisRaw("Horizontal"), moveBlendTime, ref horizMoveBlend);
		AxisBlendCalcuate(Input.GetAxisRaw("Vertical"), moveBlendTime, ref vertiMoveBlend);
		
		animator.SetFloat("HorizMoveBlend", horizMoveBlend);
		animator.SetFloat("VertiMoveBlend", vertiMoveBlend);
		
		// 점프
		if (moveCtrl.IsFall && rigid.velocity.y < 0 && !isLanding) {

			Ray ray = new Ray(transform.position, Vector3.down);
			if (Physics.Raycast(ray, landTime * -rigid.velocity.y)) {
				Land();
			}	// 가속도를 반영하여 착지 애니메이션이 정확한 타이밍에 작동하도록 함
		}

		
		// IK 제어
		BlendCalculate(moveCtrl.IsFly, flyBlendTime, ref flyBlend);
		aimSpineRig.weight = camCtrl.AimIkWeight * (1-flyBlend);
			// 비행 시엔 ik 무게를 0까지 낮춤

	}


	
	public void Jump() {
		animator.ResetTrigger("LandTrigger");
		animator.SetTrigger("JumpTrigger");
		isLanding = false;
	}
	


	public void Land() {
		animator.SetTrigger("LandTrigger");
		isLanding = true;
	}
	
	
	

	float BlendCalculate(bool parameter, float blendTime, ref float blendVariable) {
		
		if (parameter) {
			blendVariable += Time.deltaTime / blendTime;
		} else blendVariable -= Time.deltaTime / blendTime;

		blendVariable = Mathf.Clamp(blendVariable, 0, 1);

		return blendVariable;
	}

	

	/// blend Variable can slide as per parameter
	void AxisBlendCalcuate(float parameter, float blendTime, ref float blendVariable) {
		

		if (parameter != 0) {
			blendVariable += parameter / blendTime * Time.deltaTime;
			blendVariable = Mathf.Clamp(blendVariable, -1, 1);
			
		} else if (blendVariable != 0) {
			float deltaValue = -Mathf.Sign(blendVariable) / blendTime * Time.deltaTime;
			
			if (Mathf.Sign(blendVariable + deltaValue) != Mathf.Sign(blendVariable)) {
				blendVariable = 0;
			} else blendVariable += deltaValue;
		}
	}

	

	public void Died() {
		
		animator.SetTrigger("DieTrigger");
		enabled = false;

	}



	void OnDrawGizmos() {
		if (!showGizmos) return;
        
		Gizmos.color = Color.yellow;

		if (moveCtrl.IsFall && rigid.velocity.y < 0 && !isLanding) {

			Vector3 rayDown = Vector3.down * landTime * -rigid.velocity.y;
			Gizmos.DrawRay(transform.position, rayDown);
			Gizmos.DrawSphere(transform.position + rayDown, 0.1f);
		}
	}
}
