using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerCamera : MonoBehaviour {
    
    [SerializeField] private Camera camera;
    [SerializeField] private Transform camTransform;
    [Space(10)]
    [SerializeField] private float mouseSensitive;
    [Space(5)]
    [SerializeField, Range(0, 90)] private float camAboveLimit;
    [SerializeField, Range(0, 90)] private float camBelowLimit;
    [SerializeField, Range(0, 180)] private float weightChangeStart;
    [Space(5)]
    [SerializeField, Range(0.001f, 179)] private float defaultFOV;
    [SerializeField, Range(0.001f, 179)] private float assaultFOV;
    [SerializeField, Range(0.001f, 179)] private float aimFOV;
    [SerializeField] private float fovBlendSpeed;
    [Space(10)]
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private Vector3 aimPosition;
    [SerializeField] private float camDistance;
    
    private Vector3 camArmPosition;
    private Vector3 camArmPoint;
    private float targetFOV;
    private float aimIkWeight;
    private MultiAimConstraint aimSpineRig;

    private PlayerMove moveCtrl;
    private PlayerWeapon wpCtrl;
    
    public Transform CamTransform { get => camTransform; }
    public Vector3 CamEulerAngle { get => camera.gameObject.transform.eulerAngles; }
    public float AimIkWeight { get => aimIkWeight; }
    
    
    

    void OnEnable() {
        moveCtrl = GetComponent<PlayerMove>();
        wpCtrl = GetComponent<PlayerWeapon>();
    }
    
    
    
    void Update() {
        
        // 카메라 회전
        Vector3 inputVec = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        Vector3 angle = camTransform.eulerAngles;
        angle += inputVec * mouseSensitive;

        if (angle.x > 180) {
            angle.x = Mathf.Clamp(angle.x, 360 - camBelowLimit, 370);
        } else angle.x = Mathf.Clamp(angle.x, -10, camAboveLimit);

        camTransform.eulerAngles = angle;
        
        
        // 카메라암 배치
        camArmPosition = transform.position;
        camArmPosition += Quaternion.Euler(0, camTransform.eulerAngles.y, camTransform.eulerAngles.z) 
                          * Vector3.Lerp(defaultPosition, aimPosition, wpCtrl.AimBlend);
        
        
        // 카메라 배치
        camTransform.position = camArmPosition;
        camTransform.position += (camTransform.rotation * Vector3.back) * camDistance;
            // 카메라암의 뒤로 camDistance만큼 떨어진 거리에 카메라를 배치한다
        
        
        // FOV 제어
        if (moveCtrl.IsFly)    targetFOV = assaultFOV;
        else if (wpCtrl.IsAim) targetFOV = aimFOV;
        else                   targetFOV = defaultFOV;
        
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, fovBlendSpeed * Time.deltaTime);
        
        
        // IK 제어
        float diffAngle = Mathf.Abs(Mathf.DeltaAngle(camTransform.eulerAngles.y, transform.eulerAngles.y));
        aimIkWeight = 1 - ( Mathf.Clamp((diffAngle - weightChangeStart) / (180 - weightChangeStart), 0, 1) );
            // AimSpine Ik의 무게를
            // ( WeightChangeStart(최소각도) )0~1( 180도 )로 설정한다
    }



    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;

        Vector3 somePos = new Vector3(camArmPosition.x, transform.position.y, camArmPosition.z);
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawLine(transform.position, somePos);
        Gizmos.DrawLine(somePos, camArmPosition);
        
        Gizmos.DrawSphere(camArmPosition, 0.2f);
        Gizmos.DrawWireSphere(camArmPosition, 0.3f);
        Gizmos.DrawSphere(camTransform.position, 0.2f);
        Gizmos.DrawLine(camArmPosition, camTransform.position);
    }
}
