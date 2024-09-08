using System.Collections;
using UnityEngine;

public enum PlayerMoveState {
    Ground,
    Assault
}

public class PlayerMove : MonoBehaviour {
    
    [SerializeField] private float defaultSpeed;
    [SerializeField] private float aimSpeed;
    [SerializeField] private float runMultiplier;
    [Space(5)]
    [SerializeField] private float jumpSpeedMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpDelay, landDelay, assaultLandDelay;
    [Space(5)]
    [SerializeField] private float flightSpeed;
    [SerializeField] private float tiltFixHeight;
    [SerializeField] private ParticleSystem flightParticle;
    [Space(10)]
    [SerializeField] private float speedTransitionTime;
    [Space(5)]
    [SerializeField] private float accelTime;
    [SerializeField] private float breakTime;
    [Space(5)]
    [SerializeField] private float rotationSpeed;
    [Space(10)]
    [SerializeField] private bool showGizmos;

    [Space(20)]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isFly;
    [SerializeField] private bool isRun;
    [SerializeField] private bool isMove;
    [SerializeField] private bool isFall;
    [SerializeField] private PlayerMoveState curState;
    private Vector3 moveVel;
    private Vector3 targetDirx;
    private Vector3 moveInput;
    private Vector3 defaultForward;
    private float curSpeed;
    private float assaultModeWeight;
    private bool inputSpace;
    private IEnumerator curJump;
    private IEnumerator lastLand;

    private Player main;
    private PlayerCamera camCtrl;
    private PlayerAnimation aniCtrl;
    private PlayerWeapon wpCtrl;
    private Rigidbody rigid;
    private Collider collider;

    public bool IsMove { get => isMove; }
    public bool IsRun { get => isRun; }
    public bool IsGrounded { get => isGrounded; }
    public bool IsJumping { get => isJumping; }
    public bool IsFly { get => isFly; }
    public bool IsFall{ get => isFall; }

    public PlayerMoveState CurState {
        get => curState;
    }


    
    void Awake() {
        showGizmos = true;
        
        curState = PlayerMoveState.Ground;

        main = GetComponent<Player>(); 
        camCtrl = GetComponent<PlayerCamera>();
        aniCtrl = GetComponent<PlayerAnimation>();
        wpCtrl = GetComponent<PlayerWeapon>();
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }
    
    
    
    void Update() {
        
        // 참조값 설정
        float distanceToGround = collider.bounds.extents.y;
        isGrounded = Physics.Raycast(collider.bounds.center, Vector3.down, distanceToGround + 0.1f);
        
        isFly = curState == PlayerMoveState.Assault && !isGrounded;
        
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        isMove = moveInput.magnitude != 0;
        isRun = isMove && Input.GetKey(KeyCode.LeftShift) && !wpCtrl.IsAim;

        if (curState != PlayerMoveState.Ground) {
            isMove = false;
            isRun = false;
        }
        
        
        
        // 움직임 상태 설정
        inputSpace = Input.GetKeyDown(KeyCode.Space);

        if (curState == PlayerMoveState.Assault && isGrounded) StartCoroutine(AssaultLand());
        if (isGrounded && isJumping && isFall) StartCoroutine(JumpLand());
        
        Vector3 camDirx = Quaternion.Euler(camCtrl.CamEulerAngle) * Vector3.forward;
        if (inputSpace && !isGrounded && camDirx.y > -0.2f && rigid.velocity.y > 0) {
            curState = PlayerMoveState.Assault; 
            isJumping = false;
            isFall = false;
        }
        
        
        // 움직임
        switch (curState) {
            case PlayerMoveState.Ground:
                DefaultMove();
                break;
            
            case PlayerMoveState.Assault:
                AssaultMove();
                break;
        }
        
        
        // 파티클
        assaultModeWeight += ((curState == PlayerMoveState.Assault && !isGrounded) ? 10 : -10) * Time.deltaTime;
        assaultModeWeight = Mathf.Clamp(assaultModeWeight, 0, 1);
            // (일반)0~1(어설트모드)
        
        flightParticle.emission.SetBurst(0, new ParticleSystem.Burst(0f, (int)(assaultModeWeight * 10)) );
            // assaultModeWeight에 따라 파티클의 수를 0~10까지 블랜드함
    }

    


