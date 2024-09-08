using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SupplyBox : MonoBehaviour {
    
    [SerializeField] private float fallHeight;
    [SerializeField] private float fallTime;
    [Space(5)]
    [SerializeField] private AnimationCurve lineWidth;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [Space(10)]
    [SerializeField] private GameObject landParticle;

    [SerializeField] private WorldSpaceUI boxMark;
    [SerializeField] private WorldSpaceUI boxFallWarning;
    private Vector3 fallPos;
    private Vector3 landPos;
    private float gravity;
    private bool isLanding;
    private bool isLanded;

    private LineRenderer line;
    
    
    
    void Awake() {

        line = GetComponent<LineRenderer>();
        
        landPos = transform.position;
        fallPos = landPos + (Vector3.up * fallHeight);

        transform.position = fallPos;

        boxMark = UIManager.Instance.BoxMark;
        boxFallWarning = UIManager.Instance.BoxFallWarning;
        
        boxMark.SetEnabled(true);
        boxFallWarning.SetEnabled(false);
    }
    
    void Update() {

        if (!isLanding) {

            if (GameManager.Instance.IsGaming) Landing();
            else boxMark.SetPosition(landPos);
            
        } // 게임이 시작되면 함수를 호출하여 UI를 끔
          // 그 전까지는 UI의 위치을 계속 지정함
        

        
        if (isLanding && !isLanded) {
            
            // 착륙
            gravity += Time.deltaTime / fallTime;
            transform.position = Vector3.Lerp(fallPos, landPos, gravity);
            
            if (Vector3.Distance(transform.position, landPos) < 1) {
                
                transform.position = landPos;
                landParticle.SetActive(true);
                
                boxFallWarning.SetEnabled(false);
                line.enabled = false;
                isLanded = true;
            }
            

            // 라인 렌더러
            line.startWidth = lineWidth.Evaluate(gravity);
            line.endWidth = lineWidth.Evaluate(gravity);

            line.positionCount = 2;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, landPos);

            Color color = Color.Lerp(startColor, endColor, gravity);
            line.SetColors(color, color);
            
            
            // 착륙 경고
            boxFallWarning.SetPosition(landPos);
            boxFallWarning.Ui.GetComponent<TextMeshProUGUI>().text =
                "보급 물자 착륙까지 0:" + Mathf.FloorToInt(fallTime - (gravity * fallTime)).ToString("00");
        }
    }

    void Landing() {

        boxMark.SetEnabled(false);
        landParticle.SetActive(false);
        
        boxFallWarning.SetEnabled(true);
        gameObject.SetActive(true);
        line.enabled = true;
        isLanding = true;
        
    }
}
