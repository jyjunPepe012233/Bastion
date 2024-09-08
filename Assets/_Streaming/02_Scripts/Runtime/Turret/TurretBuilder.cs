using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBuilder : MonoBehaviour {

    private struct PreviewData {
        public GameObject instance;
        public TurretPreview data;

        public PreviewData(GameObject instance, TurretPreview data) {
            this.instance = instance;
            this.data = data;
        }
    }

    [SerializeField] private Color colorCanBuild;
    [SerializeField] private Color colorCantBuild;
    [Space(10)]
    [SerializeField] private TurretDataSO[] turretDatas;

    private PreviewData[] previews;
    private PreviewData curPreview;
    private int buildRayLayerMask;
    private int curPreviewIndex;
    private bool isCanBuild;



    void Awake() {

        previews = new PreviewData[turretDatas.Length];

        buildRayLayerMask = ~LayerMask.GetMask("Player", "Enemy", "Projectile");
        
    }




    public int GetTurretIndex(string name) {

        for (int i = 0; i < turretDatas.Length; i++) {

            if (turretDatas[i].name.ToUpper() == name.ToUpper())
                return i;
        }
        
        Debug.LogError("Can't find turret data to this name " + name.ToUpper());
        return 0;
    }


    private void Update() {

        // 건설 모드 토글
        if (Input.GetKeyDown(KeyCode.B)) {
            
            GameManager.Instance.IsBuildMode = !GameManager.Instance.IsBuildMode;

            if (GameManager.Instance.IsBuildMode) {
                SetPreviewBuild();
            } else CancelPreview();
        }


        // 터렛 전환
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            
            curPreviewIndex += 1;
            if (curPreviewIndex >= previews.Length) curPreviewIndex = 0;
            
            SetPreviewBuild();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            
            curPreviewIndex -= 1;
            if (curPreviewIndex <= 0) curPreviewIndex = 0;
            
            SetPreviewBuild();
        }
        
        
        
        // 터렛 설치
        if (GameManager.Instance.IsBuildMode)
            PreviewManage();
    }



    private void PreviewManage() {
        
        Ray buildRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(buildRay, out RaycastHit hitInfo, 20, buildRayLayerMask)) {

            curPreview.instance.transform.position = hitInfo.point;

            isCanBuild = !curPreview.data.IsColliding;
            curPreview.data.SetColor(isCanBuild ? colorCanBuild : colorCantBuild);

            if (isCanBuild) {

                if (Input.GetMouseButtonDown(0)) {
                    Build(hitInfo.point);
                    CancelPreview();
                }
                    
            }
        }
        
    }



    private void SetPreviewBuild() {

        if (curPreview.instance != null)
            curPreview.instance.SetActive(false);
            // 이전 오브젝트를 끔
            
        if (previews[curPreviewIndex].instance == null) {
            var newPreview = Instantiate(turretDatas[curPreviewIndex].previewObject);
            previews[curPreviewIndex] = new PreviewData(newPreview, newPreview.GetComponent<TurretPreview>());
        } // 호출하려는 프리뷰 오브젝트가 없으면 생성해서 배열에 저장함
        
        previews[curPreviewIndex].instance.gameObject.SetActive(true);
        curPreview = previews[curPreviewIndex];

        UIManager.Instance.BuildHUDEnabled = true;
        UIManager.Instance.BuildHUDText = turretDatas[curPreviewIndex].name;
        UIManager.Instance.BuildHUDCredit = turretDatas[curPreviewIndex].creditCost.ToString();
    }
    
    

    private void CancelPreview() {

        curPreview.instance.SetActive(false);
        UIManager.Instance.BuildHUDEnabled = false;

        GameManager.Instance.IsBuildMode = false;
    }

    private void Build(Vector3 position) {

        if (GameManager.Instance.UseCredit(turretDatas[curPreviewIndex].creditCost)) {
            // 크레딧을 사용한다.
            // 크레딧이 부족하면 허가되지 않음
            
            Instantiate(turretDatas[curPreviewIndex].turret, position, Quaternion.identity);
        }

    }
    
}
