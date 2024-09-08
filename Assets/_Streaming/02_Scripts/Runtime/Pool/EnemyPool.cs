using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour {
    
    [SerializeField]
    private Queue<GameObject> enemyPool = new Queue<GameObject>();
    
    

    public GameObject CallObject(GameObject enemy) {
        
        foreach (GameObject curEnemy in enemyPool) {

            if (curEnemy == null) continue;
            
            if (curEnemy.activeSelf == false) {
                curEnemy.SetActive(true);
                return curEnemy;
            }
        }
        
            
        GameObject obj = Instantiate(enemy, transform);
        enemyPool.Enqueue(obj);
        return obj;
    }


    public int GetActiveCount() {

        int count = 0;
        
        foreach (GameObject enemy in enemyPool) {
            if (enemy.activeSelf) count += 1;
        }

        return count;
    }
}
