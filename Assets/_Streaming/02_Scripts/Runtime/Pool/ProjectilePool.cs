using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {
    
    [SerializeField]
    private Queue<GameObject> projectilePool = new Queue<GameObject>();
    
    

    public GameObject CallObject(GameObject projectile) {
        
        foreach (GameObject curProjectile in projectilePool) {

            if (curProjectile == null) continue;
            
            if (curProjectile.activeSelf == false) {
                curProjectile.SetActive(true);
                return curProjectile;
            }
        }
        
            
        GameObject obj = Instantiate(projectile, transform);
        projectilePool.Enqueue(obj);
        return obj;
    }
}