    void DefaultMove() {
        if (isJumping) return;
        
        //속도 계산
        float standardSpeed = wpCtrl.IsAim ? aimSpeed : defaultSpeed;
        
        float runAddSpeed = (runMultiplier - 1) * standardSpeed;
        if (isRun) {
            curSpeed += runAddSpeed  / speedTransitionTime * Time.deltaTime;
        } else curSpeed -= runAddSpeed / speedTransitionTime * Time.deltaTime;
        curSpeed = Mathf.Clamp(curSpeed, standardSpeed, standardSpeed * runMultiplier);


        Quaternion fixedRotation = Quaternion.Euler(0, camCtrl.CamEulerAngle.y, camCtrl.CamEulerAngle.z);
        Vector3 addVel = fixedRotation * moveInput * curSpeed;
        
        
        // 감속 및 가속
        if (addVel.magnitude != 0) {
            moveVel += addVel / accelTime * Time.deltaTime;
            moveVel = Vector3.ClampMagnitude(moveVel, curSpeed);
            
        } else {
            float nextSpeed = moveVel.magnitude - (curSpeed / breakTime * Time.deltaTime);
            if (nextSpeed < 0) nextSpeed = 0;
            moveVel = moveVel.normalized * nextSpeed;
        }
        rigid.velocity = new Vector3(moveVel.x, rigid.velocity.y, moveVel.z);
        
        
        // 점프  
        if (inputSpace && !isJumping) {
            curJump = Jump();
            StartCoroutine(curJump);
        }
        
        
        // 캐릭터 방향
        float tParam = rotationSpeed * Time.deltaTime;

        if (wpCtrl.IsAim) {
            targetDirx = fixedRotation * Vector3.forward;
            transform.forward = Vector3.Slerp(transform.forward, targetDirx, tParam);
            
        } else if (addVel.magnitude != 0) {
            targetDirx = addVel.normalized;
            defaultForward = Vector3.Slerp(transform.forward, targetDirx, tParam);
            transform.forward = defaultForward; 

        } else {
            transform.forward =
                Vector3.Slerp(transform.forward, new Vector3(targetDirx.x, 0, targetDirx.z), tParam);
        }
        
    }
    
    
    // 나는 똑똑해
    IEnumerator Jump() {
        
        aniCtrl.Jump();
        isJumping = true;
        
        Vector3 boostedVel = rigid.velocity * jumpSpeedMultiplier;
        rigid.velocity = Vector3.zero;
        
        yield return new WaitForSeconds(jumpDelay);
        rigid.AddForce(new Vector3(boostedVel.x, jumpForce, boostedVel.z), ForceMode.Impulse);

        yield return new WaitForSeconds(0.1f);
        isFall = true;
    }
    
    
    
    IEnumerator JumpLand() {
        
        isFall = false;

        rigid.velocity = Vector3.zero;
        yield return new WaitForSeconds(landDelay);
        
        StopCoroutine(curJump);
        
        if (isJumping) {
            curState = PlayerMoveState.Ground;
            isJumping = false;
        }
    }
    
    
    
    IEnumerator AssaultLand() {
        
        aniCtrl.Land();
        rigid.velocity = Vector3.zero;

        yield return new WaitForSeconds(assaultLandDelay);
        
        if (isGrounded) {
            curState = PlayerMoveState.Ground;
        }

        targetDirx = new Vector3(transform.forward.x, 0, transform.forward.z);
    }


    
    void AssaultMove() {
        if (isGrounded) return;

        // 캐릭터 방향
        Vector3 moveDirx = Quaternion.Euler(camCtrl.CamEulerAngle) * Vector3.forward;
        transform.forward = Vector3.Slerp(transform.forward, moveDirx, rotationSpeed * Time.deltaTime);
        
        rigid.velocity = moveDirx * flightSpeed;

        
        // 높이에 따른 캐릭터 방향(상하) 제어
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, tiltFixHeight)) {
            
            float normalizedHeight = Vector3.Distance(hitInfo.point, ray.origin) / tiltFixHeight;
            moveDirx.y = Mathf.Lerp(0, moveDirx.y, normalizedHeight);
            
        }
        transform.forward = new Vector3(transform.forward.x, moveDirx.y, transform.forward.z);
        
    }


    
    private void OnDrawGizmos() {
        if (!showGizmos) return;
    
        Gizmos.color = Color.red;
        
        Gizmos.DrawRay(transform.position, targetDirx * 2);
        Gizmos.DrawRay(transform.position, transform.forward * 1);
        Gizmos.DrawRay(transform.position, moveVel);
        
        
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, tiltFixHeight)) {

            if (curState == PlayerMoveState.Assault) {
                Gizmos.DrawLine(ray.origin, hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, 0.3f);
            }
            
        }
        
        
    }
}