using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactPool : MonoBehaviour {
    
    private int summonSize = PoolManager.Instance.ImpactSummonSize;
    [SerializeField]
    private Queue<GameObject> impactPool = new Queue<GameObject>();
    

    public GameObject CallObject(GameObject impact) {
        
        foreach (GameObject curImpact  in impactPool) {

            if (curImpact == null) continue;
            
            if (curImpact.activeSelf == false) {
                curImpact.SetActive(true);
                return curImpact;
            }
        }
        
        
        GameObject obj = default;
        for (int i = 0; i < summonSize; i++) {
            
            obj = Instantiate(impact, transform);
            
            obj.SetActive(false);
            impactPool.Enqueue(obj);
        }
        
        obj.SetActive(true);
        return obj;
    }
}
