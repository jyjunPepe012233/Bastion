using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerPool : MonoBehaviour {
    
    private int summonSize = PoolManager.Instance.LazerSummonSize;
    [SerializeField]
    private Queue<GameObject> lazerPool = new Queue<GameObject>();
    

    public GameObject CallObject(GameObject lazer) {
        
        foreach (GameObject curLazer  in lazerPool) { // Pool에서 비활성화된 레이저가 있을 때

            if (curLazer == null) continue;
            
            if (curLazer.activeSelf == false) {
                curLazer.SetActive(true);
                return curLazer;
            }
        }
        
        
        GameObject obj = default;
        for (int i = 0; i < summonSize; i++) { // Pool에 오브젝트를 추가
            
            obj = Instantiate(lazer, transform);
            
            obj.SetActive(false);
            lazerPool.Enqueue(obj);
        }
        
        obj.SetActive(true);
        return obj;
    }
}
